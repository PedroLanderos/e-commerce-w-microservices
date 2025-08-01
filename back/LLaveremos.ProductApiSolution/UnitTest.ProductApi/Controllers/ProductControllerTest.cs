using FakeItEasy;
using FluentAssertions;
using Llaveremos.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ProductApi.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productsController;

        public ProductControllerTest()
        {
            productInterface = A.Fake<IProduct>();
            productsController = new ProductsController(productInterface);
        }

        [Fact]
        public async Task GetProducts200()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Product 1", Quantity = 10, Price = 100 },
                new() { Id = 2, Name = "Product 2", Quantity = 5, Price = 50 }
            };
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetProducts404()
        {
            // Arrange
            A.CallTo(() => productInterface.GetAllAsync()).Returns(new List<Product>());

            // Act
            var result = await productsController.GetProducts();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult?.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetProductById200()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Quantity = 10, Price = 100 };
            A.CallTo(() => productInterface.FindByIdAsync(1)).Returns(product);

            // Act
            var result = await productsController.GetProduct(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetProductById404()
        {
            // Arrange
            A.CallTo(() => productInterface.FindByIdAsync(1)).Returns(Task.FromResult<Product>(null));


            // Act
            var result = await productsController.GetProduct(1);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult?.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateProduct200()
        {
            // Arrange
            var productDTO = new ProductDTO(0, "New Product", 10, 100);
            var response = new Response(true, "Product created successfully");
            A.CallTo(() => productInterface.CreateAsync(A<Product>._)).Returns(response);

            // Act
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task CreateProduct400()
        {
            // Arrange
            var productDTO = new ProductDTO(0, "", -1, -100); // Invalid data
            productsController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await productsController.CreateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateProduct200()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Updated Product", 15, 150);
            var response = new Response(true, "Product updated successfully");
            A.CallTo(() => productInterface.UpdateAsync(A<Product>._)).Returns(response);

            // Act
            var result = await productsController.UpdateProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteProduct200()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Deleted Product", 10, 100);
            var response = new Response(true, "Product deleted successfully");
            A.CallTo(() => productInterface.DeleteAsync(A<Product>._)).Returns(response);

            // Act
            var result = await productsController.DeleteProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteProduct400()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Failed Product", 10, 100);
            var response = new Response(false, "Failed to delete product");
            A.CallTo(() => productInterface.DeleteAsync(A<Product>._)).Returns(response);

            // Act
            var result = await productsController.DeleteProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
        }
    }
}