using System.Security.Claims;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using E_Lang.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace E_Lang.Infrastructure.UnitTests.Services;

[TestClass]
public class UserServiceTests : Setup
{
    private static Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private static Mock<IUserRepository> _userRepository;
    
    [ClassInitialize]
    public static void InitializeClass(TestContext testContext)
    {
        InitClass();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _userRepository = new Mock<IUserRepository>();
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
    public async Task UserService_GetCurrentUser_ShouldReturnUserIfCached()
    {
        // Arrange
        var user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = "Admin",
            Email = "test@test.com"
        };
        
        object? userObject = user;

        _mockHttpContextAccessor.Setup(m =>
            m.HttpContext.Items.TryGetValue(It.IsAny<string>(), out userObject ))
            .Returns(true);

        var userService = new UserService(_mockHttpContextAccessor.Object, _userRepository.Object);
        
        // Act
        var result = await userService.GetCurrentUser(CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);
    }

    [TestMethod]
    public async Task UserService_GetCurrentUser_ShouldReturnNullIfNoUserContext()
    {
        // Arrange
        var user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = "Admin",
            Email = "test@test.com"
        };
        
        object? userObject = user;

        _mockHttpContextAccessor.Setup(m =>
                m.HttpContext.Items.TryGetValue(It.IsAny<string>(), out userObject ))
            .Returns(false);

        var userService = new UserService(_mockHttpContextAccessor.Object, _userRepository.Object);
        
        // Act
        var result = await userService.GetCurrentUser(CancellationToken.None);
        
        // Assert
        result.Should().BeNull();
    }
    
    // TODO: Ignore till authentication with token not implemented
    [Ignore]
    [TestMethod]
    public async Task UserService_GetCurrentUser_ShouldReturnNullIfNoUserNameClaim()
    {
        // Arrange
        var user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = "Admin",
            Email = "test@test.com"
        };
        
        object? userObject = user;

        _mockHttpContextAccessor.Setup(m =>
                m.HttpContext.Items.TryGetValue(It.IsAny<string>(), out userObject ))
            .Returns(false);

        _mockHttpContextAccessor.Setup(m =>
            m.HttpContext.User).Returns(new ClaimsPrincipal());

        var userService = new UserService(_mockHttpContextAccessor.Object, _userRepository.Object);
        
        // Act
        var result = await userService.GetCurrentUser(CancellationToken.None);
        
        // Assert
        result.Should().BeNull();
    }
    
    [TestMethod]
    public async Task UserService_GetCurrentUser_ShouldReturnNullIfUserNotFound()
    {
        // Arrange
        User? user = null;
        
        object? userObject = user;
                       
        List<Claim> claims = new()
        {
            new Claim(nameof(User.UserName), "InvalidUsername")
        };

        _mockHttpContextAccessor.Setup(m =>
                m.HttpContext.Items.TryGetValue(It.IsAny<string>(), out userObject ))
            .Returns(false);
        
        _mockHttpContextAccessor.Setup(m =>
            m.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

        _userRepository.Setup(m =>
            m.GetUserByUserName(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((User?)null));

        var userService = new UserService(_mockHttpContextAccessor.Object, _userRepository.Object);
        
        // Act
        var result = await userService.GetCurrentUser(CancellationToken.None);
        
        // Assert
        result.Should().BeNull();
    }
    
    [TestMethod]
    public async Task UserService_GetCurrentUser_ShouldReturnUser()
    {
        // Arrange
        var user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = "Admin",
            Email = "test@test.com"
        };
        
        object? userObject = user;

        List<Claim> claims = new()
        {
            new Claim(nameof(User.UserName), user.UserName)
        };

        _mockHttpContextAccessor.Setup(m =>
                m.HttpContext.Items.TryGetValue(It.IsAny<string>(), out userObject ))
            .Returns(false);
        
        _mockHttpContextAccessor.Setup(m =>
            m.HttpContext.Items.Add(It.IsAny<string>(), It.IsAny<User>()))
            .Verifiable();
        
        _mockHttpContextAccessor.Setup(m =>
            m.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

        _userRepository.Setup(m =>
                m.GetUserByUserName(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(user));

        var userService = new UserService(_mockHttpContextAccessor.Object, _userRepository.Object);
        
        // Act
        var result = await userService.GetCurrentUser(CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);
        
        _mockHttpContextAccessor.Verify(m => 
            m.HttpContext.Items.Add(It.IsAny<string>(), It.IsAny<User>()), Times.Once);
    }
}