using E_Lang.Application.Collections.Commands;
using E_Lang.Application.Common.DTOs;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Collections.Commands;

[TestClass]
public class UpdateCollectionRequestTests : Setup
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
    public async Task UpdateCollectionRequest_Handle_ShouldThrowIfCollectionNotFound()
    {
         // Arrange
         await _testBuilder
             .AddUser(out var user)
                .AddCollection(out var collection)
                    .Build()
                .Build()
             .SaveAsync();

         MockUserService.CurrentUser = user;

         var dto = new UpdateCollectionDto()
         {
             Id = Guid.NewGuid(),
             Name = "Updated name",
             ParentCollectionId = collection.Id
         };

         var request = new UpdateCollectionRequest()
         {
             UpdateDto = dto
         };

         var exceptionMessage = $"Collection with name {dto.Name} not found.";
         
         // Act
         var exception =
             await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _mediator.Send(request));
         
         // Assert
         exception.Should().NotBeNull();
         exception.Message.Should().Be(exceptionMessage);
    }
    
     [TestMethod]
    public async Task UpdateCollectionRequest_Handle_ShouldThrowIfIsOwnParent()
    {
         // Arrange
         await _testBuilder
             .AddUser(out var user)
                .AddCollection(out var collection)
                    .Build()
                .Build()
             .SaveAsync();

         MockUserService.CurrentUser = user;

         var dto = new UpdateCollectionDto()
         {
             Id = collection.Id,
             Name = "New Updated name",
             ParentCollectionId = collection.Id
         };

         var request = new UpdateCollectionRequest()
         {
             UpdateDto = dto
         };

         var exceptionMessage = $"Collection cannot be its own subcollection.";
         
         // Act
         var exception =
             await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _mediator.Send(request));
         
         // Assert
         exception.Should().NotBeNull();
         exception.Message.Should().Be(exceptionMessage);
    }

    [TestMethod]
    public async Task UpdateCollectionRequest_Handle_ShouldThrowIfNameAlreadyInUsed()
    {
        _now = new DateTime(2023, 09, 17, 08, 00, 00);
        
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection")
                        .AddSubcollection(out var subcollection2)
                            .SetName("Subcollection")
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var dto = new UpdateCollectionDto()
        {
            Id = subcollection2.Id,
            Name = collection.Name,
            ParentCollectionId = collection.Id
        };

        var request = new UpdateCollectionRequest()
        {
            UpdateDto = dto
        };
        
        var exceptionMessage = $"Collection with name {dto.Name} already exists.";
        
        // Act  
        var exception =
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _mediator.Send(request));
         
        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(exceptionMessage);
    }
    
    [TestMethod]
    public async Task UpdateCollectionRequest_Handle_ShouldNotThrowIfCollectionWithTheSameNameOfDifferentUser()
    {
        _now = new DateTime(2023, 09, 17, 08, 00, 00);
        
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection")
                        .AddSubcollection(out var subcollection2)
                            .SetName("Subcollection")
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .AddUser(out var user2)
                .AddCollection(out var collection2)
                    .SetName("Collection 2")
                    .AddSubcollection(out var subcollection3)
                        .SetName("Subcollection 3")
                        .AddSubcollection(out var subcollection4)
                            .SetName("Subcollection 4")
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var dto = new UpdateCollectionDto()
        {
            Id = subcollection2.Id,
            Name = collection2.Name,
            ParentCollectionId = collection.Id
        };

        var request = new UpdateCollectionRequest()
        {
            UpdateDto = dto
        };

        // Act
        var _ = await _mediator.Send(request);
         
        // Assert
        var updatedCollection = await _collectionRepository.GetByIdAsync(dto.Id);
        updatedCollection.Should().NotBeNull();
        updatedCollection.Name.Should().Be(dto.Name);
        updatedCollection.ParentId.Should().Be(collection.Id);
        updatedCollection.ModifiedOn.Should().Be(_dateTimeProvider.UtcNow);
    }
    
    [TestMethod]
    public async Task UpdateCollectionRequest_Handle_ShouldNotThrowIfNameNotChanged()
    {
        _now = new DateTime(2023, 09, 17, 08, 00, 00);
        
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection")
                        .AddSubcollection(out var subcollection2)
                            .SetName("Subcollection 2")
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var dto = new UpdateCollectionDto()
        {
            Id = subcollection2.Id,
            Name = subcollection2.Name,
            ParentCollectionId = collection.Id
        };

        var request = new UpdateCollectionRequest()
        {
            UpdateDto = dto
        };

        // Act
        var _ = await _mediator.Send(request);
         
        // Assert
        var updatedCollection = await _collectionRepository.GetByIdAsync(dto.Id);
        updatedCollection.Should().NotBeNull();
        updatedCollection.Name.Should().Be(dto.Name);
        updatedCollection.ParentId.Should().Be(collection.Id);
        updatedCollection.ModifiedOn.Should().Be(_dateTimeProvider.UtcNow);
    }
    
    [TestMethod]
    public async Task UpdateCollectionRequest_Handle_ShouldUpdateCollection()
    {
        _now = new DateTime(2023, 09, 17, 08, 00, 00);
        
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection")
                        .AddSubcollection(out var subcollection2)
                            .SetName("Subcollection")
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var dto = new UpdateCollectionDto()
        {
            Id = subcollection2.Id,
            Name = "Updated name",
            ParentCollectionId = collection.Id
        };

        var request = new UpdateCollectionRequest()
        {
            UpdateDto = dto
        };

        // Act
        var _ = await _mediator.Send(request);
         
        // Assert
        var updatedCollection = await _collectionRepository.GetByIdAsync(dto.Id);
        updatedCollection.Should().NotBeNull();
        updatedCollection.Name.Should().Be(dto.Name);
        updatedCollection.ParentId.Should().Be(collection.Id);
        updatedCollection.ModifiedOn.Should().Be(_dateTimeProvider.UtcNow);
    }
    
    [TestMethod]
    public async Task UpdateCollectionRequest_Handle_ShouldReturnUpdatedCollection()
    {
        _now = new DateTime(2023, 09, 17, 08, 00, 00);
        
        // Arrange
        await _testBuilder
            .AddUser(out var user)
                .AddCollection(out var collection)
                    .AddSubcollection(out var subcollection)
                        .SetName("Subcollection")
                        .AddSubcollection(out var subcollection2)
                            .SetName("Subcollection")
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();
        
        MockUserService.CurrentUser = user;

        var dto = new UpdateCollectionDto()
        {
            Id = subcollection2.Id,
            Name = "New name",
            ParentCollectionId = collection.Id
        };

        var request = new UpdateCollectionRequest()
        {
            UpdateDto = dto
        };

        // Act
        var result = await _mediator.Send(request);
         
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.ParentId.Should().Be(collection.Id);
        result.ParentName.Should().Be(collection.Name);
    }
}