
using Microsoft.AspNetCore.Mvc;
using TMB.Challenge.Application.DTOs.Order;
using TMB.Challenge.Application.Handler;
using TMB.Challenge.Domain.Entities;
using TMB.Challenge.Domain.Enum;


namespace TMB.Challenge.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrdersController(OrderHandler orderHandler) : ControllerBase
{
    private readonly OrderHandler _orderHandler = orderHandler;

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderDto)
    {
        var result = await _orderHandler.PostOrder(orderDto);
        if (!result.IsSuccess)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
        var newOrder = result.Value;
        return CreatedAtAction(nameof(GetOrderByUuid), new { uuid = newOrder.Uuid }, newOrder);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] OrderStatus? status)
    {
        var orders = await _orderHandler.GetAllOrdersAsync(pageNumber, pageSize, status);
        return Ok(orders);
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetOrderByUuid(string uuid)
    {
        var order = await _orderHandler.GetOrderByUuidAsync(uuid);
        if (!order.IsSuccess) return NotFound(order.Error);
        return Ok(order.Value);
    }
}