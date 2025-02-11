using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.Frontend.Repositories;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.CorporationView;

public partial class DetailsCorporation
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;

    private Corporation? Corporation;
    private SoftPlan? SoftPlan;
    private Country? Country;

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
        Corporation = responseHTTP.Response;
        var responseHTTP2 = await _repository.GetAsync<SoftPlan>($"/api/softplans/{Corporation!.SoftPlanId}");
        if (responseHTTP2.Error)
        {
            if (responseHTTP2.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _sweetAlert.FireAsync("Error", "Plan No Encontrado", SweetAlertIcon.Error);
            }
            else
            {
                var messageError = await responseHTTP.GetErrorMessageAsync();
                await _sweetAlert.FireAsync("Error", messageError, SweetAlertIcon.Error);
            }
            _navigationManager.NavigateTo("/corporations");
        }
        SoftPlan = responseHTTP2.Response;
        var responseHTT3 = await _repository.GetAsync<Country>($"/api/countries/{Corporation.CountryId}");
        if (responseHTT3.Error)
        {
            if (responseHTTP2.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _sweetAlert.FireAsync("Error", "Plan No Encontrado", SweetAlertIcon.Error);
            }
            else
            {
                var messageError = await responseHTTP.GetErrorMessageAsync();
                await _sweetAlert.FireAsync("Error", messageError, SweetAlertIcon.Error);
            }
            _navigationManager.NavigateTo("/corporations");
        }
        Country = responseHTT3.Response;
    }

    private string GetDisplayName<T>(Expression<Func<T>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as PropertyInfo;
            if (property != null)
            {
                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                {
                    return displayAttribute.Name!;
                }
            }
        }
        return "Texto no definido";
    }
}