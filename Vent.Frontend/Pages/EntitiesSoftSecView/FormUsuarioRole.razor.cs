using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.Frontend.Repositories;
using Vent.Shared.EntitiesSoftSec;
using Vent.Shared.Enum;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class FormUsuarioRole
{
    private EditContext _editContext = null!;
    private EnumItemModel? SelectedUserType;
    private List<EnumItemModel>? ListUserType;

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    [Parameter, EditorRequired] public UsuarioRole UsuarioRole { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(UsuarioRole);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadRoles();
    }

    private async Task LoadRoles()
    {
        var responseHTTP = await _repository.GetAsync<List<EnumItemModel>>($"api/usuarioRoles/loadCombo");
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
            ListUserType = responseHTTP.Response;
        }
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = _editContext.IsModified();

        if (!formWasEdited)
        {
            return;
        }

        if (FormPostedSuccessfully)
        {
            return;
        }

        var result = await _sweetAlert.FireAsync(new SweetAlertOptions
        {
            Title = "Confirmación",
            Text = "¿Deseas abandonar la página y perder los cambios?",
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true
        });

        var confirm = !string.IsNullOrEmpty(result.Value);

        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private void UsertTypeChanged(EnumItemModel modelo)
    {
        if (modelo.Name == "User") { UsuarioRole.UserType = UserType.User; }
        if (modelo.Name == "UserAux") { UsuarioRole.UserType = UserType.UserAux; }
        if (modelo.Name == "Cachier") { UsuarioRole.UserType = UserType.Cachier; }
        if (modelo.Name == "Storage") { UsuarioRole.UserType = UserType.Storage; }
        SelectedUserType = modelo;
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