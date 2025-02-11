using Vent.AccessData.Data;
using Vent.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.Enum;

namespace Vent.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;

    public SeedDb(DataContext context, IUserHelper userHelper)
    {
        _context = context;
        _userHelper = userHelper;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckSoftPlan();
        await CheckCountries();
        await CheckRolesAsync();
        await CheckUserAsync("Nexxtplanet", "SPI", "soporte@nexxtplanet.net", "+1 786 503", UserType.Admin);
    }

    private async Task CheckRolesAsync()
    {
        await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
        await _userHelper.CheckRoleAsync(UserType.User.ToString());
        await _userHelper.CheckRoleAsync(UserType.UserAux.ToString());
        await _userHelper.CheckRoleAsync(UserType.Cachier.ToString());
        await _userHelper.CheckRoleAsync(UserType.Storage.ToString());
    }

    private async Task<User> CheckUserAsync(string firstName, string lastName, string email,
                string phone, UserType userType)
    {
        User user = await _userHelper.GetUserAsync(email);
        if (user == null)
        {
            user = new()
            {
                FirstName = firstName,
                LastName = lastName,
                FullName = $"{firstName} {lastName}",
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                JobPosition = "Administrador",
                UserFrom = "SeedDb",
                UserRoleDetails = new List<UserRoleDetails> { new UserRoleDetails { UserType = userType } },
                Activo = true,
            };

            await _userHelper.AddUserAsync(user, "123456");
            await _userHelper.AddUserToRoleAsync(user, userType.ToString());

            //Para Confirmar automaticamente el Usuario y activar la cuenta
            string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, token);
            await _userHelper.AddUserClaims(userType, email);
        }
        return user;
    }

    private async Task CheckSoftPlan()
    {
        if (!_context.SoftPlans.Any())
        {
            //Tipos de planes de venta del sistema
            _context.SoftPlans.Add(new SoftPlan
            {
                Name = "Plan 1 mes",
                Price = 30,
                Meses = 6,
                ProductCount = 20,
                Active = true
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckCountries()
    {
        if (!_context.Countries.Any())
        {
            _context.Countries.Add(new Country
            {
                Name = "Colombia",
                CodPhone = "+57",
                States = new List<State>()
            {
                new State()
                {
                    Name = "Antioquia",
                    Cities = new List<City>() {
                        new City() { Name = "Medellín" },
                        new City() { Name = "Itagüí" },
                        new City() { Name = "Envigado" },
                        new City() { Name = "Bello" },
                        new City() { Name = "Rionegro" },
                    }
                },
                new State()
                {
                    Name = "Bogotá",
                    Cities = new List<City>() {
                        new City() { Name = "Usaquen" },
                        new City() { Name = "Champinero" },
                        new City() { Name = "Santa fe" },
                        new City() { Name = "Useme" },
                        new City() { Name = "Bosa" },
                    }
                },
                new State()
                {
                    Name = "Arauca",
                    Cities = new List<City>() {
                        new City() { Name = "Arauca 1" },
                        new City() { Name = "Arauca 2" },
                        new City() { Name = "Arauca 3" },
                        new City() { Name = "Arauca 4" },
                        new City() { Name = "Arauca 5" },
                    }
                },
                new State()
                {
                    Name = "Boyacá",
                    Cities = new List<City>() {
                        new City() { Name = "Boyaca 1" },
                        new City() { Name = "Boyaca 2" },
                        new City() { Name = "Boyaca 3" },
                        new City() { Name = "Boyaca 4" },
                        new City() { Name = "Boyaca 5" },
                    }
                },
                new State()
                {
                    Name = "Chocó",
                    Cities = new List<City>() {
                        new City() { Name = "Choco 1" },
                        new City() { Name = "Choco 2" },
                        new City() { Name = "Choco 3" },
                        new City() { Name = "Choco 4" },
                        new City() { Name = "Choco 5" },
                    }
                }
            }
            });
            await _context.SaveChangesAsync();
        }
    }
}