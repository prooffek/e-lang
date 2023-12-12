using System.Net;
using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Flashcards.Commands;
using E_Lang.Domain.Enums;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Flashcards.Commands;

[TestClass]
public class AddOrUpdateFlashcardRequestTests : Setup
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
    public async Task AddFlashcardRequestHandler_Handle_ShouldThrowIfUserNotFound()
    {
        // Arrange
        MockUserService.CurrentUser = null;

        var request = new AddOrUpdateFlashcardRequest();
        
        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<UserNotFoundException>(async () => await _mediator.Send(request));
        
        // Assert
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldReturnFlashcardDto()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto {Value = meaning}
                }
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty().And.NotBe(Guid.Empty);
        result.CollectionName.Should().NotBeNullOrEmpty().And.Be(collection.Name);
        result.WordOrPhrase.Should().Be(wordOrPhrase);
        result.Meanings.Count.Should().Be(1);
        result.Meanings.First().Value.Should().Be(meaning);
        result.CreatedOn.Should().Be(expectedTime);
        result.LastSeenOn.Should().BeNull();
        result.Status.Should().Be(FlashcardStatus.Active);
    }

    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldAddNewFlashcard()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);
        
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Value = meaning }
                }
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        var flashcard = await _flashcardRepository.GetByIdAsync(result.Id, default);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(expectedTime);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        var savedMeaning = meanings.First();
        savedMeaning.Value.Should().Be(meaning);
        savedMeaning.CreatedOn.Should().Be(expectedTime);
        savedMeaning.ModifiedOn.Should().Be(expectedTime);
    }
    
    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldAddDuplicateIfFlashcardBaseWithWordPhraseExists()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);
        
        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning }
                },
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(2);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(2);

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().NotBe(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(expectedTime);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        var savedMeaning = meanings.First();
        savedMeaning.Value.Should().Be(meaning);
        savedMeaning.CreatedOn.Should().Be(expectedTime);
        savedMeaning.ModifiedOn.Should().Be(expectedTime);
    }
    
    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldAssignExistingFlashcardBaseIfFlashcardIdGiven()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);
        
        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning }
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var createdOn = _dateTimeProvider.UtcNow;

        _now = _now.AddDays(2);

        var expectedTime = _dateTimeProvider.UtcNow;
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(2);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(createdOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        var savedMeaning = meanings.First();
        savedMeaning.Value.Should().Be(meaning);
        savedMeaning.CreatedOn.Should().Be(createdOn);
        savedMeaning.ModifiedOn.Should().Be(expectedTime);
    }
    
    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldUpdateWordOrPhraseOnExistingFlashcardBaseIfUpdateAction()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);
        
        var wordOrPhrase = "New test word";
        var meaning = "Test meaning";
        
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase("Test word")
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning }
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var createdOn = _dateTimeProvider.UtcNow;

        _now = _now.AddDays(2);

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(2);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);
        flashcardBases.All(f => f.WordOrPhrase == wordOrPhrase).Should().BeTrue();

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(createdOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        var savedMeaning = meanings.First();
        savedMeaning.Value.Should().Be(meaning);
        savedMeaning.CreatedOn.Should().Be(createdOn);
        savedMeaning.ModifiedOn.Should().Be(expectedTime);
    }
    
    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldAddNewMeaningToExistingFlashcardBaseIfUpdateAction()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);
        
        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var newMeaning = "New meaning";
        
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning },
                    new AddOrUpdateMeaningDto{ Value = newMeaning }
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;
        
        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(2);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);
        flashcardBases.All(f => f.WordOrPhrase == wordOrPhrase).Should().BeTrue();
        flashcardBases.All(f => f.Meanings.Count == 2).Should().BeTrue();

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(flashcardBase.CreatedOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(2);
        meanings.Any(m => m.Value == meaning).Should().BeTrue();
        meanings.Any(m => m.Value == newMeaning).Should().BeTrue();

        var oldMeaning = meanings.First(m => m.Id == meaning1.Id);
        oldMeaning.CreatedOn.Should().Be(meaning1.CreatedOn);
        oldMeaning.ModifiedOn.Should().Be(meaning1.ModifiedOn);

        var freshMeaning = meanings.First(m => m.Id != meaning1.Id);
        freshMeaning.CreatedOn.Should().Be(expectedTime);
        freshMeaning.ModifiedOn.Should().Be(expectedTime);
    }

    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldRemoveMeaningsFromExistingFlashcardBaseIfUpdateAction()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var newMeaning = "New meaning";

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning2)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning3)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning4)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning },
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(2);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);
        flashcardBases.All(f => f.WordOrPhrase == wordOrPhrase).Should().BeTrue();
        flashcardBases.All(f => f.Meanings.Count == 1).Should().BeTrue();

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(flashcardBase.CreatedOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        meanings.Any(m => m.Value == meaning).Should().BeTrue();

        var oldMeaning = meanings.First(m => m.Id == meaning1.Id);
        oldMeaning.CreatedOn.Should().Be(meaning1.CreatedOn);
        oldMeaning.ModifiedOn.Should().Be(meaning1.ModifiedOn);
    }

    [TestMethod]
    public async Task AddFlashcardRequestHandler_Handle_ShouldUpdateMeaningOnExistingFlashcardBaseIfUpdateAction()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);
        
        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var anotherMeaning = "Another meaning";
        var newMeaning = "New meaning";
        
        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning2)
                            .SetValue(anotherMeaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning },
                    new AddOrUpdateMeaningDto{ Id = meaning2.Id, Value = newMeaning }
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var createdOn = _dateTimeProvider.UtcNow;

        _now = _now.AddDays(2);

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);
        
        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(2);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);
        flashcardBases.All(f => f.WordOrPhrase == wordOrPhrase).Should().BeTrue();
        flashcardBases.All(f => f.Meanings.Count == 2).Should().BeTrue();

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(flashcardBase.CreatedOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(2);
        meanings.Any(m => m.Value == meaning).Should().BeTrue();
        meanings.Any(m => m.Value == newMeaning).Should().BeTrue();
        meanings.Any(m => m.Value == anotherMeaning).Should().BeFalse();
    }

    [TestMethod]
    public async Task UpdateFlashcardRequestHandler_Handle_ShouldUpdateFlashcard()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var anotherMeaning = "Another meaning";

        await _testBuilder
           .AddUser(out var user)
               .Build()
           .AddCollection(out var collection, user.Id)
               .AddFlashcard(out var flashcard1)
                   .AddFlashcardBase(out var flashcardBase1)
                       .SetWordOrPhrase(wordOrPhrase)
                       .AddMeaning(out var meaning1)
                           .SetValue(meaning)
                           .Build()
                       .AddMeaning(out var meaning2)
                           .SetValue(anotherMeaning)
                           .Build()
                       .Build()
                   .Build()
               .Build()
            .AddCollection(out var collection2, user.Id)
                .Build()
           .SaveAsync();

        MockUserService.CurrentUser = user;

        var editedWordOrPhrase = "Edited word";
        var editedMeaning = "Edited meaning";


        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                FlashcardId = flashcard1.Id,
                FlashcardBaseId = flashcardBase1.Id,
                CollectionId = collection2.Id,
                WordOrPhrase = editedWordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = editedMeaning },
                    new AddOrUpdateMeaningDto{ Id = meaning2.Id, Value = meaning2.Value },
                }
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcard = await _flashcardRepository.GetByIdAsync(result.Id, default);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection2.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);
        flashcard.CreatedOn.Should().Be(flashcard1.CreatedOn);
        flashcard.ModifiedOn.Should().Be(expectedTime);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcardBase.WordOrPhrase.Should().Be(editedWordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(flashcardBase1.CreatedOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(2);
        var savedMeaning1 = meanings.First(m => m.Id == meaning1.Id);
        savedMeaning1.Value.Should().Be(editedMeaning);
        savedMeaning1.CreatedOn.Should().Be(meaning1.CreatedOn);
        savedMeaning1.ModifiedOn.Should().Be(expectedTime);

        var savedMeaning2 = meanings.First(m => m.Id == meaning2.Id);
        savedMeaning2.Value.Should().Be(meaning2.Value);
        savedMeaning2.CreatedOn.Should().Be(meaning2.CreatedOn);
        savedMeaning2.ModifiedOn.Should().Be(meaning2.ModifiedOn);
    }

    [TestMethod]
    public async Task UpdateFlashcardRequestHandler_Handle_ShouldAddDuplicateIfFlashcardBaseDetached()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard2)
                    .SetFlashcardBase(flashcardBase1)
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                FlashcardId = flashcard2.Id,
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning }
                },
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(2);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(2);

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().NotBe(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(expectedTime);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        var savedMeaning = meanings.First();
        savedMeaning.Value.Should().Be(meaning);
        savedMeaning.CreatedOn.Should().Be(expectedTime);
        savedMeaning.ModifiedOn.Should().Be(expectedTime);
    }

    [TestMethod]
    public async Task UpdateFlashcardRequestHandler_Handle_ShouldUpdateFlashcardBase()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        var editedWordOrPhrase = "Edited word";
        var editedMeaning = "Edited meaning";

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                FlashcardId = flashcard1.Id,
                FlashcardBaseId = flashcardBase1.Id,
                CollectionId = collection.Id,
                WordOrPhrase = editedWordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = editedMeaning }
                },
            }
        };

        var createdOn = _dateTimeProvider.UtcNow;

        _now = _now.AddDays(2);

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(1);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);
        flashcard.CreatedOn.Should().Be(flashcard1.CreatedOn);
        flashcard.ModifiedOn.Should().Be(expectedTime);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(editedWordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(createdOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        var savedMeaning = meanings.First();
        savedMeaning.Value.Should().Be(editedMeaning);
        savedMeaning.CreatedOn.Should().Be(createdOn);
        savedMeaning.ModifiedOn.Should().Be(expectedTime);
    }

    [TestMethod]
    public async Task UpdateFlashcardRequestHandler_Handle_ShouldAddNewMeaningToFlashcardBase()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var newMeaning = "New meaning";

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                FlashcardId = flashcard1.Id,
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning },
                    new AddOrUpdateMeaningDto{ Value = newMeaning }
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(1);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);
        flashcardBases.All(f => f.WordOrPhrase == wordOrPhrase).Should().BeTrue();
        flashcardBases.All(f => f.Meanings.Count == 2).Should().BeTrue();

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(flashcardBase.CreatedOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(2);
        meanings.Any(m => m.Value == meaning).Should().BeTrue();
        meanings.Any(m => m.Value == newMeaning).Should().BeTrue();

        var oldMeaning = meanings.First(m => m.Id == meaning1.Id);
        oldMeaning.CreatedOn.Should().Be(meaning1.CreatedOn);
        oldMeaning.ModifiedOn.Should().Be(meaning1.ModifiedOn);

        var freshMeaning = meanings.First(m => m.Id != meaning1.Id);
        freshMeaning.CreatedOn.Should().Be(expectedTime);
        freshMeaning.ModifiedOn.Should().Be(expectedTime);
    }

    [TestMethod]
    public async Task UpdateFlashcardRequestHandler_Handle_ShouldRemoveMeaningsFromExistingFlashcardBase()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var newMeaning = "New meaning";

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning2)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning3)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning4)
                            .SetValue(meaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                FlashcardId = flashcard1.Id,
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning },
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(1);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);
        flashcardBases.All(f => f.WordOrPhrase == wordOrPhrase).Should().BeTrue();
        flashcardBases.All(f => f.Meanings.Count == 1).Should().BeTrue();

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(flashcardBase.CreatedOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(1);
        meanings.Any(m => m.Value == meaning).Should().BeTrue();

        var oldMeaning = meanings.First(m => m.Id == meaning1.Id);
        oldMeaning.CreatedOn.Should().Be(meaning1.CreatedOn);
        oldMeaning.ModifiedOn.Should().Be(meaning1.ModifiedOn);
    }

    [TestMethod]
    public async Task UpdateFlashcardRequestHandler_Handle_ShouldUpdateMeaningOnFlashcardBase()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var anotherMeaning = "Another meaning";
        var newMeaning = "New meaning";

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning2)
                            .SetValue(anotherMeaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                FlashcardId = flashcard1.Id,
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning1.Id, Value = meaning },
                    new AddOrUpdateMeaningDto{ Id = meaning2.Id, Value = newMeaning }
                },
                FlashcardBaseId = flashcardBase1.Id
            }
        };

        var createdOn = _dateTimeProvider.UtcNow;

        _now = _now.AddDays(2);

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcards = await _flashcardRepository.GetAllAsync();
        flashcards.Should().NotBeNullOrEmpty().And.HaveCount(1);
        var flashcardBases = await _flashcardBaseRepository.GetAllAsync();
        flashcardBases.Should().NotBeNullOrEmpty().And.HaveCount(1);
        flashcardBases.All(f => f.WordOrPhrase == wordOrPhrase).Should().BeTrue();
        flashcardBases.All(f => f.Meanings.Count == 2).Should().BeTrue();

        var flashcard = flashcards.First(f => f.Id == result.Id);
        flashcard.Should().NotBeNull();
        flashcard.CollectionId.Should().Be(collection.Id);
        flashcard.Status.Should().Be(FlashcardStatus.Active);
        flashcard.LastSeenOn.Should().BeNull();
        flashcard.LastStatusChangedOn.Should().Be(expectedTime);
        flashcard.OwnerId.Should().Be(user.Id);

        var flashcardBase = flashcard.FlashcardBase;
        flashcardBase.Should().NotBeNull();
        flashcard.FlashcardBase.Id.Should().Be(flashcardBase1.Id);
        flashcardBase.WordOrPhrase.Should().Be(wordOrPhrase);
        flashcardBase.CreatedOn.Should().Be(flashcardBase.CreatedOn);
        flashcardBase.ModifiedOn.Should().Be(expectedTime);

        var meanings = flashcardBase.Meanings;
        meanings.Should().NotBeNull().And.HaveCount(2);
        meanings.Any(m => m.Value == meaning).Should().BeTrue();
        meanings.Any(m => m.Value == newMeaning).Should().BeTrue();
        meanings.Any(m => m.Value == anotherMeaning).Should().BeFalse();
    }

    [TestMethod]
    public async Task UpdateFlashcardRequestHandler_Handle_ShouldRemoveUnusedFlashcardBase()
    {
        // Arrange
        _now = new DateTime(2022, 10, 10, 10, 00, 00);

        var wordOrPhrase = "Test word";
        var meaning = "Test meaning";
        var anotherMeaning = "Another meaning";
        var newMeaning = "New meaning";

        await _testBuilder
            .AddUser(out var user)
                .Build()
            .AddCollection(out var collection, user.Id)
                .AddFlashcard(out var flashcard1)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning1)
                            .SetValue(meaning)
                            .Build()
                        .AddMeaning(out var meaning2)
                            .SetValue(anotherMeaning)
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard2)
                    .AddFlashcardBase(out var flashcardBase2)
                        .SetWordOrPhrase(wordOrPhrase)
                        .AddMeaning(out var meaning3)
                            .SetValue(newMeaning)
                            .Build()
                        .Build()
                    .Build()
                .Build()
            .SaveAsync();

        MockUserService.CurrentUser = user;

        var request = new AddOrUpdateFlashcardRequest
        {
            Flashcard = new AddOrUpdateFlashcardDto
            {
                FlashcardId = flashcard1.Id,
                FlashcardBaseId = flashcardBase2.Id,
                CollectionId = collection.Id,
                WordOrPhrase = wordOrPhrase,
                Meanings = new List<AddOrUpdateMeaningDto>
                {
                    new AddOrUpdateMeaningDto{ Id = meaning3.Id, Value = newMeaning }
                },
            }
        };

        var createdOn = _dateTimeProvider.UtcNow;

        _now = _now.AddDays(2);

        var expectedTime = _dateTimeProvider.UtcNow;

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var flashcardBase = await _flashcardBaseRepository.GetByIdAsync(flashcardBase1.Id);
        flashcardBase.Should().BeNull();

        var flashcard = await _flashcardRepository.GetByIdAsync(flashcard1.Id);
        flashcard.Should().NotBeNull();
        flashcard.FlashcardBaseId.Should().Be(flashcardBase2.Id);
    }
}