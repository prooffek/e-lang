using E_Lang.Application.Attempts.Commands;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Attempts.Commands;

[TestClass]
public class DeleteAttemptRequestTests : Setup
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
    public async Task DeleteAttemptRequestTests_Handle_ShouldThrowIfUserNotFound()
    {
        // Arrange 
        await _testBuilder
            .AddUser(out var user)
            .Build()
            .SaveAsync();

        MockUserService.CurrentUser = null;

        var request = new DeleteAttemptRequest()
        {
            AttemptId = Guid.NewGuid()
        };

        var expectedException = new UserNotFoundException();

        // Act
        var exception = await Assert.ThrowsExceptionAsync<UserNotFoundException>(async () =>
            await _mediator.Send(request));

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
    }

    [TestMethod]
    public async Task DeleteAttemptRequestTests_Handle_ShouldThrowIfUserNotOwner()
    {
        // Arrange 
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddQuizType(user.Id, out var quizType)
                .Build()
            .AddUser(out var user2)
                .SetUsername("differentUser")
                .SetEmail("different@test.pl")
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase("Word 1")
                        .Build()
                    .AddMeaning(out var meaning1, flashcardBase1.Id)
                        .SetValue("Meaning 1")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard2)
                    .AddFlashcardBase(out var flashcardBase2)
                        .SetWordOrPhrase("Word 2")
                        .Build()
                    .AddMeaning(out var meaning2, flashcardBase1.Id)
                        .SetValue("Meaning 2")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard3)
                    .AddFlashcardBase(out var flashcardBase3)
                        .SetWordOrPhrase("Word 3")
                        .Build()
                    .AddMeaning(out var meaning3, flashcardBase3.Id)
                        .SetValue("Meaning 3")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard4)
                    .AddFlashcardBase(out var flashcardBase4)
                        .SetWordOrPhrase("Word 4")
                        .Build()
                    .AddMeaning(out var meaning4, flashcardBase4.Id)
                        .SetValue("Meaning 4")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard5)
                    .AddFlashcardBase(out var flashcardBase5)
                        .SetWordOrPhrase("Word 5")
                        .Build()
                    .AddMeaning(out var meaning5, flashcardBase5.Id)
                        .SetValue("Meaning 5")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard6)
                    .AddFlashcardBase(out var flashcardBase6)
                        .SetWordOrPhrase("Word 6")
                        .Build()
                    .AddMeaning(out var meaning6, flashcardBase6.Id)
                        .SetValue("Meaning 6")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard7)
                    .AddFlashcardBase(out var flashcardBase7)
                        .SetWordOrPhrase("Word 7")
                        .Build()
                    .AddMeaning(out var meaning7, flashcardBase7.Id)
                        .SetValue("Meaning 7")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard8)
                    .AddFlashcardBase(out var flashcardBase8)
                        .SetWordOrPhrase("Word 8")
                        .Build()
                    .AddMeaning(out var meaning8, flashcardBase8.Id)
                        .SetValue("Meaning 8")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard9)
                    .AddFlashcardBase(out var flashcardBase9)
                        .SetWordOrPhrase("Word 9")
                        .Build()
                    .AddMeaning(out var meaning9, flashcardBase9.Id)
                        .SetValue("Meaning 9")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard10)
                    .AddFlashcardBase(out var flashcardBase10)
                        .SetWordOrPhrase("Word 5")
                        .Build()
                    .AddMeaning(out var meaning10, flashcardBase10.Id)
                        .SetValue("Meaning 5")
                        .Build()
                    .Build()
                .AddAttempt(out var attempt, quizType)
                    .AddInitAttemptStageAsCurrentStage(out var initStage)
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user2;

        var request = new DeleteAttemptRequest()
        {
            AttemptId = attempt.Id,
        };

        var expectedException = new UnauthorizedException(user2.Id, ActionTypes.Delete);

        // Act
        var exception = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            await _mediator.Send(request));

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedException.Message);
        exception.StatusCode.Should().Be(expectedException.StatusCode);
    }

    [TestMethod]
    public async Task DeleteAttemptRequestTests_Handle_ShouldRemoveAttempt()
    {
        // Arrange 
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddQuizType(user.Id, out var quizType)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase("Word 1")
                        .Build()
                    .AddMeaning(out var meaning1, flashcardBase1.Id)
                        .SetValue("Meaning 1")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard2)
                    .AddFlashcardBase(out var flashcardBase2)
                        .SetWordOrPhrase("Word 2")
                        .Build()
                    .AddMeaning(out var meaning2, flashcardBase1.Id)
                        .SetValue("Meaning 2")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard3)
                    .AddFlashcardBase(out var flashcardBase3)
                        .SetWordOrPhrase("Word 3")
                        .Build()
                    .AddMeaning(out var meaning3, flashcardBase3.Id)
                        .SetValue("Meaning 3")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard4)
                    .AddFlashcardBase(out var flashcardBase4)
                        .SetWordOrPhrase("Word 4")
                        .Build()
                    .AddMeaning(out var meaning4, flashcardBase4.Id)
                        .SetValue("Meaning 4")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard5)
                    .AddFlashcardBase(out var flashcardBase5)
                        .SetWordOrPhrase("Word 5")
                        .Build()
                    .AddMeaning(out var meaning5, flashcardBase5.Id)
                        .SetValue("Meaning 5")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard6)
                    .AddFlashcardBase(out var flashcardBase6)
                        .SetWordOrPhrase("Word 6")
                        .Build()
                    .AddMeaning(out var meaning6, flashcardBase6.Id)
                        .SetValue("Meaning 6")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard7)
                    .AddFlashcardBase(out var flashcardBase7)
                        .SetWordOrPhrase("Word 7")
                        .Build()
                    .AddMeaning(out var meaning7, flashcardBase7.Id)
                        .SetValue("Meaning 7")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard8)
                    .AddFlashcardBase(out var flashcardBase8)
                        .SetWordOrPhrase("Word 8")
                        .Build()
                    .AddMeaning(out var meaning8, flashcardBase8.Id)
                        .SetValue("Meaning 8")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard9)
                    .AddFlashcardBase(out var flashcardBase9)
                        .SetWordOrPhrase("Word 9")
                        .Build()
                    .AddMeaning(out var meaning9, flashcardBase9.Id)
                        .SetValue("Meaning 9")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard10)
                    .AddFlashcardBase(out var flashcardBase10)
                        .SetWordOrPhrase("Word 10")
                        .Build()
                    .AddMeaning(out var meaning10, flashcardBase10.Id)
                        .SetValue("Meaning 10")
                        .Build()
                    .Build()
                .AddAttempt(out var attempt, quizType)
                    .AddInitAttemptStageAsCurrentStage(out var initStage)
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new DeleteAttemptRequest()
        {
            AttemptId = attempt.Id,
        };

        // Act
        await _mediator.Send(request);

        // Assert
        var result = await _attemptRepostory.GetByIdAsync(attempt.Id);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteAttemptRequestTests_Handle_ShouldRemoveAttemptStage()
    {
        // Arrange 
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddQuizType(user.Id, out var quizType)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase("Word 1")
                        .Build()
                    .AddMeaning(out var meaning1, flashcardBase1.Id)
                        .SetValue("Meaning 1")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard2)
                    .AddFlashcardBase(out var flashcardBase2)
                        .SetWordOrPhrase("Word 2")
                        .Build()
                    .AddMeaning(out var meaning2, flashcardBase1.Id)
                        .SetValue("Meaning 2")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard3)
                    .AddFlashcardBase(out var flashcardBase3)
                        .SetWordOrPhrase("Word 3")
                        .Build()
                    .AddMeaning(out var meaning3, flashcardBase3.Id)
                        .SetValue("Meaning 3")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard4)
                    .AddFlashcardBase(out var flashcardBase4)
                        .SetWordOrPhrase("Word 4")
                        .Build()
                    .AddMeaning(out var meaning4, flashcardBase4.Id)
                        .SetValue("Meaning 4")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard5)
                    .AddFlashcardBase(out var flashcardBase5)
                        .SetWordOrPhrase("Word 5")
                        .Build()
                    .AddMeaning(out var meaning5, flashcardBase5.Id)
                        .SetValue("Meaning 5")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard6)
                    .AddFlashcardBase(out var flashcardBase6)
                        .SetWordOrPhrase("Word 6")
                        .Build()
                    .AddMeaning(out var meaning6, flashcardBase6.Id)
                        .SetValue("Meaning 6")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard7)
                    .AddFlashcardBase(out var flashcardBase7)
                        .SetWordOrPhrase("Word 7")
                        .Build()
                    .AddMeaning(out var meaning7, flashcardBase7.Id)
                        .SetValue("Meaning 7")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard8)
                    .AddFlashcardBase(out var flashcardBase8)
                        .SetWordOrPhrase("Word 8")
                        .Build()
                    .AddMeaning(out var meaning8, flashcardBase8.Id)
                        .SetValue("Meaning 8")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard9)
                    .AddFlashcardBase(out var flashcardBase9)
                        .SetWordOrPhrase("Word 9")
                        .Build()
                    .AddMeaning(out var meaning9, flashcardBase9.Id)
                        .SetValue("Meaning 9")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard10)
                    .AddFlashcardBase(out var flashcardBase10)
                        .SetWordOrPhrase("Word 10")
                        .Build()
                    .AddMeaning(out var meaning10, flashcardBase10.Id)
                        .SetValue("Meaning 10")
                        .Build()
                    .Build()
                .AddAttempt(out var attempt, quizType)
                    .AddAttemptStage(out var initStage)
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new DeleteAttemptRequest()
        {
            AttemptId = attempt.Id,
        };

        // Act
        await _mediator.Send(request);

        // Assert
        var result = await _attemptStageRepostory.GetByIdAsync(initStage.Id);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteAttemptRequestTests_Handle_ShouldRemoveRelatedFlashcardStates()
    {
        // Arrange 
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddQuizType(user.Id, out var quizType)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase("Word 1")
                        .Build()
                    .AddMeaning(out var meaning1, flashcardBase1.Id)
                        .SetValue("Meaning 1")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard2)
                    .AddFlashcardBase(out var flashcardBase2)
                        .SetWordOrPhrase("Word 2")
                        .Build()
                    .AddMeaning(out var meaning2, flashcardBase1.Id)
                        .SetValue("Meaning 2")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard3)
                    .AddFlashcardBase(out var flashcardBase3)
                        .SetWordOrPhrase("Word 3")
                        .Build()
                    .AddMeaning(out var meaning3, flashcardBase3.Id)
                        .SetValue("Meaning 3")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard4)
                    .AddFlashcardBase(out var flashcardBase4)
                        .SetWordOrPhrase("Word 4")
                        .Build()
                    .AddMeaning(out var meaning4, flashcardBase4.Id)
                        .SetValue("Meaning 4")
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard5)
                    .AddFlashcardBase(out var flashcardBase5)
                        .SetWordOrPhrase("Word 5")
                        .Build()
                    .AddMeaning(out var meaning5, flashcardBase5.Id)
                        .SetValue("Meaning 5")
                        .Build()
                    .Build()
                .AddAttempt(out var attempt, quizType)
                    .AddAttemptStage(out var initStage)
                        .AddFlashcard(out var flashcard6, collection)
                            .AddFlashcardBase(out var flashcardBase6)
                                .SetWordOrPhrase("Word 6")
                                .Build()
                            .AddMeaning(out var meaning6, flashcardBase6.Id)
                                .SetValue("Meaning 6")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard7, collection)
                            .AddFlashcardBase(out var flashcardBase7)
                                .SetWordOrPhrase("Word 7")
                                .Build()
                            .AddMeaning(out var meaning7, flashcardBase7.Id)
                                .SetValue("Meaning 7")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard8, collection)
                            .AddFlashcardBase(out var flashcardBase8)
                                .SetWordOrPhrase("Word 8")
                            .Build()
                        .AddMeaning(out var meaning8, flashcardBase8.Id)
                            .SetValue("Meaning 8")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard9, collection)
                        .AddFlashcardBase(out var flashcardBase9)
                            .SetWordOrPhrase("Word 9")
                            .Build()
                        .AddMeaning(out var meaning9, flashcardBase9.Id)
                            .SetValue("Meaning 9")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard10, collection)
                        .AddFlashcardBase(out var flashcardBase10)
                            .SetWordOrPhrase("Word 10")
                            .Build()
                        .AddMeaning(out var meaning10, flashcardBase10.Id)
                            .SetValue("Meaning 10")
                            .Build()
                        .Build()
                            .Build()
                        .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new DeleteAttemptRequest()
        {
            AttemptId = attempt.Id,
        };

        // Act
        await _mediator.Send(request);

        // Assert
        var result = await _flashcardStateRepository.GetAllAsync();
        result.Should().BeNullOrEmpty();

        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().HaveCount(10);
    }

    [TestMethod]
    public async Task DeleteAttemptRequestTests_Handle_ShouldNotRemoveCollection()
    {
        // Arrange 
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddQuizType(user.Id, out var quizType)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddAttempt(out var attempt, quizType)
                    .AddInitAttemptStageAsCurrentStage(out var initStage)
                        .AddFlashcard(out var flashcard1, collection)
                            .AddFlashcardBase(out var flashcardBase1)
                                .SetWordOrPhrase("Word 1")
                                .Build()
                            .AddMeaning(out var meaning1, flashcardBase1.Id)
                                .SetValue("Meaning 1")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard2, collection)
                            .AddFlashcardBase(out var flashcardBase2)
                                .SetWordOrPhrase("Word 2")
                                .Build()
                            .AddMeaning(out var meaning2, flashcardBase2.Id)
                                .SetValue("Meaning 2")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard3, collection)
                            .AddFlashcardBase(out var flashcardBase3)
                                .SetWordOrPhrase("Word 3")
                            .Build()
                        .AddMeaning(out var meaning3, flashcardBase3.Id)
                            .SetValue("Meaning 3")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4, collection)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Word 4")
                            .Build()
                        .AddMeaning(out var meaning4, flashcardBase4.Id)
                            .SetValue("Meaning 4")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5, collection)
                        .AddFlashcardBase(out var flashcardBase5)
                            .SetWordOrPhrase("Word 5")
                            .Build()
                        .AddMeaning(out var meaning5, flashcardBase5.Id)
                            .SetValue("Meaning 5")
                            .Build()
                        .Build()
                            .Build()
                        .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new DeleteAttemptRequest()
        {
            AttemptId = attempt.Id,
        };

        // Act
        await _mediator.Send(request);

        // Assert
        var result = await _collectionRepository.GetByIdAsync(collection.Id);
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task DeleteAttemptRequestTests_Handle_ShouldRemoveOnlyOneAttempt()
    {
        // Arrange 
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddQuizType(user.Id, out var quizType)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddAttempt(out var attempt, quizType)
                    .AddInitAttemptStageAsCurrentStage(out var initStage)
                        .AddFlashcard(out var flashcard1, collection)
                            .AddFlashcardBase(out var flashcardBase1)
                                .SetWordOrPhrase("Word 1")
                                .Build()
                            .AddMeaning(out var meaning1, flashcardBase1.Id)
                                .SetValue("Meaning 1")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard2, collection)
                            .AddFlashcardBase(out var flashcardBase2)
                                .SetWordOrPhrase("Word 2")
                                .Build()
                            .AddMeaning(out var meaning2, flashcardBase2.Id)
                                .SetValue("Meaning 2")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard3, collection)
                            .AddFlashcardBase(out var flashcardBase3)
                                .SetWordOrPhrase("Word 3")
                            .Build()
                        .AddMeaning(out var meaning3, flashcardBase3.Id)
                            .SetValue("Meaning 3")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4, collection)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Word 4")
                            .Build()
                        .AddMeaning(out var meaning4, flashcardBase4.Id)
                            .SetValue("Meaning 4")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5, collection)
                        .AddFlashcardBase(out var flashcardBase5)
                            .SetWordOrPhrase("Word 5")
                            .Build()
                        .AddMeaning(out var meaning5, flashcardBase5.Id)
                            .SetValue("Meaning 5")
                            .Build()
                        .Build()
                            .Build()
                        .Build()
                .AddAttempt(out var attempt2, quizType)
                    .AddInitAttemptStageAsCurrentStage(out var initStage2)
                        .AddFlashcard(out var flashcard6, collection)
                            .AddFlashcardBase(out var flashcardBase6)
                                .SetWordOrPhrase("Word 6")
                                .Build()
                            .AddMeaning(out var meaning6, flashcardBase6.Id)
                                .SetValue("Meaning 6")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard7, collection)
                            .AddFlashcardBase(out var flashcardBase7)
                                .SetWordOrPhrase("Word 7")
                                .Build()
                            .AddMeaning(out var meaning7, flashcardBase7.Id)
                                .SetValue("Meaning 7")
                                .Build()
                            .Build()
                        .AddFlashcard(out var flashcard8, collection)
                            .AddFlashcardBase(out var flashcardBase8)
                                .SetWordOrPhrase("Word 8")
                            .Build()
                        .AddMeaning(out var meaning8, flashcardBase8.Id)
                            .SetValue("Meaning 8")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard9, collection)
                        .AddFlashcardBase(out var flashcardBase9)
                            .SetWordOrPhrase("Word 9")
                            .Build()
                        .AddMeaning(out var meaning9, flashcardBase9.Id)
                            .SetValue("Meaning 9")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard10, collection)
                        .AddFlashcardBase(out var flashcardBase10)
                            .SetWordOrPhrase("Word 10")
                            .Build()
                        .AddMeaning(out var meaning10, flashcardBase10.Id)
                            .SetValue("Meaning 10")
                            .Build()
                        .Build()
                            .Build()
                        .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new DeleteAttemptRequest()
        {
            AttemptId = attempt.Id,
        };

        // Act
        await _mediator.Send(request);

        // Assert
        var result = await _attemptRepostory.GetByIdAsync(attempt.Id);
        result.Should().BeNull();

        var result2 = await _attemptRepostory.GetByIdAsync(attempt2.Id);
        result2.Should().NotBeNull();
    }
}