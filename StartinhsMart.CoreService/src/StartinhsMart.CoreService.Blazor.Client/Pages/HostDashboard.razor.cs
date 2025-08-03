using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Authorization.Permissions;
using Volo.Saas.Host;
using Volo.Saas.Host.Blazor.Pages.Shared.Components.SaasLatestTenantsWidget;
using Volo.Saas.Host.Blazor.Pages.Shared.Components.SaasEditionPercentageWidget;
using Volo.Abp.BlazoriseUI;
using Volo.Abp.Timing;
using System.Linq;

namespace StartinhsMart.CoreService.Blazor.Client.Pages;

public partial class HostDashboard
{
    [Inject]
    public IPermissionChecker PermissionChecker { get; set; } = default!;

    protected List<BreadcrumbItem> BreadcrumbItems = new();

    protected SaasEditionPercentageWidgetComponent? SaasEditionPercentageWidgetComponent;

    protected SaasLatestTenantsWidgetComponent? SaasLatestTenantsWidgetComponent;

    protected DateTime StartDate { get; set; }

    protected DateTime EndDate { get; set; }

    protected bool HasSaasPermission { get; set; }

    protected async override Task OnInitializedAsync()
    {
        StartDate = Clock.Now.AddMonths(-1).Date;
        EndDate = Clock.Now.Date;
        HasSaasPermission = await PermissionChecker.IsGrantedAsync(SaasHostPermissions.Tenants.Default);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    } 

    protected virtual async Task RefreshAsync()
    {
        
        if (HasSaasPermission && SaasEditionPercentageWidgetComponent != null)
        {
            await SaasEditionPercentageWidgetComponent.RefreshAsync();
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Dashboard"]));
        return ValueTask.CompletedTask;
    }

    protected virtual Task OnDatesChangedAsync(IReadOnlyList<DateTime> dates)
    {
        StartDate = dates.Min();
        EndDate = dates.Max();

        return Task.CompletedTask;
    }
}
