using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Interface;
using Microsoft.Extensions.Logging;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateOrder: {@OrderDto}", orderDto);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new order: {@OrderDto}", orderDto);
            var createdOrder = await _orderService.CreateOrderAsync(orderDto);
            _logger.LogInformation("Order created successfully with ID: {OrderId}", createdOrder.Id);

            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching orders with page: {Page}, pageSize: {PageSize}", page, pageSize);
            var pagedOrders = await _orderService.GetOrdersAsync(page, pageSize);
            _logger.LogInformation("Fetched {OrderCount} orders", pagedOrders.Items.Count);

            return Ok(pagedOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            _logger.LogInformation("Fetching order with ID: {OrderId}", id);
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Order with ID: {OrderId} fetched successfully", id);
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateOrder: {@OrderDto}", orderDto);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating order with ID: {OrderId}, Data: {@OrderDto}", id, orderDto);
            var result = await _orderService.UpdateOrderAsync(id, orderDto);
            if (!result)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for update", id);
                return NotFound();
            }

            _logger.LogInformation("Order with ID: {OrderId} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation("Deleting order with ID: {OrderId}", id);
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Order with ID: {OrderId} deleted successfully", id);
            return NoContent();
        }

        [HttpPost("{id}/order-details")]
        public async Task<IActionResult> AddOrderDetail(int id, [FromBody] OrderDetailDto detailDto)
        {
            _logger.LogInformation("Adding order detail to order ID: {OrderId}, Data: {@OrderDetailDto}", id, detailDto);
            var detail = await _orderService.AddOrderDetailAsync(id, detailDto);
            if (detail == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for adding order detail", id);
                return NotFound();
            }

            _logger.LogInformation("Order detail added successfully to order ID: {OrderId}", id);
            return CreatedAtAction(nameof(GetOrder), new { id }, detail);
        }

        [HttpGet("{id}/order-details")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            _logger.LogInformation("Fetching order details for order ID: {OrderId}", id);
            var details = await _orderService.GetOrderDetailsAsync(id);
            _logger.LogInformation("Fetched {DetailCount} order details for order ID: {OrderId}", details.Count, id);

            return Ok(details);
        }

        [HttpDelete("order-details/{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            _logger.LogInformation("Deleting order detail with ID: {DetailId}", id);
            var result = await _orderService.DeleteOrderDetailAsync(id);
            if (!result)
            {
                _logger.LogWarning("Order detail with ID: {DetailId} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Order detail with ID: {DetailId} deleted successfully", id);
            return NoContent();
        }
    }
}
