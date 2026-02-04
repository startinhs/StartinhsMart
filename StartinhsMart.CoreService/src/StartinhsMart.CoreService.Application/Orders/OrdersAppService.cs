using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Entities;
using StartinhsMart.CoreService.Permissions;
using StartinhsMart.CoreService.Pets;

namespace StartinhsMart.CoreService.Orders
{
    [Authorize(CoreServicePermissions.Orders.Default)]
    public class OrdersAppService : CoreServiceAppService, IOrdersAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderManager _orderManager;
        private readonly IRepository<Pet, Guid> _petRepository;

        public OrdersAppService(
            IOrderRepository orderRepository,
            OrderManager orderManager,
            IRepository<Pet, Guid> petRepository)
        {
            _orderRepository = orderRepository;
            _orderManager = orderManager;
            _petRepository = petRepository;
        }

        public virtual async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrdersInput input)
        {
            var totalCount = await _orderRepository.GetCountAsync(
                input.FilterText,
                input.OrderNumber,
                input.UserId,
                input.Status,
                input.PaymentStatus,
                input.MinCreationTime,
                input.MaxCreationTime
            );

            var items = await _orderRepository.GetListAsync(
                input.FilterText,
                input.OrderNumber,
                input.UserId,
                input.Status,
                input.PaymentStatus,
                input.MinCreationTime,
                input.MaxCreationTime,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }

        public virtual async Task<OrderDto> GetAsync(Guid id)
        {
            var order = await _orderRepository.GetWithItemsAsync(id);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), id);
            }

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize(CoreServicePermissions.Orders.Create)]
        public virtual async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            // Validate items
            if (input.Items == null || !input.Items.Any())
            {
                throw new BusinessException(CoreServiceDomainErrorCodes.OrderMustHaveItems);
            }

            // Get current user ID
            var userId = CurrentUser.Id ?? throw new BusinessException(CoreServiceDomainErrorCodes.UserNotAuthenticated);

            // Create order
            var order = await _orderManager.CreateAsync(
                userId,
                input.ShippingAddress,
                input.ShippingPhone,
                input.PaymentMethod,
                input.CustomerName,
                input.CustomerEmail,
                input.Note
            );

            // Add items
            foreach (var itemDto in input.Items)
            {
                var pet = await _petRepository.GetAsync(itemDto.PetId);
                if (pet == null)
                {
                    throw new EntityNotFoundException(typeof(Pet), itemDto.PetId);
                }

                // Check stock availability
                if (pet.Quantity < itemDto.Quantity)
                {
                    throw new BusinessException(CoreServiceDomainErrorCodes.InsufficientStock)
                        .WithData("PetName", pet.Name)
                        .WithData("Available", pet.Quantity)
                        .WithData("Requested", itemDto.Quantity);
                }

                var orderItem = new OrderItem(
                    GuidGenerator.Create(),
                    order.Id,
                    pet.Id,
                    pet.Name,
                    itemDto.Quantity,
                    pet.Price ?? 0
                );

                order.AddItem(orderItem);

                // Update pet stock
                pet.Quantity -= itemDto.Quantity;
                await _petRepository.UpdateAsync(pet);
            }

            // Calculate total
            _orderManager.CalculateOrderTotal(order);

            // Save order
            await _orderRepository.InsertAsync(order);

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize(CoreServicePermissions.Orders.Edit)]
        public virtual async Task<OrderDto> UpdateAsync(Guid id, UpdateOrderDto input)
        {
            var order = await _orderRepository.GetWithItemsAsync(id);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), id);
            }

            if (!string.IsNullOrWhiteSpace(input.ShippingAddress))
            {
                order.ShippingAddress = input.ShippingAddress;
            }

            if (!string.IsNullOrWhiteSpace(input.ShippingPhone))
            {
                order.ShippingPhone = input.ShippingPhone;
            }

            if (!string.IsNullOrWhiteSpace(input.CustomerName))
            {
                order.CustomerName = input.CustomerName;
            }

            if (!string.IsNullOrWhiteSpace(input.CustomerEmail))
            {
                order.CustomerEmail = input.CustomerEmail;
            }

            if (!string.IsNullOrWhiteSpace(input.Note))
            {
                order.Note = input.Note;
            }

            if (input.Status.HasValue)
            {
                _orderManager.ValidateStatusTransition(order.Status, input.Status.Value);
                order.UpdateStatus(input.Status.Value);
            }

            if (input.PaymentStatus.HasValue)
            {
                order.UpdatePaymentStatus(input.PaymentStatus.Value);
            }

            await _orderRepository.UpdateAsync(order);

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize(CoreServicePermissions.Orders.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _orderRepository.DeleteAsync(id);
        }

        [Authorize(CoreServicePermissions.Orders.Edit)]
        public virtual async Task<OrderDto> UpdateStatusAsync(Guid id, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetWithItemsAsync(id);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), id);
            }

            _orderManager.ValidateStatusTransition(order.Status, newStatus);
            order.UpdateStatus(newStatus);

            await _orderRepository.UpdateAsync(order);

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize(CoreServicePermissions.Orders.Edit)]
        public virtual async Task<OrderDto> CancelOrderAsync(Guid id, string reason)
        {
            var order = await _orderRepository.GetWithItemsAsync(id);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), id);
            }

            // Validate can cancel
            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            {
                throw new BusinessException(CoreServiceDomainErrorCodes.CannotCancelOrder)
                    .WithData("Status", order.Status);
            }

            order.Cancel(reason);

            // Restore pet stock
            foreach (var item in order.Items)
            {
                var pet = await _petRepository.FindAsync(item.PetId);
                if (pet != null)
                {
                    pet.Quantity += item.Quantity;
                    await _petRepository.UpdateAsync(pet);
                }
            }

            await _orderRepository.UpdateAsync(order);

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public virtual async Task<PagedResultDto<OrderDto>> GetMyOrdersAsync(PagedAndSortedResultRequestDto input)
        {
            var userId = CurrentUser.Id ?? throw new BusinessException(CoreServiceDomainErrorCodes.UserNotAuthenticated);

            var orders = await _orderRepository.GetUserOrdersAsync(userId);

            var pagedOrders = orders
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            return new PagedResultDto<OrderDto>
            {
                TotalCount = orders.Count,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(pagedOrders)
            };
        }
    }
}
