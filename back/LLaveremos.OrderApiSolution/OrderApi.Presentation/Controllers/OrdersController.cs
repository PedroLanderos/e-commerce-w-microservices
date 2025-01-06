using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using System.Runtime.CompilerServices;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
                return NotFound("No order found");

            var (_, list) = OrderConversion.FromEntity(null, orders);
            return !list!.Any() ? NotFound() : Ok(list.ToList()); // Materializar como lista si eliges la opción b)
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model is invalid");
            var getEntity = OrderConversion.ToEntity(orderDto);
            var response = await orderInterface.CreateAsync(getEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);

            if (order is null)
                return NotFound(null);

            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model is invalid");

            var order = OrderConversion.ToEntity(orderDto);
            var response = await orderInterface.UpdateAsync(order);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDto orderDto)
        {
            var order = OrderConversion.ToEntity(orderDto);
            var response = await orderInterface.DeleteAsync(order);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("Invalid id");

            var orders = await orderService.GetOrderByClientId(clientId);
            return !orders.Any() ? NotFound(null) : Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Invalid id");

            var orderDetail = await orderService.GetOrderDetails(orderId);
            if (orderDetail == null || orderDetail.OrderId <= 0)
                return NotFound("Order not found");

            return Ok(orderDetail);
        }
    }
}
