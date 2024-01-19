using API.Dtos;
using API.Errors;
using AutoMapper;
using Domain.Entities.OrderAggregate;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

public class OrdersController : BaseApiController
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public OrdersController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto, string email)
    {
        var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

        var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

        if (order is null)
        {
            return BadRequest(new ApiResponse(400, "Problem creating order"));
        }

        return Ok(order);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(string email)
    {
        var orders = await _orderService.GetOrdersByUserEmailAsync(email);

        var ordersToReturn = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);

        return Ok(ordersToReturn);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id, string email)
    {
        var order = await _orderService.GetOrderByIdAsync(id, email);

        if (order is null) return NotFound(new ApiResponse(404));

        var orderToReturn = _mapper.Map<Order, OrderToReturnDto>(order);

        return Ok(orderToReturn);
    }

    [HttpGet("deliveryMethods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        var methods = await _orderService.GetDeliveryMethodsAsync();

        return Ok(methods);
    }
}
