using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.DTOs;
using WebApplication1.Interface;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Tests.Controller
{
    public class OrdersControllerTests
    {
        private readonly Mock<ILogger<OrdersController>> _mockLogger; 
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _controller = new OrdersController(_mockOrderService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateOrder_ValidOrderDto_ReturnsCreatedAtAction()
        {
           
            var orderDto = new OrderDto
            {
                Id = 1,
                CustomerName = "Thanh",
                TotalAmount = 100.00m,
                Status = 0
            };
            _mockOrderService.Setup(s => s.CreateOrderAsync(It.IsAny<OrderDto>())).ReturnsAsync(orderDto);

          
            var result = await _controller.CreateOrder(orderDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetOrder), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);
            Assert.Equal(orderDto, createdResult.Value);
        }

        [Fact]
        public async Task CreateOrder_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("CustomerName", "Required");
            var result = await _controller.CreateOrder(new OrderDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetOrders_ReturnsPagedResult()
        {
            var pagedResult = new PagedResultDto<OrderDto>(
                new List<OrderDto>
                {
                    new OrderDto { Id = 1, CustomerName = "Thanh", TotalAmount = 100.00m, Status = 0 },
                    new OrderDto { Id = 2, CustomerName = "Nam", TotalAmount = 200.00m, Status = 1 }
                },
                page: 1,
                pageSize: 2,
                totalItems: 5
            );
            _mockOrderService.Setup(s => s.GetOrdersAsync(1, 2)).ReturnsAsync(pagedResult);
            var result = await _controller.GetOrders(1, 2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPagedResult = Assert.IsType<PagedResultDto<OrderDto>>(okResult.Value);
            Assert.Equal(2, returnedPagedResult.Items.Count);
            Assert.Equal(1, returnedPagedResult.Page);
            Assert.Equal(2, returnedPagedResult.PageSize);
            Assert.Equal(5, returnedPagedResult.TotalItems);
            Assert.Equal(3, returnedPagedResult.TotalPages);
        }

        [Fact]
        public async Task GetOrder_ExistingId_ReturnsOk()
        {
            var orderDto = new OrderDto { Id = 1, CustomerName = "Thanh", TotalAmount = 100.00m, Status = 0 };
            _mockOrderService.Setup(s => s.GetOrderAsync(1)).ReturnsAsync(orderDto);
            var result = await _controller.GetOrder(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderDto, okResult.Value);
        }

        [Fact]
        public async Task GetOrder_NonExistingId_ReturnsNotFound()
        {
            _mockOrderService.Setup(s => s.GetOrderAsync(1)).ReturnsAsync((OrderDto)null);

            var result = await _controller.GetOrder(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateOrder_ValidOrderDto_ReturnsNoContent()
        {
            var orderDto = new OrderDto { Id = 1, CustomerName = "Thanh", TotalAmount = 100.00m, Status = 0 };
            _mockOrderService.Setup(s => s.UpdateOrderAsync(1, orderDto)).ReturnsAsync(true);

            var result = await _controller.UpdateOrder(1, orderDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateOrder_NonExistingId_ReturnsNotFound()
        {
            var orderDto = new OrderDto { Id = 1, CustomerName = "Thanh", TotalAmount = 100.00m, Status = 0 };
            _mockOrderService.Setup(s => s.UpdateOrderAsync(1, orderDto)).ReturnsAsync(false);

            var result = await _controller.UpdateOrder(1, orderDto);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateOrder_InvalidModelState_ReturnsBadRequest()
        {            
            _controller.ModelState.AddModelError("CustomerName", "Required");
            var result = await _controller.UpdateOrder(1, new OrderDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ExistingId_ReturnsNoContent()        {

            
            _mockOrderService.Setup(s => s.DeleteOrderAsync(1)).ReturnsAsync(true);
            var result = await _controller.DeleteOrder(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_NonExistingId_ReturnsNotFound()
        {            
            _mockOrderService.Setup(s => s.DeleteOrderAsync(1)).ReturnsAsync(false);
            var result = await _controller.DeleteOrder(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddOrderDetail_ValidDetailDto_ReturnsCreatedAtAction()
        {
            
            var detailDto = new OrderDetailDto { Id = 1, OrderId = 1, ProductName = "Product 1", Quantity = 2, Price = 50.00m };
            _mockOrderService.Setup(s => s.AddOrderDetailAsync(1, detailDto)).ReturnsAsync(detailDto);
            var result = await _controller.AddOrderDetail(1, detailDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetOrder), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);
            Assert.Equal(detailDto, createdResult.Value);
        }

        [Fact]
        public async Task AddOrderDetail_NonExistingOrderId_ReturnsNotFound()
        {
            
            var detailDto = new OrderDetailDto { Id = 1, OrderId = 1, ProductName = "Product 1", Quantity = 2, Price = 50.00m };
            _mockOrderService.Setup(s => s.AddOrderDetailAsync(1, detailDto)).ReturnsAsync((OrderDetailDto)null);
            var result = await _controller.AddOrderDetail(1, detailDto);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsOk()
        {
            
            var details = new List<OrderDetailDto>
            {
                new OrderDetailDto { Id = 1, OrderId = 1, ProductName = "Product 1", Quantity = 2, Price = 50.00m },
                new OrderDetailDto { Id = 2, OrderId = 1, ProductName = "Product 2", Quantity = 1, Price = 30.00m }
            };
            _mockOrderService.Setup(s => s.GetOrderDetailsAsync(1)).ReturnsAsync(details);
            var result = await _controller.GetOrderDetails(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDetails = Assert.IsType<List<OrderDetailDto>>(okResult.Value);
            Assert.Equal(2, returnedDetails.Count);
        }

        [Fact]
        public async Task DeleteOrderDetail_ExistingId_ReturnsNoContent()
        {
            
            _mockOrderService.Setup(s => s.DeleteOrderDetailAsync(1)).ReturnsAsync(true);
            var result = await _controller.DeleteOrderDetail(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrderDetail_NonExistingId_ReturnsNotFound()
        {            
            _mockOrderService.Setup(s => s.DeleteOrderDetailAsync(1)).ReturnsAsync(false);
            var result = await _controller.DeleteOrderDetail(1);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
