namespace Vent.Frontend.AuthenticationProviders
{
    public interface ILoginService
    {
        Task LoginAsync(string token);

        Task LogoutAsync();
    }
}