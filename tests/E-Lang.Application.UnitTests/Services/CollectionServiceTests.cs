using E_Lang.Application.Collections.Requests;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Services;
using FluentAssertions;
using Moq;

namespace E_Lang.Application.UnitTests.Services;

[TestClass]
public class CollectionServiceTests : Setup
{
    private static Mock<ICollectionRepository> _mockCollectionRepository;
    
    [ClassInitialize]
    public static void InitializeClass(TestContext testContext)
    {
        InitClass();
        _mockCollectionRepository = new Mock<ICollectionRepository>();
    
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
    public async Task CollectionService_ValidateUserCollectionId_ShouldThrowIfCollectionIdNullOrEmpty(bool isEmpty)
    {
        // Arrange
        Guid? collectionId = isEmpty ? Guid.Empty : null;

        var collectionService = new Mock<CollectionService>(_mockCollectionRepository.Object).Object;
        
        var expectedErrorMessage = $"Parent collection Id cannot be null or empty. (Parameter '{nameof(GetCollectionRequest.CollectionId)}')";
        
        // Act
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            await collectionService.ValidateUserCollectionId(Guid.NewGuid(), collectionId, default));
        
        // Assert
        exception.Should().NotBeNull();
        exception.ParamName.Should().Be(nameof(GetCollectionRequest.CollectionId));
        exception.Message.Should().Be(expectedErrorMessage);
    }
    
    [TestMethod]
    public async Task CollectionService_ValidateUserCollectionId_ShouldThrowIfCollectionOwnedByAnotherUser()
    {
        // Arrange
        _mockCollectionRepository.Setup(m => 
            m.IsUserCollectionOwner(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
            .Returns(Task.FromResult(false));
        
        var collectionService = new Mock<CollectionService>(_mockCollectionRepository.Object).Object;
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var expectedErrorMessage = $"User with Id {userId} does not have collection with Id {collectionId}";
        
        // Act
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await collectionService.ValidateUserCollectionId(userId, collectionId, default));
        
        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedErrorMessage);
    }
}