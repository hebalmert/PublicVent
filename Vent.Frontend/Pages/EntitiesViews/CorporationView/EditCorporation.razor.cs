using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.CorporationView;

public partial class EditCorporation
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private Corporation? Corporation;

    private FormCorporation? FormCorporation { get; set; }

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<Corporation>($"/api/corporations/{Id}");

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
            _navigationManager.NavigateTo("/corporations");
        }
        else
        {
            Corporation = responseHTTP.Response;
        }
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync("/api/corporations", Corporation);

        if (responseHTTP.Error)
        {
            var mensajeError = await responseHTTP.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
        }
        else
        {
            FormCorporation!.FormPostedSuccessfully = true;
            _navigationManager.NavigateTo("/corporations");
        }
    }

    private void Return()
    {
        FormCorporation!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/corporations");
    }
}