using E_Lang.Application.Collections.Requests;
using E_Lang.Application.Common.Errors;
using E_Lang.Domain.Entities;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Collections.Requests;

[TestClass]
public class GetCollectionCardsRequestTests : Setup
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
    public async Task GetUserCollectionsRequestHandler_Handle_ShouldThrowIfUserNotFound()
    {
        // Arrange
        MockUserService.CurrentUser = null;

        var request = new GetCollectionCardsRequest();

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
    public async Task GetUserCollectionsRequestHandler_Handle_ShouldReturnEmptyListIfNoCollectionFound()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new GetCollectionCardsRequest();
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetUserCollectionsRequestHandler_Handle_ShouldReturnCorrectCollectionDtos()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection1)
                    .SetName("collection 1")
                    .Build()
                .AddCollection(out var collection2)
                    .SetName("collection 2")
                    .AddSubcollection(out var subcollection1)
                        .SetName("subcollection 2.1")
                        .Build()
                    .Build()
                .AddCollection(out var collection3)
                    .SetName("collection 3")
                    .AddSubcollection(out var subcollection2)
                        .SetName("subcollection 3.1")
                        .Build()
                    .AddSubcollection(out var subcollection3)
                        .SetName("subcollection 3.2")
                        .Build()    
                    .AddSubcollection(out var subcollection5)
                        .SetName("subcollection 3.3")
                        .Build()    
                    .Build()
                .Build()
            .SaveAsync();
        
        MockUserService.CurrentUser = user;

        var request = new GetCollectionCardsRequest();
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        
        var result1 = result.FirstOrDefault(x => x.Id == collection1.Id);
        result1.Should().NotBeNull();
        result1.SubcollectionsCount.Should().Be(0);
        result1.FlashcardsCount.Should().Be(0);
        
        var result2 = result.FirstOrDefault(x => x.Id == collection2.Id);
        result2.Should().NotBeNull();
        result2.SubcollectionsCount.Should().Be(1);
        result2.FlashcardsCount.Should().Be(0);
        
        var result3 = result.FirstOrDefault(x => x.Id == collection3.Id);
        result3.Should().NotBeNull();
        result3.SubcollectionsCount.Should().Be(3);
        result3.FlashcardsCount.Should().Be(0);
    }
    
    [TestMethod]
    public async Task GetUserCollectionsRequestHandler_Handle_ShouldReturnOnlyParentCollectionDtosIfParentCollectionIdIsNull()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .SetName("collection 1")
                    .AddSubcollection(out var subcollection1)
                        .SetName("subcollection 1")
                        .AddSubcollection(out var subcollection4)
                            .SetName("subcollection 1.1")
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

        var request = new GetCollectionCardsRequest();
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(1);

        var resultCollection = result.First();
        resultCollection.Id.Should().Be(collection.Id);
        resultCollection.SubcollectionsCount.Should().Be(3);
        resultCollection.FlashcardsCount.Should().Be(0);
    }
    
     [TestMethod]
    public async Task GetUserCollectionsRequestHandler_Handle_ShouldReturnSubcollectionIfParentIdNotNull()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .SetName("collection 1")
                    .AddSubcollection(out var subcollection1)
                        .SetName("subcollection 1")
                        .AddSubcollection(out var subcollection4)
                            .SetName("subcollection 1.1")
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

        var request = new GetCollectionCardsRequest() { ParentCollectionId = subcollection1.Id};
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(3);

        result.Should().Contain(x => x.Id == subcollection4.Id);
        result.Should().Contain(x => x.Id == subcollection5.Id);
        result.Should().Contain(x => x.Id == subcollection6.Id);
    }

    [TestMethod]
    public async Task GetUserCollectionsRequestHandler_Handle_ShouldReturnOnlyCollectionsOfTheUser()
    {
        // Arrange
        await _testBuilder
            .AddUser(out var user1)
                .AddCollection(out var collection1)
                    .SetName("collection 1")
                    .Build()
                .Build()
            .AddUser(out var user2)
                .AddCollection(out var collection2)
                    .SetName("collection 1")
                    .Build()
                .Build()
            .AddUser(out var user3)
                .AddCollection(out var collection3)
                    .SetName("collection 1")
                    .Build()
                .Build()
            .SaveAsync();
        
        MockUserService.CurrentUser = user2;

        var request = new GetCollectionCardsRequest();
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(1);

        var resultCollection = result.First();
        resultCollection.Id.Should().Be(collection2.Id);
    }
}