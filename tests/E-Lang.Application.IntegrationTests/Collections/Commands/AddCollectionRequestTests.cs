using System.Data;
using E_Lang.Application.Collections.Commands;
using E_Lang.Application.Common.DTOs;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Collections.Commands;

[TestClass]
public class AddCollectionRequestTests : Setup
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
    public async Task AddCollectionRequest_Handle_ShouldThrowIfUserNotFound()
    {
         // Arrange 
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .SetName("Collection 1")
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection 1")
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = null;

        var dto = new CreateCollectionDto()
        {
            Name = "New test collection",
            ParentCollectionId = collection.Id
        };

        var request = new AddCollectionRequest()
        {
            CollectionDto = dto
        };

        var exceptionMessage =
            "User not found.";

        // Act
        var exception = await Assert.ThrowsExceptionAsync<DataException>(async () =>
            await _mediator.Send(request));
        
        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(exceptionMessage);
    }
    
    [TestMethod]
    public async Task AddCollectionRequest_Handle_ShouldReturnNewCollection()
    {
         // Arrange 
         _now = new DateTime(2023, 10, 07, 09, 26, 43);
         
         await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .SetName("Collection 1")
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection 1")
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();
         
         MockUserService.CurrentUser = user;

         var name = "New collection";
        var dto = new CreateCollectionDto()
        {
            Name = name,
            ParentCollectionId = collection.Id
        };

        var request = new AddCollectionRequest()
        {
            CollectionDto = dto
        };

        // Act
        var response = await _mediator.Send(request);
        
        // Assert
        response.Name.Should().Be(name);
        response.ParentId.Should().Be(collection.Id);
        response.ParentName.Should().Be(collection.Name);
    }
    
    [TestMethod]
    public async Task AddCollectionRequest_Handle_ShouldAddToDataBase()
    {
         // Arrange 
         _now = new DateTime(2023, 10, 07, 09, 26, 43);
         
         await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .SetName("Collection 1")
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection 1")
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

         MockUserService.CurrentUser = user;

         var name = "New subcollection";
        var dto = new CreateCollectionDto()
        {
            Name = name,
            ParentCollectionId = collection.Id
        };

        var request = new AddCollectionRequest()
        {
            CollectionDto = dto
        };

        // Act
        var response = await _mediator.Send(request);
        
        // Assert
        var newCollection = await _collectionRepository.GetByIdAsync(response.Id);
        newCollection.Should().NotBeNull();
        newCollection.Name.Should().Be(name);
        newCollection.CreatedOn.Should().Be(_dateTimeProvider.UtcNow);
        newCollection.ModifiedOn.Should().Be(_dateTimeProvider.UtcNow);
        newCollection.ParentId.Should().Be(collection.Id);
        newCollection.OwnerId.Should().Be(user.Id);
    }
}