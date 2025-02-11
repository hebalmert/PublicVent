using Microsoft.AspNetCore.Components;

namespace Vent.Frontend.Shared;

public partial class FilterComponent
{
    [Parameter] public string FilterValue { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ApplyFilter { get; set; }

    private async Task ClearFilter()
    {
        FilterValue = string.Empty;
        await ApplyFilter.InvokeAsync(FilterValue);
    }

    private async Task OnfilterApply()
    {
        await ApplyFilter.InvokeAsync(FilterValue);
    }
}