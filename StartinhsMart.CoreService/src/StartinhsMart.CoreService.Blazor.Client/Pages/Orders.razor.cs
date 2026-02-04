using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using StartinhsMart.CoreService.Orders;
using StartinhsMart.CoreService.Permissions;
using StartinhsMart.CoreService.Pets;
using Microsoft.AspNetCore.Components;

namespace StartinhsMart.CoreService.Blazor.Client.Pages
{
    public partial class Orders
    {
        [Inject]
        protected IPetsAppService PetsAppService { get; set; }

        private IReadOnlyList<OrderDto> OrderList { get; set; } = new List<OrderDto>();
        private IReadOnlyList<PetDto> AvailablePets { get; set; } = new List<PetDto>();
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = "CreationTime DESC";
        private int TotalCount { get; set; }
        private bool CanUpdateStatus { get; set; }
        private bool CanCancelOrder { get; set; }
        private bool CanCreateOrder { get; set; }
        private GetOrdersInput Filter { get; set; }
        private DataGrid<OrderDto> OrdersDataGrid { get; set; } = new();
        private bool ShowAdvancedFilters { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();

        // View Details Modal
        private Modal ViewDetailsModal { get; set; } = new();
        private OrderDto? ViewingOrder { get; set; }

        // Update Status Modal
        private Modal UpdateStatusModal { get; set; } = new();
        private Guid UpdatingOrderId { get; set; }
        private string UpdatingOrderNumber { get; set; } = string.Empty;
        private OrderStatus NewStatus { get; set; }

        // Cancel Order Modal
        private Modal CancelOrderModal { get; set; } = new();
        private Guid CancellingOrderId { get; set; }
        private string CancellingOrderNumber { get; set; } = string.Empty;
        private string CancelReason { get; set; } = string.Empty;

        // Create Order Modal
        private Modal CreateOrderModal { get; set; } = new();
        private CreateOrderDto NewOrder { get; set; }
        private Validations NewOrderValidations { get; set; } = new();

        public Orders()
        {
            Filter = new GetOrdersInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            NewOrder = new CreateOrderDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await GetOrdersAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Orders"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["NewOrder"], async () =>
            {
                await OpenCreateOrderModalAsync();
            }, IconName.Add, requiredPolicyName: CoreServicePermissions.Orders.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateOrder = await AuthorizationService.IsGrantedAsync(CoreServicePermissions.Orders.Create);
            CanUpdateStatus = await AuthorizationService.IsGrantedAsync(CoreServicePermissions.Orders.UpdateStatus);
            CanCancelOrder = await AuthorizationService.IsGrantedAsync(CoreServicePermissions.Orders.Cancel);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetOrdersAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task GetOrdersAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await OrdersAppService.GetListAsync(Filter);
            OrderList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        // View Details
        private async Task ViewOrderDetailsAsync(OrderDto order)
        {
            ViewingOrder = await OrdersAppService.GetAsync(order.Id);
            await ViewDetailsModal.Show();
        }

        private async Task CloseViewDetailsModal()
        {
            ViewingOrder = null;
            await ViewDetailsModal.Hide();
        }

        // Update Status
        private async Task OpenUpdateStatusModal(OrderDto order)
        {
            UpdatingOrderId = order.Id;
            UpdatingOrderNumber = order.OrderNumber;
            NewStatus = order.Status;
            await UpdateStatusModal.Show();
        }

        private async Task CloseUpdateStatusModal()
        {
            UpdatingOrderId = Guid.Empty;
            UpdatingOrderNumber = string.Empty;
            await UpdateStatusModal.Hide();
        }

        private async Task UpdateOrderStatusAsync()
        {
            try
            {
                await OrdersAppService.UpdateStatusAsync(UpdatingOrderId, NewStatus);
                await UiMessageService.Success(L["OrderStatusUpdated"]);
                await CloseUpdateStatusModal();
                await GetOrdersAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error(ex.Message);
            }
        }

        // Cancel Order
        private async Task OpenCancelOrderModal(OrderDto order)
        {
            CancellingOrderId = order.Id;
            CancellingOrderNumber = order.OrderNumber;
            CancelReason = string.Empty;
            await CancelOrderModal.Show();
        }

        private async Task CloseCancelOrderModal()
        {
            CancellingOrderId = Guid.Empty;
            CancellingOrderNumber = string.Empty;
            CancelReason = string.Empty;
            await CancelOrderModal.Hide();
        }

        private async Task CancelOrderAsync()
        {
            if (string.IsNullOrWhiteSpace(CancelReason))
            {
                await UiMessageService.Warn(L["PleaseEnterCancelReason"]);
                return;
            }

            try
            {
                await OrdersAppService.CancelOrderAsync(CancellingOrderId, CancelReason);
                await UiMessageService.Success(L["OrderCancelled"]);
                await CloseCancelOrderModal();
                await GetOrdersAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error(ex.Message);
            }
        }

        // Helper methods for colors
        private Color GetStatusColor(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => Color.Warning,
                OrderStatus.Confirmed => Color.Info,
                OrderStatus.Processing => Color.Primary,
                OrderStatus.Shipped => Color.Link,
                OrderStatus.Delivered => Color.Success,
                OrderStatus.Cancelled => Color.Danger,
                OrderStatus.Refunded => Color.Secondary,
                _ => Color.Dark
            };
        }

        private Color GetPaymentStatusColor(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => Color.Warning,
                PaymentStatus.Completed => Color.Success,
                PaymentStatus.Failed => Color.Danger,
                PaymentStatus.Refunded => Color.Secondary,
                _ => Color.Dark
            };
        }

        // Create Order
        private async Task OpenCreateOrderModalAsync()
        {
            NewOrder = new CreateOrderDto();
            // Add one empty item by default
            NewOrder.Items.Add(new CreateOrderItemDto());
            
            // Load available pets
            await LoadAvailablePetsAsync();
            
            await NewOrderValidations.ClearAll();
            await CreateOrderModal.Show();
        }

        private async Task LoadAvailablePetsAsync()
        {
            var result = await PetsAppService.GetListAsync(new GetPetsInput
            {
                MaxResultCount = 1000,
                IsStock = true // Only show pets in stock
            });
            AvailablePets = result.Items;
        }

        private async Task CloseCreateOrderModalAsync()
        {
            await CreateOrderModal.Hide();
        }

        private async Task CreateOrderAsync()
        {
            try
            {
                if (!await NewOrderValidations.ValidateAll())
                {
                    return;
                }

                if (NewOrder.Items == null || !NewOrder.Items.Any())
                {
                    await UiMessageService.Warn(L["PleaseAddAtLeastOneItem"]);
                    return;
                }

                await OrdersAppService.CreateAsync(NewOrder);
                await UiMessageService.Success(L["OrderCreated"]);
                await CloseCreateOrderModalAsync();
                await GetOrdersAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error(ex.Message);
            }
        }

        private void AddOrderItem()
        {
            NewOrder.Items.Add(new CreateOrderItemDto());
        }

        private void RemoveOrderItem(int index)
        {
            if (NewOrder.Items.Count > 1)
            {
                NewOrder.Items.RemoveAt(index);
            }
        }
    }
}
