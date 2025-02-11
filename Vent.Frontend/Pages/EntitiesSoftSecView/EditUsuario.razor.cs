using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class EditUsuario
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private Usuario? Usuario;

    private FormUsuario? FormUsuario { get; set; }

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<Usuario>($"/api/usuarios/{Id}");

        if (responseHTTP.Error)
        {
            if (responseHTTP.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _sweetAlert.FireAsync("Error", "Registro No Encontrado", SweetAlertIcon.Error);
            }
            else
            {
                var messageError = await responseHTTP.GetErrorMessageAsync();
                await _sweetAlert.FireAsync("Error", messageError, SweetAlertIcon.Error);
            }
            _navigationManager.NavigateTo("/usuarios");
        }
        else
        {
            Usuario = responseHTTP.Response;
        }
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync("/api/usuarios", Usuario);

        if (responseHTTP.Error)
        {
            var mensajeError = await responseHTTP.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
        }
        else
        {
            FormUsuario!.FormPostedSuccessfully = true;
            _navigationManager.NavigateTo("/usuarios");
        }
    }

    private void Return()
    {
        FormUsuario!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/usuarios");
    }
}