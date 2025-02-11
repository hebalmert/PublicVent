using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.ManagerView;

public partial class EditManager
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private Manager? Manager;

    private FormManager? FormManager { get; set; }

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<Manager>($"/api/managers/{Id}");
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
            _navigationManager.NavigateTo("/managers");
        }
        Manager = responseHTTP.Response;
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync("/api/managers", Manager);
        if (responseHTTP.Error)
        {
            var mensajeError = await responseHTTP.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
        }
        else
        {
            FormManager!.FormPostedSuccessfully = true;
            _navigationManager.NavigateTo("/managers");
        }
    }

    private void Return()
    {
        FormManager!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/managers");
    }
}