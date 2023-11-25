using E_Lang.Application.Collections.Commands;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
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

        var expectedException = new NullOrEmptyValidationException(nameof(Collection), nameof(Collection.Id), ActionTypes.Delete);

        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<NullOrEmptyValidationException>(async () => await _mediator.Send(request));
        
        // Assert
        exception.Should().NotBeNull();
        exception.EntityName.Should().Be(nameof(Collection));
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
        exception.PropertyName.Should().Be(nameof(Collection.Id));
        exception.ActionType.Should().Be(ActionTypes.Delete);

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

        var expectedException = new NotFoundValidationException(nameof(Collection), nameof(Collection.Id), request.Id.Value.ToString());
        
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

        var expectedException = new UnauthorizedException(user2.Id, ActionTypes.Delete);
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () => await _mediator.Send(request));

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
        exception.UserId.Should().Be(user2.Id);
        exception.ActionType.Should().Be(ActionTypes.Delete);
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

        var instruction = "Delete all subcollections and try again.";

        var expectedException = new RelatedRecordValidationException(nameof(Collection), instruction);
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<RelatedRecordValidationException>(async () => await _mediator.Send(request));

        // Assert
        exception.Should().NotBeNull();
        exception.EntityName.Should().Be(nameof(Collection));
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
        exception.Instruction.Should().Be(instruction);
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