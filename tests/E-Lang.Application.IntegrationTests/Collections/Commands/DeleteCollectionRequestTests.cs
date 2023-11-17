using E_Lang.Application.Collections.Commands;
using E_Lang.Domain.Entities;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Collections.Commands;

[TestClass]
public class DeleteCollectionRequestTests : Setup
{
    [ClassInitialize]
    public static void InitializeClass(TestContext context)
    {
        InitClass();
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
    public async Task DeleteCollectionRequest_Handle_ShouldThrowIdIdNullOrEmpty(bool isNull)
    {
        // Arrange
        var request = new DeleteCollectionRequest()
        {
            Id = isNull ? null : Guid.Empty
        };

        var exceptionName = $"Collection id cannot be null or empty. (Parameter '{nameof(Collection.Id)}')";
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _mediator.Send(request));
        
        // Assert
        exception.Should().NotBeNull();
        exception.ParamName.Should().Be(nameof(Collection.Id));
        exception.Message.Should().Be(exceptionName);
    }
    
    [TestMethod]
    public async Task DeleteCollectionRequest_Handle_ShouldThrowIfCollectionNotFound()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();
        
        var request = new DeleteCollectionRequest()
        {
            Id = Guid.NewGuid()
        };

        var exceptionName = $"Collection with id {request.Id.Value.ToString()} not found.";
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _mediator.Send(request));
        
        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(exceptionName);
    }
    
    [TestMethod]
    public async Task DeleteCollectionRequest_Handle_ShouldThrowIfUserDoNotOwnCollection()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .Build()
                    .Build()
                .Build()
            .AddUser(out var user2)
                .SetUsername("CurrentUser")
                .SetEmail("current@test.com")
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user2;
        
        var request = new DeleteCollectionRequest()
        {
            Id = subcollection.Id
        };

        var exceptionName = $"Access denied. User with id ${user2.Id} is not allowed to remove the collection.";
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await _mediator.Send(request));
        
        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(exceptionName);
        }
    
    [TestMethod]
    public async Task DeleteCollectionRequest_Handle_ShouldThrowIfCollectionHasSubcollections()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();
        
        MockUserService.CurrentUser = user;
        
        var request = new DeleteCollectionRequest()
        {
            Id = collection.Id
        };

        var exceptionName = "Cannot delete collection. Delete its subcollections before proceeding.";
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<Exception>(async () => await _mediator.Send(request));
        
        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(exceptionName);
    }
    
    [TestMethod]
    public async Task DeleteCollectionRequest_Handle_ShouldDeleteCollection()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;
        
        var request = new DeleteCollectionRequest()
        {
            Id = subcollection.Id
        };
        
        // Act
        await _mediator.Send(request);
        
        // Assert
        var result = await _collectionRepository.GetByIdAsync(subcollection.Id);
        result.Should().BeNull();
    }
}