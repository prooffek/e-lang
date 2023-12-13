using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Flashcards.Commands;
using E_Lang.Domain.Entities;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;
using System.Net;

namespace E_Lang.Application.IntegrationTests.Flashcards.Commands
{
    [TestClass]
    public class RemoveFlashcardsRequestTests : Setup
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

        [DataRow(false)]
        [DataRow(true)]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfIdNullOrEmpty(bool isNull)
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var flashcardIds = isNull ? null : new List<Guid>();

            MockUserService.CurrentUser = null;

            var request = new RemoveFlashcardsRequest
            {
                FlashcardIds = flashcardIds,
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<NullOrEmptyValidationException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.EntityName.Should().Be(nameof(Flashcard));
            exception.PropertyName.Should().Be(nameof(RemoveFlashcardsRequest.FlashcardIds));
            exception.ActionType.Should().Be(ActionTypes.Delete);
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfUserNotFound()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            MockUserService.CurrentUser = null;

            var request = new RemoveFlashcardsRequest
            {
                FlashcardIds = new List<Guid>() { Guid.NewGuid() },
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<UserNotFoundException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfAnyFlashcardNotOwnedByUser()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var userId = Guid.NewGuid();

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
                        .AddFlashcardBase(out var flashcardBase2)
                            .AddMeaning(out var meaning2)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .AddCollection(out var collection2, userId)
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .AddMeaning(out var meaning3)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .AddMeaning(out var meaning4)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var ids = new List<Guid> { flashcard1.Id, flashcard2.Id, flashcard3.Id };

            var request = new RemoveFlashcardsRequest
            {
                FlashcardIds = ids,
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            exception.UserId.Should().Be(user.Id);
            exception.ActionType.Should().Be(ActionTypes.Delete);
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfAnyFlashcardNotFound()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var wrongId = Guid.NewGuid();

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
                        .AddFlashcardBase(out var flashcardBase2)
                            .AddMeaning(out var meaning2)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .AddMeaning(out var meaning3)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .AddMeaning(out var meaning4)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5)
                        .AddFlashcardBase(out var flashcardBase5)
                            .AddMeaning(out var meaning5)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var ids = new List<Guid> { flashcard1.Id, flashcard2.Id, flashcard3.Id, flashcard4.Id, wrongId };

            var request = new RemoveFlashcardsRequest
            {
                FlashcardIds = ids,
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<NotFoundValidationException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.EntityName.Should().Be(nameof(Flashcard));
            exception.AttributeName.Should().Be(nameof(Flashcard.Id));
            exception.Value.Should().Be(wrongId.ToString());
        }

        [TestMethod]
        public async Task RemoveFlashcardRequest_Handle_ShouldThrowIfManyFlashcardNotFound()
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var wrongId1 = Guid.NewGuid();
            var wrongId2 = Guid.NewGuid();
            var wrongId3 = Guid.NewGuid();

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
                        .AddFlashcardBase(out var flashcardBase2)
                            .AddMeaning(out var meaning2)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .AddMeaning(out var meaning3)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .AddMeaning(out var meaning4)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5)
                        .AddFlashcardBase(out var flashcardBase5)
                            .AddMeaning(out var meaning5)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var ids = new List<Guid> { flashcard1.Id, wrongId1, wrongId2, wrongId3 };

            var expectedErorValue = string.Join(',', new List<Guid> { wrongId1, wrongId2, wrongId3 });

            var request = new RemoveFlashcardsRequest
            {
                FlashcardIds = ids,
            };

            // Act
            var exception = await Assert.ThrowsExceptionAsync<NotFoundValidationException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.EntityName.Should().Be(nameof(Flashcard));
            exception.AttributeName.Should().Be(nameof(Flashcard.Id));
            exception.Value.Should().Be(expectedErorValue);
        }

        [TestMethod]
        public async Task RemoveFlashcardsRequest_Hanlde_ShouldRemoveFlashcards()
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
                        .AddFlashcardBase(out var flashcardBase2)
                            .AddMeaning(out var meaning2)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .AddMeaning(out var meaning3)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .AddMeaning(out var meaning4)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5)
                        .AddFlashcardBase(out var flashcardBase5)
                            .AddMeaning(out var meaning5)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var ids = new List<Guid> { flashcard1.Id, flashcard2.Id, flashcard3.Id, flashcard4.Id, flashcard5.Id };

            var request = new RemoveFlashcardsRequest
            {
                FlashcardIds = ids,
            };

            // Act
            await _mediator.Send(request);

            // Assert
            var flashcards = await _flashcardRepository.GetAllAsync();
            flashcards.Should().BeEmpty();

            var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
            flashcardBases.Should().BeEmpty();

            var meanings = await _meaningRepostory.GetAllAsync();
            meanings.Should().BeEmpty();
        }

        [TestMethod]
        public async Task RemoveFlashcardsRequest_Hanlde_ShouldRemoveUnusedFlashcardBases()
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
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .AddMeaning(out var meaning4)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5)
                        .AddFlashcardBase(out var flashcardBase5)
                            .AddMeaning(out var meaning5)
                                .SetValue("Meaning")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var ids = new List<Guid> { flashcard2.Id, flashcard3.Id };

            var request = new RemoveFlashcardsRequest
            {
                FlashcardIds = ids,
            };

            // Act
            await _mediator.Send(request);

            // Assert
            var flashcards = await _flashcardRepository.GetAllAsync();
            flashcards.Should().NotBeNullOrEmpty().And.HaveCount(3);
            flashcards.Any(f => f.Id == flashcard2.Id).Should().BeFalse();
            flashcards.Any(f => f.Id == flashcard3.Id).Should().BeFalse();

            var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
            flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(3);
            flashcardBases.Any(f => f.Id == flashcardBase1.Id).Should().BeTrue();
            flashcardBases.Any(f => f.Id == flashcardBase3.Id).Should().BeFalse();

            var meanings = await _meaningRepostory.GetAllAsync();
            meanings.Should().NotBeNullOrEmpty().And.HaveCount(3);
            meanings.Any(f => f.Id == meaning1.Id).Should().BeTrue();
            meanings.Any(f => f.Id == meaning3.Id).Should().BeFalse();
        }
    }
}
