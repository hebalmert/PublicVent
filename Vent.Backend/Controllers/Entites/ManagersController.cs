using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vent.AccessData.Data;
using Vent.Backend.Helpers;
using Vent.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.Enum;
using Vent.Shared.Pagination;
using Vent.Shared.Responses;

namespace Vent.Backend.Controllers.Entites;

[Route("api/managers")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class ManagersController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IUserHelper _userHelper;
    private readonly IConfiguration _configuration;
    private readonly IEmailHelper _emailHelper;
    private readonly string ImgRoute;

    public ManagersController(DataContext context, IFileStorage fileStorage,
        IUserHelper userHelper, IConfiguration configuration, IEmailHelper emailHelper)
    {
        _context = context;
        _fileStorage = fileStorage;
        _userHelper = userHelper;
        _configuration = configuration;
        _emailHelper = emailHelper;
        ImgRoute = "wwwroot\\Images\\ImgManager";
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Manager>>> GetCorporations([FromQuery] PaginationDTO pagination)
    {
        var queryable = _context.Managers.Include(x => x.Corporation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.FullName!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.FullName).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Manager>> GetCorporation(int id)
    {
        try
        {
            var modelo = await _context.Managers.Include(x => x.Corporation)
                .FirstOrDefaultAsync(x => x.ManagerId == id);

            if (modelo == null)
            {
                return NotFound();
            }

            return modelo;
        }
        catch (DbUpdateException dbUpdateException)
        {
            return BadRequest(dbUpdateException.InnerException!.Message);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    // PUT: api/Corporations/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut()]
    public async Task<IActionResult> PutCorporation(Manager modelo)
    {
        try
        {
            Manager NewModelo = new()
            {
                ManagerId = modelo.ManagerId,
                FirstName = modelo.FirstName,
                LastName = modelo.LastName,
                FullName = $"{modelo.FirstName} {modelo.LastName}",
                Nro_Document = modelo.Nro_Document,
                PhoneNumber = modelo.PhoneNumber,
                Address = modelo.Address,
                UserName = modelo.UserName,
                CorporationId = modelo.CorporationId,
                Job = modelo.Job,
                UserType = modelo.UserType,
                Photo = modelo.Photo,
                Active = modelo.Active,
            };
            if (modelo.ImgBase64 != null)
            {
                NewModelo.ImgBase64 = modelo.ImgBase64;
            }

            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();

            if (!string.IsNullOrEmpty(modelo.ImgBase64))
            {
                string guid;
                if (modelo.Photo == null)
                {
                    guid = Guid.NewGuid().ToString() + ".jpg";
                }
                else
                {
                    guid = modelo.Photo;
                }
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.Photo = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Managers.Update(modelo);
            await _context.SaveChangesAsync();

            User UserCurrent = await _userHelper.GetUserAsync(modelo.UserName);
            if (UserCurrent != null)
            {
                UserCurrent.FirstName = modelo.FirstName;
                UserCurrent.LastName = modelo.LastName;
                UserCurrent.FullName = $"{modelo.FirstName} {modelo.LastName}";
                UserCurrent.PhoneNumber = modelo.PhoneNumber;
                UserCurrent.PhotoUser = modelo.Photo;
                UserCurrent.JobPosition = modelo.Job;
                UserCurrent.Activo = modelo.Active;
                IdentityResult result = await _userHelper.UpdateUserAsync(UserCurrent);
            }
            else
            {
                if (modelo.Active)
                {
                    Response response = await AcivateUser(modelo);
                    if (response.IsSuccess == false)
                    {
                        var guid = modelo.Photo;
                        _fileStorage.DeleteImage(ImgRoute, guid!);
                        await transaction.RollbackAsync();
                        return BadRequest("No se ha podido crear el Usuario, Intentelo de nuevo");
                    }
                }
            }

            await transaction.CommitAsync();
            return Ok();
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
            {
                return BadRequest("Ya existe un Registro con el mismo nombre.");
            }
            else
            {
                return BadRequest(dbUpdateException.InnerException.Message);
            }
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    // POST: api/Corporations
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Manager>> PostCorporation(Manager modelo)
    {
        try
        {
            User CheckUser = await _userHelper.GetUserAsync(modelo.UserName);
            if (CheckUser != null)
            {
                return BadRequest("El Correo ingresado ya se encuentra reservado, debe cambiarlo.");
            }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();

            modelo.FullName = $"{modelo.FirstName} {modelo.LastName}";
            modelo.UserType = UserType.User;
            if (modelo.ImgBase64 is not null)
            {
                string guid = Guid.NewGuid().ToString() + ".jpg";
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.Photo = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Managers.Add(modelo);
            await _context.SaveChangesAsync();

            //Aseguramos los cambios en la base de datos
            if (modelo.Active)
            {
                Response response = await AcivateUser(modelo);
                if (response.IsSuccess == false)
                {
                    var guid = modelo.Photo;
                    _fileStorage.DeleteImage(ImgRoute, guid!);
                    await transaction.RollbackAsync();
                    return BadRequest("No se ha podido crear el Usuario, Intentelo de nuevo");
                }
            }

            await transaction.CommitAsync();
            return CreatedAtAction("GetCorporation", new { id = modelo.CorporationId }, modelo);
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
            {
                return BadRequest("Ya existe un Registro con el mismo nombre.");
            }
            else
            {
                return BadRequest(dbUpdateException.InnerException.Message);
            }
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    // DELETE: api/Corporations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCorporation(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.Managers.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.Managers.Remove(DataRemove);
            await _context.SaveChangesAsync();

            if (DataRemove.Photo is not null)
            {
                var response = _fileStorage.DeleteImage(ImgRoute, DataRemove.Photo);
                if (!response)
                {
                    return BadRequest("Se Elimino el Registro pero sin la Imagen");
                }
            }

            await _userHelper.DeleteUser(DataRemove.UserName);

            await transaction.CommitAsync();
            return NoContent();
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException!.Message.Contains("REFERENCE"))
            {
                return BadRequest("Existen Registros Relacionados y no se puede Eliminar");
            }
            else
            {
                return BadRequest(dbUpdateException.InnerException.Message);
            }
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    private async Task<Response> AcivateUser(Manager manager)
    {
        User user = await _userHelper.AddUserUsuarioAsync(manager.FirstName, manager.LastName, manager.UserName,
            manager.PhoneNumber, manager.Address, manager.Job, manager.CorporationId, manager.Photo!, "Manager", manager.Active, manager.UserType);

        //Envio de Correo con Token de seguridad para Verificar el correo
        string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
        string tokenLink = Url.Action("ConfirmEmail", "accounts", new
        {
            userid = user.Id,
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["UrlFrontend"])!.Replace("api/managers", "api/accounts");

        string subject = "Activacion de Cuenta";
        string body = ($"De: NexxtPlanet" +
            $"<h1>Email Confirmation</h1>" +
            $"<p>" +
            $"Su Clave Temporal es: <h2> \"{user.Pass}\"</h2>" +
            $"</p>" +
            $"Para Activar su vuenta, " +
            $"Has Click en el siguiente Link:</br></br><strong><a href = \"{tokenLink}\">Confirmar Correo</a></strong>");

        Response response = await _emailHelper.ConfirmarCuenta(user.UserName!, user.FullName!, subject, body);
        if (response.IsSuccess == false)
        {
            return response;
        }
        return response;
    }
}