using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vent.AccessData.Data;
using Vent.Shared.Entities;
using Vent.Shared.Enum;
using Vent.Shared.ResponsesSec;

namespace Vent.Helpers;

public class UserHelper : IUserHelper
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUtilityTools _utilityTools;

    public UserHelper(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager, IUtilityTools utilityTools)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _utilityTools = utilityTools;
    }

    public async Task<User> GetUserAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user!;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId.ToString());
        return user!;
    }

    public async Task<IdentityResult> AddUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<bool> DeleteUser(string username)
    {
        User? userAsp = await _userManager.FindByEmailAsync(username);
        if (userAsp == null)
        {
            return false;
        }
        IdentityResult response = await _userManager.DeleteAsync(userAsp);
        return response.Succeeded;
    }

    public async Task CheckRoleAsync(string roleName)
    {
        bool roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = roleName
            });
        }
    }

    public async Task AddUserToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task RemoveUserToRoleAsync(User user, string roleName)
    {
        await _userManager.RemoveFromRoleAsync(user, roleName);
    }

    public async Task AddUserClaims(UserType userType, string email)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        await _userManager.AddClaimAsync(usuario!, new Claim(userType.ToString(), "1"));
    }

    public async Task RemoveUserClaims(UserType userType, string email)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        await _userManager.RemoveClaimAsync(usuario!, new Claim(userType.ToString(), "1"));
    }

    public async Task<bool> IsUserInRoleAsync(User user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    //Sistema de Acceso de suarios
    public async Task<SignInResult> LoginAsync(LoginDTO model)
    {
        return await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    //Para Administrar El Cambio de Clave de los usuarios
    public async Task<IdentityResult> UpdateUserAsync(User user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    }

    //Para Validar Correo y Activar la cuenta del usuario
    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    //Recuperacion de Clave automatica del Usuario
    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
    {
        return await _userManager.ResetPasswordAsync(user, token, password);
    }

    //Registro de Usuarios al Sistema de User
    //Se pasa el Usuario y el Pass, para ver si es Valido el Usuario, nos da un Result
    //metodo para implementar la socitud del token de seguridad.
    public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
    {
        return await _signInManager.CheckPasswordSignInAsync(user, password, false);
    }

    public async Task<User> AddUserUsuarioAsync(string firstname, string lastname,
            string email, string phone, string address, string job,
            int Idcorporate, string ImagenFull, string Origin, bool UserActivo, UserType usertype)
    {
        string largo = "AaFfHhnNOPpsSRrErTtDcJjBmM098765#432@1";
        var clave = _utilityTools.GeneratePass(8, largo);

        User user = new User
        {
            FirstName = firstname,
            LastName = lastname,
            FullName = $"{firstname} {lastname}",
            Email = email,
            UserName = email,
            PhoneNumber = phone,
            JobPosition = job,
            CorporationId = Idcorporate,
            PhotoUser = ImagenFull,
            UserFrom = Origin,
            Activo = UserActivo,
            UserRoleDetails = new List<UserRoleDetails>
                {
                    new UserRoleDetails
                    {
                        UserType = usertype
                    }
                },
            Pass = clave
        };

        IdentityResult result = await _userManager.CreateAsync(user, clave); //(modelo, Password)
        if (result != IdentityResult.Success)
        {
            return null!;
        }

        User newUser = await GetUserAsync(email);
        await AddUserToRoleAsync(newUser, usertype.ToString());
        await AddUserClaims(usertype, email);
        return newUser;
    }

    public async Task<User> AddUserUsuarioSoftAsync(string firstname, string lastname,
    string email, string phone, string address, string job,
    int Idcorporate, string ImagenFull, string Origin, bool UserActivo)
    {
        string largo = "AaFfHhnNOPpsSRrErTtDcJjBmM098765#432@1";
        var clave = _utilityTools.GeneratePass(8, largo);

        User user = new User
        {
            FirstName = firstname,
            LastName = lastname,
            FullName = $"{firstname} {lastname}",
            Email = email,
            UserName = email,
            PhoneNumber = phone,
            JobPosition = job,
            CorporationId = Idcorporate,
            PhotoUser = ImagenFull,
            UserFrom = Origin,
            Activo = UserActivo,
            Pass = clave
        };

        IdentityResult result = await _userManager.CreateAsync(user, clave); //(modelo, Password)
        if (result != IdentityResult.Success)
        {
            return null!;
        }

        User newUser = await GetUserAsync(email);
        return newUser;
    }
}