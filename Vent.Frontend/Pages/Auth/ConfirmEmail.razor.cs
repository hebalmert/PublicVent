using Microsoft.AspNetCore.Components;
using MudBlazor;
using Vent.Frontend.Repositories;

namespace Vent.Frontend.Pages.Auth;

public partial class ConfirmEmail
{
    private string? message;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

    protected async Task ConfirmAccountAsync()
    {
        var responseHttp = await Repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");
        if (responseHttp.Error)
        {
            message = await responseHttp.GetErrorMessageAsync();
            NavigationManager.NavigateTo("/");
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Snackbar.Add("Su Cuenta ha sido Confirmada", Severity.Success);
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<Login>("Iniciar Sesion", closeOnEscapeKey);
    }
}