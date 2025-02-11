using Microsoft.AspNetCore.Components;
using MudBlazor;
using Vent.Frontend.Repositories;
using Vent.Shared.ResponsesSec;

namespace Vent.Frontend.Pages.Auth;

public partial class ResetPassword
{
    private ResetPasswordDTO resetPasswordDTO = new();
    private bool loading;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    [CascadingParameter] private IMudDialogInstance _mudDialog { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

    private async Task ChangePasswordAsync()
    {
        resetPasswordDTO.Token = Token;
        loading = true;
        var responseHttp = await Repository.PostAsync("/api/accounts/ResetPassword", resetPasswordDTO);
        loading = false;
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Snackbar.Add("Se ha Cambiado con Exito", Severity.Success);
        NavigationManager.NavigateTo("/");
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<Login>("Iniciar Sesion", closeOnEscapeKey);
    }
}