using API.Dtos;
using API.Errors;
using AutoMapper;
using Domain.Entities.OrderAggregate;
using Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers;

[Tags("Order")]
public class OrderController : ControllerBase
{
    public static void AddRoutes(WebApplication app)
    {
        app.MapPost("/api/Order", async (
            [FromServices] IOrderService _orderService,
            [FromServices] IMapper _mapper,
            OrderDto orderDto,
            string email) =>
        {
            var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

            var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

            if (order is null)
            {
                return Results.BadRequest(new ApiResponse(400, "Problem creating order"));
            }

            return Results.Ok(order);
        }).WithTags("Order");

        app.MapGet("/api/OrderForUser", async (
            [FromServices] IOrderService _orderService,
            [FromServices] IMapper _mapper,
            string email) =>
        {
            var orders = await _orderService.GetOrdersByUserEmailAsync(email);

            var ordersToReturn = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);

            return Results.Ok(ordersToReturn);
        }).WithTags("Order");

        app.MapGet("/api/OrderByIdForUser/{id}", async (
            int id,
            [FromServices] IOrderService _orderService,
            [FromServices] IMapper _mapper,
            string email) =>
        {
            var orders = await _orderService.GetOrdersByUserEmailAsync(email);

            var ordersToReturn = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);

            return Results.Ok(ordersToReturn);
        }).WithTags("Order");

        app.MapGet("/api/deliveryMethods", async ([FromServices] IOrderService _orderService) =>
        {
            var methods = await _orderService.GetDeliveryMethodsAsync();

            return Results.Ok(methods);
        }).WithTags("Order");
    }

}