using FakeItEasy;
using FluentAssertions;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Presentation.Controllers;
using Llaveremos.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.AuthenticationApi.Controllers
{
    public class AuthenticationControllerTest
    {
        private readonly IUser userInterface;
        private readonly AuthenticationController authenticationController;

        public AuthenticationControllerTest()
        {
            userInterface = A.Fake<IUser>();
            authenticationController = new AuthenticationController(userInterface);
        }

        [Fact]
        public async Task Register200()
        {
            // Arrange
            var userDTO = new AppUserDTO(0, "John Doe", "1234567890", "123 Main St", "john@example.com", "password123", "User");
            var response = new Response(true, "User registered successfully");
            A.CallTo(() => userInterface.Register(userDTO)).Returns(Task.FromResult(response));

            // Act
            var result = await authenticationController.Register(userDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Register400()
        {
            // Arrange
            var userDTO = new AppUserDTO(0, "", "1234567890", "123 Main St", "invalid-email", "password123", "User");
            authenticationController.ModelState.AddModelError("Email", "Invalid email address");

            // Act
            var result = await authenticationController.Register(userDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Login200()
        {
            // Arrange
            var loginDTO = new LoginDTO("john@example.com", "password123");
            var response = new Response(true, "Login successful");
            A.CallTo(() => userInterface.Login(loginDTO)).Returns(Task.FromResult(response));

            // Act
            var result = await authenticationController.Login(loginDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Login400()
        {
            // Arrange
            var loginDTO = new LoginDTO("", "");
            authenticationController.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await authenticationController.Login(loginDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetUser200()
        {
            // Arrange
            var getUserDTO = new GetUserDTO(1, "John Doe", "1234567890", "123 Main St", "john@example.com", "User");
            A.CallTo(() => userInterface.GetUser(1)).Returns(Task.FromResult(getUserDTO));

            // Act
            var result = await authenticationController.GetUser(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUser400()
        {
            // Arrange
            // Invalid ID (e.g., less than or equal to 0)
            var invalidId = 0;

            // Act
            var result = await authenticationController.GetUser(invalidId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetUser404()
        {
            // Arrange
            A.CallTo(() => userInterface.GetUser(1)).Returns(Task.FromResult<GetUserDTO>(null));

            // Act
            var result = await authenticationController.GetUser(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult?.StatusCode.Should().Be(404);
        }
    }
}
