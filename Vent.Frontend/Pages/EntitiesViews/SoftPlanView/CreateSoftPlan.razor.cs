using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.SoftPlanView;

public partial class CreateSoftPlan
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private SoftPlan _softPlan = new();

    private FormSoftPlan? FormSoftplan { get; set; }

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync("/api/softplans", _softPlan);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        FormSoftplan!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/softplans");
    }

    private void Return()
    {
        FormSoftplan!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/softplans");
    }
}