using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Presentation.Controllers;
using Llaveremos.SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using OrderApi.Domain.Entities;
using OrderApi.Application.Services;

namespace UnitTest.OrderApi.Controllers
{
    public class OrdersControllerTest
    {
        private readonly IOrder orderInterface;
        private readonly IOrderService orderService;
        private readonly OrdersController ordersController;

        public OrdersControllerTest()
        {
            orderInterface = A.Fake<IOrder>();
            orderService = A.Fake<IOrderService>();
            ordersController = new OrdersController(orderInterface, orderService);
        }

        #region GetOrders Tests

        [Fact]
        public async Task GetOrders_ReturnsOk_WhenOrdersExist()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, ProductId = 1, ClientId = 1, PurchaseQuantity = 10 },
                new Order { Id = 2, ProductId = 2, ClientId = 1, PurchaseQuantity = 5 }
            };
            A.CallTo(() => orderInterface.GetAllAsync()).Returns(Task.FromResult((IEnumerable<Order>)orders));

            // Act
            var result = await ordersController.GetOrders();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeAssignableTo<IEnumerable<OrderDto>>();
            var returnedOrders = okResult.Value as IEnumerable<OrderDto>;
            returnedOrders.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrders_ReturnsNotFound_WhenNoOrdersExist()
        {
            // Arrange
            A.CallTo(() => orderInterface.GetAllAsync()).Returns(Task.FromResult((IEnumerable<Order>)new List<Order>()));

            // Act
            var result = await ordersController.GetOrders();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult?.StatusCode.Should().Be(404);
            notFoundResult?.Value.Should().Be("No order found");
        }

        #endregion

        #region CreateOrder Tests

        [Fact]
        public async Task CreateOrder_ReturnsOk_WhenOrderIsCreatedSuccessfully()
        {
            // Arrange
            var orderDto = new OrderDto(0, 1, 1, 10, DateTime.Now);
            var response = new Response(true, "Order created successfully");
            A.CallTo(() => orderInterface.CreateAsync(A<Order>._)).Returns(Task.FromResult(response));

            // Act
            var result = await ordersController.CreateOrder(orderDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var orderDto = new OrderDto(0, 0, 0, -1, DateTime.Now); // Datos inválidos
            ordersController.ModelState.AddModelError("ProductId", "Invalid ProductId");
            ordersController.ModelState.AddModelError("ClientId", "Invalid ClientId");
            ordersController.ModelState.AddModelError("PurchaseQuantity", "Invalid PurchaseQuantity");

            // Act
            var result = await ordersController.CreateOrder(orderDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult?.Value.Should().Be("Model is invalid");
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var orderDto = new OrderDto(0, 1, 1, 10, DateTime.Now);
            var response = new Response(false, "Failed to create order");
            A.CallTo(() => orderInterface.CreateAsync(A<Order>._)).Returns(Task.FromResult(response));

            // Act
            var result = await ordersController.CreateOrder(orderDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult?.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region GetOrder Tests

        [Fact]
        public async Task GetOrder_ReturnsOk_WhenOrderExists()
        {
            // Arrange
            var order = new Order { Id = 1, ProductId = 1, ClientId = 1, PurchaseQuantity = 10 };
            A.CallTo(() => orderInterface.FindByIdAsync(1)).Returns(Task.FromResult(order));

            // Act
            var result = await ordersController.GetOrder(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeOfType<OrderDto>();
            var returnedOrder = okResult.Value as OrderDto;
            returnedOrder.Should().NotBeNull();
            returnedOrder?.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            A.CallTo(() => orderInterface.FindByIdAsync(1)).Returns(Task.FromResult<Order>(null));

            // Act
            var result = await ordersController.GetOrder(1);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult?.StatusCode.Should().Be(404);
            notFoundResult?.Value.Should().BeNull();
        }

        #endregion

        #region UpdateOrder Tests

        [Fact]
        public async Task UpdateOrder_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var orderDto = new OrderDto(1, 2, 2, 20, DateTime.Now);
            var response = new Response(true, "Order updated successfully");
            A.CallTo(() => orderInterface.UpdateAsync(A<Order>._)).Returns(Task.FromResult(response));

            // Act
            var result = await ordersController.UpdateOrder(orderDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdateOrder_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var orderDto = new OrderDto(1, 2, 2, 20, DateTime.Now);
            var response = new Response(false, "Failed to update order");
            A.CallTo(() => orderInterface.UpdateAsync(A<Order>._)).Returns(Task.FromResult(response));

            // Act
            var result = await ordersController.UpdateOrder(orderDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult?.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdateOrder_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var orderDto = new OrderDto(1, 0, 0, -5, DateTime.Now); // Datos inválidos
            ordersController.ModelState.AddModelError("ProductId", "Invalid ProductId");
            ordersController.ModelState.AddModelError("ClientId", "Invalid ClientId");
            ordersController.ModelState.AddModelError("PurchaseQuantity", "Invalid PurchaseQuantity");

            // Act
            var result = await ordersController.UpdateOrder(orderDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult?.Value.Should().Be("Model is invalid");
        }

        #endregion

        #region DeleteOrder Tests

        [Fact]
        public async Task DeleteOrder_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var orderDto = new OrderDto(1, 2, 2, 20, DateTime.Now);
            var response = new Response(true, "Order deleted successfully");
            A.CallTo(() => orderInterface.DeleteAsync(A<Order>._)).Returns(Task.FromResult(response));

            // Act
            var result = await ordersController.DeleteOrder(orderDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsBadRequest_WhenDeletionFails()
        {
            // Arrange
            var orderDto = new OrderDto(1, 2, 2, 20, DateTime.Now);
            var response = new Response(false, "Failed to delete order");
            A.CallTo(() => orderInterface.DeleteAsync(A<Order>._)).Returns(Task.FromResult(response));

            // Act
            var result = await ordersController.DeleteOrder(orderDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult?.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region GetClientOrders Tests

        [Fact]
        public async Task GetClientOrders_ReturnsOk_WhenOrdersExistForClient()
        {
            // Arrange
            var clientId = 1;
            var ordersDto = new List<OrderDto>
            {
                new OrderDto(1, 1, 1, 10, DateTime.Now),
                new OrderDto(2, 2, 1, 5, DateTime.Now)
            };
            A.CallTo(() => orderService.GetOrderByClientId(clientId)).Returns(Task.FromResult<IEnumerable<OrderDto>>(ordersDto));

            // Act
            var result = await ordersController.GetClientOrders(clientId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(ordersDto);
        }

        [Fact]
        public async Task GetClientOrders_ReturnsBadRequest_WhenClientIdIsInvalid()
        {
            // Arrange
            var invalidClientId = 0;

            // Act
            var result = await ordersController.GetClientOrders(invalidClientId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult?.Value.Should().Be("Invalid id");
        }

        [Fact]
        public async Task GetClientOrders_ReturnsNotFound_WhenNoOrdersExistForClient()
        {
            // Arrange
            var clientId = 1;
            A.CallTo(() => orderService.GetOrderByClientId(clientId)).Returns(Task.FromResult<IEnumerable<OrderDto>>(new List<OrderDto>()));

            // Act
            var result = await ordersController.GetClientOrders(clientId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult?.StatusCode.Should().Be(404);
            notFoundResult?.Value.Should().BeNull();
        }

        #endregion

        #region GetOrderDetails Tests

        [Fact]
        public async Task GetOrderDetails_ReturnsOk_WhenOrderDetailsExist()
        {
            // Arrange
            var orderId = 1;
            var orderDetailsDto = new OrderDetailsDto(
                OrderId: orderId,
                ProductId: 1,
                ClientId: 1,
                Name: "John Doe",
                Email: "john@example.com",
                Address: "123 Main St",
                TelephoneNumber: "1234567890",
                ProductName: "Product 1",
                PurchaseQuanyity: 10,
                UnitPrice: 100m,
                TotalPrice: 1000m,
                OrderedDate: DateTime.Now
            );
            A.CallTo(() => orderService.GetOrderDetails(orderId)).Returns(Task.FromResult(orderDetailsDto));

            // Act
            var result = await ordersController.GetOrderDetails(orderId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeOfType<OrderDetailsDto>();
            var returnedOrderDetails = okResult.Value as OrderDetailsDto;
            returnedOrderDetails.Should().NotBeNull();
            returnedOrderDetails?.OrderId.Should().Be(orderId);
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsBadRequest_WhenOrderIdIsInvalid()
        {
            // Arrange
            var invalidOrderId = 0;

            // Act
            var result = await ordersController.GetOrderDetails(invalidOrderId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult?.Value.Should().Be("Invalid id");
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsNotFound_WhenOrderDetailsDoNotExist()
        {
            // Arrange
            var orderId = 1;
            A.CallTo(() => orderService.GetOrderDetails(orderId)).Returns(Task.FromResult<OrderDetailsDto>(null));

            // Act
            var result = await ordersController.GetOrderDetails(orderId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult?.StatusCode.Should().Be(404);
            notFoundResult?.Value.Should().Be("Order not found");
        }

        #endregion
    }
}
