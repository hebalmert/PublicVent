using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class CreateUsuarioRole
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private UsuarioRole UsuarioRole = new();

    private FormUsuarioRole? FormUsuarioRole { get; set; }

    [Parameter] public int Id { get; set; }

    private async Task Create()
    {
        UsuarioRole.UsuarioId = Id;
        var responseHttp = await _repository.PostAsync("/api/usuarioRoles", UsuarioRole);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        FormUsuarioRole!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/usuarios/details/{Id}");
    }

    private void Return()
    {
        FormUsuarioRole!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/usuarios/details/{Id}");
    }
}