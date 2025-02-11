using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Repositories;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.CorporationView;

public partial class CreateCorporation
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private Corporation Corporation = new();

    private FormCorporation? FormCorporation { get; set; }

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync("/api/corporations", Corporation);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await _sweetAlert.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }
        FormCorporation!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/corporations");
    }

    private void Return()
    {
        FormCorporation!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/corporations");
    }
}