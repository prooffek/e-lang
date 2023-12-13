using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Flashcards.Commands;
using E_Lang.Domain.Entities;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;
using Mapster;
using System.Net;

namespace E_Lang.Application.IntegrationTests.Flashcards.Commands
{
    [TestClass]
    public class RemoveFlashcardRequestTests : Setup
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
        [DataRow(false)]
        [DataRow(true)]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfIdNullOrEmpty(bool isNull)
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var flashcardId = isNull ? (Guid?)null : Guid.Empty;

            MockUserService.CurrentUser = null;

            var request = new RemoveFlashcardRequest
            {
                FlashcardId = flashcardId,
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<NullOrEmptyValidationException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.EntityName.Should().Be(nameof(Flashcard));
            exception.PropertyName.Should().Be(nameof(RemoveFlashcardRequest.FlashcardId));
            exception.ActionType.Should().Be(ActionTypes.Delete);
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfUserNotFound()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            MockUserService.CurrentUser = null;

            var request = new RemoveFlashcardRequest
            {
                FlashcardId = Guid.NewGuid(),
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<UserNotFoundException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfFlashcardNotFound()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var flashcardId = Guid.NewGuid();

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddFlashcard(out var flashcard1)
                        .AddFlashcardBase(out var flashcardBase1)
                            .AddMeaning(out var meaning)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new RemoveFlashcardRequest
            {
                FlashcardId = flashcardId
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<NotFoundValidationException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.EntityName.Should().Be(nameof(Flashcard));
            exception.AttributeName.Should().Be(nameof(Flashcard.Id));
            exception.Value.Should().Be(flashcardId.ToString());
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfUserNotFlashcardOwner()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddUser(out var user2)
                    .SetUsername("User2")
                    .SetEmail("user2@test.com")
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddFlashcard(out var flashcard1)
                        .AddFlashcardBase(out var flashcardBase1)
                            .AddMeaning(out var meaning)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user2;

            var request = new RemoveFlashcardRequest
            {
                FlashcardId = flashcard1.Id
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            exception.UserId.Should().Be(user2.Id);
            exception.ActionType.Should().Be(ActionTypes.Delete);
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldRemoveFlashcard()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddFlashcard(out var flashcard1)
                        .AddFlashcardBase(out var flashcardBase1)
                            .AddMeaning(out var meaning1)
                                .SetValue("Meaning")
                                .Build()
                            .AddMeaning(out var meaning2)
                                .SetValue("Meaning")
                                .Build()
                            .AddMeaning(out var meaning3)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard2)
                        .AddFlashcardBase(out var flashcardBase2)
                            .AddMeaning(out var meaning4)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .AddMeaning(out var meaning5)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new RemoveFlashcardRequest
            {
                FlashcardId = flashcard1.Id
            };

            // Act
            await _mediator.Send(request);

            // Assert
            var flashcard = await _flashcardRepository.GetByIdAsync(flashcard1.Id);
            flashcard.Should().BeNull();

            var flashcardBase = await _flashcardBaseRepository.GetByIdAsync(flashcardBase1.Id);
            flashcardBase.Should().BeNull();

            var meaning = await _meaningRepostory.GetByIdAsync(meaning1.Id);
            meaning.Should().BeNull();

            meaning = await _meaningRepostory.GetByIdAsync(meaning2.Id);
            meaning.Should().BeNull();

            meaning = await _meaningRepostory.GetByIdAsync(meaning3.Id);
            meaning.Should().BeNull();
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldNotRemoveFlashcardBaseIfUsed()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddFlashcard(out var flashcard1)
                        .AddFlashcardBase(out var flashcardBase1)
                            .AddMeaning(out var meaning1)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard2)
                        .SetFlashcardBase(flashcardBase1)
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .AddMeaning(out var meaning3)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new RemoveFlashcardRequest
            {
                FlashcardId = flashcard1.Id
            };

            // Act
            await _mediator.Send(request);

            // Assert
            var flashcard = await _flashcardRepository.GetByIdAsync(flashcard1.Id);
            flashcard.Should().BeNull();

            var flashcardBase = await _flashcardBaseRepository.GetByIdAsync(flashcardBase1.Id);
            flashcardBase.Should().NotBeNull();

            var meaning = await _meaningRepostory.GetByIdAsync(meaning1.Id);
            meaning.Should().NotBeNull();
        }
    }
}
