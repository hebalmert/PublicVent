using Microsoft.AspNetCore.Components;
using MudBlazor;
using Vent.Frontend.AuthenticationProviders;
using Vent.Frontend.Repositories;
using Vent.Shared.ResponsesSec;

namespace Vent.Frontend.Pages.Auth;

public partial class Login
{
    private LoginDTO _loginDTO = new();
    private bool wasClose;

    [Inject] private NavigationManager _navigation { get; set; } = null!;
    [Inject] private IDialogService _dialogService { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private ILoginService _loginService { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private void CloseModal()
    {
        wasClose = true;
        MudDialog.Cancel();
    }

    private async Task LoginAsync()
    {
        if (wasClose)
        {
            _navigation.NavigateTo("/");
            return;
        }

        var responseHttp = await _repository.PostAsync<LoginDTO, TokenDTO>("/api/accounts/Login", _loginDTO);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            _snackbar.Add(message!, Severity.Error);
            return;
        }

        await _loginService.LoginAsync(responseHttp.Response!.Token);
        _navigation.NavigateTo("/");
    }

    private void ShowModalRecoverPassword()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraLarge };
        _dialogService.Show<RecoverPassword>("Recuperar su Clave", closeOnEscapeKey);
    }
}