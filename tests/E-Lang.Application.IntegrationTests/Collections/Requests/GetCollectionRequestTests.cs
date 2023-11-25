using E_Lang.Application.Collections.Requests;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Domain.Entities;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Collections.Requests;

[TestClass]
public class GetCollectionRequestTests : Setup
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
    public async Task GetCollectionRequestHandler_Handle_ShouldThrowIfIdNullOrEmpty(bool isEmpty)
    {
        // Arrange
        Guid? CollectionId = isEmpty ? Guid.Empty : null;
        
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;
        
        var request = new GetCollectionRequest()
        {
            CollectionId = CollectionId
        };

        var expectedException = new NullOrEmptyValidationException(nameof(Collection), nameof(Collection.Id), ActionTypes.Get);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<NullOrEmptyValidationException>(async () => await _mediator.Send(request));

        // Assert
        exception.Should().NotBeNull();
        exception.EntityName.Should().Be(nameof(Collection));
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
        exception.PropertyName.Should().Be(nameof(Collection.Id));
        exception.ActionType.Should().Be(ActionTypes.Get);
    }
    
    [TestMethod]
    public async Task GetCollectionRequestHandler_Handle_ShouldThrowIfUserNotFound()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .SetUsername("DifferentUser")
                .AddCollection(out var collection)
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = null;
        
        var request = new GetCollectionRequest()
        {
            CollectionId = collection.Id
        };

        var expectedException = new UserNotFoundException();

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<UserNotFoundException>(async () => await _mediator.Send(request));

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
    }
    
    [TestMethod]
    public async Task GetCollectionRequestHandler_Handle_ShouldReturnCollectionOfCurrentUser()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user1)
                .AddCollection(out var collection1)
                    .SetName("collection 1")
                    .Build()
                .Build()
            .AddUser(out var user2)
            .SetUsername("User 2")
                .AddCollection(out var collection2)
                    .SetName("collection 2")
                    .AddSubcollection(out var subcollection1)
                        .SetName("subcollection 2.1")
                        .AddSubcollection(out var subcollection2)
                            .SetName("subcollection 2.1.1")
                            .Build()
                        .Build()
                    .Build()    
                .Build()
            .AddUser(out var user3)
                .SetUsername("User 3")
                .AddCollection(out var collection3)
                    .SetName("collection 3")
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user2;
        
        var request = new GetCollectionRequest()
        {
            CollectionId = subcollection1.Id
        };

        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(subcollection1.Id);
        result.Name.Should().Be(subcollection1.Name);
        result.ParentId.Should().Be(collection2.Id);
        result.ParentName.Should().Be(collection2.Name);
        result.Subcollections.Should().HaveCount(subcollection1.Subcollections.Count);
        result.Subcollections.Should().Contain(x => x.Id == subcollection2.Id);
    }
    
    [TestMethod]
    public async Task GetCollectionRequestHandler_Handle_ShouldThrowIfCollectionOfDifferentUser()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user1)
                .AddCollection(out var collection1)
                    .SetName("collection 1")
                    .Build()
                .Build()
            .AddUser(out var user2)
            .SetUsername("User 2")
                .AddCollection(out var collection2)
                    .SetName("collection 2")
                    .AddSubcollection(out var subcollection1)
                        .SetName("subcollection 2.1")
                        .Build()
                    .Build()    
                .Build()
            .AddUser(out var user3)
                .SetUsername("User 3")
                .AddCollection(out var collection3)
                    .SetName("collection 3")
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user2;
        
        var request = new GetCollectionRequest()
        {
            CollectionId = collection1.Id
        };

        var expectedException = new NotFoundValidationException(nameof(Collection), nameof(Collection.Id), collection1.Id.ToString());
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<NotFoundValidationException>(async () => await _mediator.Send(request));

        // Assert
        exception.Should().NotBeNull();
        exception.EntityName.Should().Be(nameof(Collection));
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
        exception.AttributeName.Should().Be(nameof(Collection.Id));
        exception.Value.Should().Be(expectedException.Value);
    }

    [TestMethod]
    public async Task GetCollectionRequestHandler_Handle_ShouldReturnEmptyListIfNoSubcollectionFound()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .Build()
                .Build()
            .SaveAsync();
        
        MockUserService.CurrentUser = user;
        
        var request = new GetCollectionRequest()
        {
            CollectionId = collection.Id
        };
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Subcollections.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetCollectionRequestHandler_Handle_ShouldReturnCorrectSubcollectionDtos()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .SetName("collection 1")
                    .AddSubcollection(out var subcollection1)
                        .SetName("subcollection 1.1")
                        .Build()
                    .AddSubcollection(out var subcollection2)
                        .SetName("subcollection 1.2")
                        .Build()    
                    .AddSubcollection(out var subcollection3)
                        .SetName("subcollection 1.3")
                        .Build()    
                    .Build()
                .Build()
            .SaveAsync();
        
        MockUserService.CurrentUser = user;
        
        var request = new GetCollectionRequest()
        {
            CollectionId = collection.Id
        };
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Subcollections.Should().HaveCount(3);
        
        var result1 = result.Subcollections.FirstOrDefault(x => x.Id == subcollection1.Id);
        result1.Should().NotBeNull();
        result1.SubcollectionsCount.Should().Be(0);
        result1.FlashcardsCount.Should().Be(0);
        
        var result2 = result.Subcollections.FirstOrDefault(x => x.Id == subcollection2.Id);
        result2.Should().NotBeNull();
        result2.SubcollectionsCount.Should().Be(0);
        result2.FlashcardsCount.Should().Be(0);
        
        var result3 = result.Subcollections.FirstOrDefault(x => x.Id == subcollection3.Id);
        result3.Should().NotBeNull();
        result3.SubcollectionsCount.Should().Be(0);
        result3.FlashcardsCount.Should().Be(0);
    }
    
    [TestMethod]
    public async Task GetCollectionRequestHandler_Handle_ShouldReturnOnlyDirectChildren()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .SetName("collection 1")
                    .AddSubcollection(out var subcollection1)
                        .SetName("subcollection 1")
                        .AddSubcollection(out var subcollection7)
                            .SetName("subcollection 1.1")
                            .AddSubcollection(out var subcollection8)
                                .SetName("subcollection 1.1")
                                .AddSubcollection(out var subcollection9)
                                    .SetName("subcollection 1.1")
                                    .Build()
                                .Build()
                            .Build()
                        .AddSubcollection(out var subcollection5)
                            .SetName("subcollection 1.2")
                            .Build()
                        .AddSubcollection(out var subcollection6)
                            .SetName("subcollection 1.3")
                            .Build()
                        .Build()
                    .AddSubcollection(out var subcollection2)
                        .SetName("subcollection 2")
                        .Build()    
                    .AddSubcollection(out var subcollection3)
                        .SetName("subcollection 3")
                        .Build()    
                    .Build()
                .Build()
            .SaveAsync();
        
        MockUserService.CurrentUser = user;
        
        var request = new GetCollectionRequest()
        {
            CollectionId = collection.Id
        };
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Subcollections.Should().HaveCount(3);
        var resultSubcollection1 = result.Subcollections.FirstOrDefault(x => x.Id == subcollection1.Id);
        var resultSubcollection2 = result.Subcollections.FirstOrDefault(x => x.Id == subcollection2.Id);
        var resultSubcollection3 = result.Subcollections.FirstOrDefault(x => x.Id == subcollection3.Id);

        resultSubcollection1.Should().NotBeNull();
        resultSubcollection1.Id.Should().Be(subcollection1.Id);
        resultSubcollection1.SubcollectionsCount.Should().Be(3);
        resultSubcollection1.FlashcardsCount.Should().Be(0);

        resultSubcollection2.Should().NotBeNull();
        resultSubcollection2.Id.Should().Be(subcollection2.Id);
        resultSubcollection2.SubcollectionsCount.Should().Be(0);
        resultSubcollection2.FlashcardsCount.Should().Be(0);
        
        resultSubcollection3.Should().NotBeNull();
        resultSubcollection3.Id.Should().Be(subcollection3.Id);
        resultSubcollection3.SubcollectionsCount.Should().Be(0);
        resultSubcollection3.FlashcardsCount.Should().Be(0);
    }
}