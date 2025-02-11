using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.SoftPlanView;

public partial class EditSoftPlan
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private string BaseUrl = "api/softplans";

    private SoftPlan? SoftPlan;

    private FormSoftPlan? FormSoftPlan { get; set; }

    [Parameter]
    public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<SoftPlan>($"{BaseUrl}/{Id}");
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
            _navigationManager.NavigateTo("/softplans");
        }
        else
        {
            SoftPlan = responseHTTP.Response;
        }
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync($"{BaseUrl}", SoftPlan);

        if (responseHTTP.Error)
        {
            var mensajeError = await responseHTTP.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
        }
        else
        {
            FormSoftPlan!.FormPostedSuccessfully = true;
            _navigationManager.NavigateTo("/softplans");
        }
    }

    private void Return()
    {
        FormSoftPlan!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/softplans");
    }
}