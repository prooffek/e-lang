using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Services;
using E_Lang.Domain.Entities;
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
    [DataRow(true, 0)]
    [DataRow(false, 0)]
    [DataRow(true, 1)]
    [DataRow(false, 1)]
    [DataRow(true, 2)]
    [DataRow(false, 2)]
    [DataRow(true, 3)]
    [DataRow(false, 3)]
    public async Task CollectionService_ValidateUserCollectionId_ShouldThrowIfCollectionIdNullOrEmpty(bool isEmpty, int actionType)
    {
        // Arrange
        Guid? collectionId = isEmpty ? Guid.Empty : null;

        var collectionService = new CollectionService(_mockCollectionRepository.Object);

        var expectedException = new NullOrEmptyValidationException(nameof(Collection), nameof(Collection.Id), (ActionTypes)actionType);
        
        // Act
        var exception = await Assert.ThrowsExceptionAsync<NullOrEmptyValidationException>(async () =>
            await collectionService.ValidateUserCollectionId(Guid.NewGuid(), collectionId, (ActionTypes)actionType, default));

        // Assert
        exception.Should().NotBeNull();
        exception.EntityName.Should().Be(nameof(Collection));
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
        exception.PropertyName.Should().Be(nameof(Collection.Id));
        exception.ActionType.Should().Be((ActionTypes)actionType);
    }
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public async Task CollectionService_ValidateUserCollectionId_ShouldThrowIfCollectionOwnedByAnotherUser(int actionType)
    {
        // Arrange
        _mockCollectionRepository.Setup(m => 
            m.IsUserCollectionOwnerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default))
            .Returns(Task.FromResult(false));
        
        var collectionService = new CollectionService(_mockCollectionRepository.Object);
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();

        var expectedException = new NotFoundValidationException(nameof(Collection), nameof(Collection.Id), collectionId.ToString());

        // Act
        var exception = await Assert.ThrowsExceptionAsync<NotFoundValidationException>(async () =>
            await collectionService.ValidateUserCollectionId(userId, collectionId, (ActionTypes)actionType, default));

        // Assert
        exception.Should().NotBeNull();
        exception.EntityName.Should().Be(nameof(Collection));
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
        exception.AttributeName.Should().Be(nameof(Collection.Id));
        exception.Value.Should().Be(expectedException.Value);
    }
}