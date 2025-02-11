using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vent.AccessData.Data;
using Vent.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.Enum;
using Vent.Shared.Responses;
using Vent.Shared.ResponsesSec;

namespace Vent.Backend.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;
    private readonly IConfiguration _configuration;
    private readonly IEmailHelper _emailHelper;

    public AccountsController(DataContext context, IUserHelper userHelper, IConfiguration configuration, IEmailHelper emailHelper)
    {
        _context = context;
        _userHelper = userHelper;
        _configuration = configuration;
        _emailHelper = emailHelper;
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO model)
    {
        string imgUsuario = string.Empty;
        string ImagenDefault = $"https://localhost:7148/images/NoImage.png";
        string BaseUrl = $"https://localhost:7148/images/";

        var result = await _userHelper.LoginAsync(model);
        if (result.Succeeded)
        {
            var user = await _userHelper.GetUserAsync(model.Email);
            if (user.Activo == false)
            {
                return BadRequest("El Usuario se Encuentra Inactivo, contacte al Administrador del Sistema");
            }
            var RolesUsuario = _context.UserRoleDetails.Where(c => c.UserId == user.Id).ToList();
            if (RolesUsuario.Count == 0)
            {
                return BadRequest("Este Usuario esta activo pero no tiene ningun Role Asignado...");
            }
            var RolUsuario = RolesUsuario.Where(c => c.UserType == UserType.Admin).FirstOrDefault();
            if (RolUsuario == null)
            {
                var CheckCorporation = await _context.Corporations.FirstOrDefaultAsync(x => x.CorporationId == user.CorporationId);
                DateTime hoy = DateTime.Today;
                DateTime current = CheckCorporation!.DateEnd;
                if (!CheckCorporation!.Active)
                {
                    return BadRequest("La Corporacion que trata de Acceder se encuentra Inactiva, Contacte al Administrador del Sistema");
                }
                if (current <= hoy)
                {
                    return BadRequest("El Tiempo del plan se ha cumplido, debe renovar su cuenta");
                }

                switch (user.UserFrom)
                {
                    case "Manager":
                        imgUsuario = user.PhotoUser != null ? $"{BaseUrl}ImgManager/{user.PhotoUser}" : ImagenDefault;
                        break;

                    case "UsuarioSoftware":
                        imgUsuario = user.PhotoUser != null ? $"{BaseUrl}ImgUsuarios/{user.PhotoUser}" : ImagenDefault;
                        break;
                }
            };

            return Ok(BuildToken(user, imgUsuario!));
        }
        if (result.IsLockedOut)
        {
            return BadRequest("Se Encuentra temporalmente Bloqueado");
        }

        if (result.IsNotAllowed)
        {
            return BadRequest("Lo Siento, No Tiene Acceso al Sistema");
        }

        return BadRequest("Usuario o Clave Erroneos");
    }

    [HttpPost("changePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }

        return NoContent();
    }

    [HttpPost("RecoverPassword")]
    public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
    {
        var user = await _userHelper.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        Response response = await SendRecoverEmailAsync(user);
        if (response.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(response.Message);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
    {
        var user = await _userHelper.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors.FirstOrDefault()!.Description);
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
    {
        token = token.Replace(" ", "+");
        var user = await _userHelper.GetUserAsync(new Guid(userId));
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userHelper.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault());
        }

        return NoContent();
    }

    private async Task<Response> SendRecoverEmailAsync(User user)
    {
        var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
        var tokenLink = Url.Action("ResetPassword", "accounts", new
        {
            userid = user.Id,
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["UrlFrontend"]);

        string subject = "Activacion de Cuenta";
        string body = ($"De: NexxtPlanet" +
            $"<h1>Para Recuperar su Clave</h1>" +
            $"<p>" +
            $"Para Crear una clave nueva " +
            $"Has Click en el siguiente Link:</br></br><strong><a href = \"{tokenLink}\">Cambiar Clave</a></strong>");

        Response response = await _emailHelper.ConfirmarCuenta(user.UserName!, user.FullName!, subject, body);
        if (response.IsSuccess == false)
        {
            return response;
        }
        return response;
    }

    private TokenDTO BuildToken(User user, string imgUsuario)
    {
        string NomCompa;
        string LogoCompa;
        var RolesUsuario = _context.UserRoleDetails.Where(c => c.UserId == user.Id).ToList();
        var RolUsuario = RolesUsuario.Where(c => c.UserType == UserType.Admin).FirstOrDefault();
        if (RolUsuario != null)
        {
            NomCompa = "Nexxplanet LLC";
            LogoCompa = $"https://localhost:7229/images/NexxtplanetLLC.png";
            imgUsuario = $"https://localhost:7229/images/NexxtplanetLLC.png";
        }
        else
        {
            var compname = _context.Corporations.FirstOrDefault(x => x.CorporationId == user.CorporationId);
            NomCompa = compname!.Name!;
            LogoCompa = compname!.ImageFullPath;
        }
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Photo", imgUsuario),
                new Claim("CorpName", NomCompa),
                new Claim("LogoCorp", LogoCompa),
            };
        // Agregar los roles del usuario a los claims
        foreach (var role in RolesUsuario)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.UserType.ToString()!));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(30);
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }
}