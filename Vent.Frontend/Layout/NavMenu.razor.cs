using Microsoft.AspNetCore.Components;

namespace Vent.Frontend.Layout;

public partial class NavMenu
{
    [Inject] private NavigationManager _navigation { get; set; } = null!;
    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }

    private void BtnPais()
    {
        _navigation.NavigateTo("/countries");
    }
}