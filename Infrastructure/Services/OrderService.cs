using Domain.Entities.OrderAggregate;
using Domain.Interfaces;
using Domain.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
    {
        var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);


        var orderItems = new List<OrderItem>();

        var order = new Order(orderItems, buyerEmail, shippingAddress, deliveryMethod, 0, "0");

        _unitOfWork.Repository<Order>().Add(order);

        var result = await _unitOfWork.Complete();

        if (result <= 0) return null;

        return order;
    }


    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
        return await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
    }

    public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
    {
        var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

        return await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByUserEmailAsync(string buyerEmail)
    {
        var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

        return await _unitOfWork.Repository<Order>().GetListAsync(spec);
    }
}
