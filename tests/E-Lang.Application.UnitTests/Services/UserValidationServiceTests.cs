using System.Linq.Expressions;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Services;
using E_Lang.Domain.Entities;
using FluentAssertions;
using Moq;

namespace E_Lang.Application.UnitTests.Services;

[TestClass]
public class UserValidationServiceTests : Setup
{
    private static Mock<IUserRepository> _mockUserRepository;
    
    [ClassInitialize]
    public static void InitializeClass(TestContext testContext)
    {
        InitClass();
        _mockUserRepository = new Mock<IUserRepository>();
    
    }

    [TestInitialize]
    public void InitializeTest()
    {
        InitTest();
    }
    
    [TestCleanup]
    public void CleanUp()
    {
        CleanupTest();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task UserService_ValidateUserId_ShouldThrowIfIdNullOrEmpty(bool isEmpty)
    {
        // Arrange
        Guid? userId = isEmpty ? Guid.Empty : null;

        var userService = new UserValidationService(_mockUserRepository.Object);

        var expectedErrorMessage =
            $"User not found. (Parameter '{nameof(User.Id)}')";
        
        // Act
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            userService.ValidateUserId(userId));
        
        // Assert
        exception.Should().NotBeNull();
        exception.ParamName.Should().Be(nameof(User.Id));
        exception.Message.Should().Be(expectedErrorMessage);
    }
}