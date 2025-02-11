using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class CreateUsuario
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private Usuario Usuario = new();

    private FormUsuario? FormUsuario { get; set; }

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync("/api/usuarios", Usuario);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        FormUsuario!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/usuarios");
    }

    private void Return()
    {
        FormUsuario!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/usuarios");
    }
}