﻿using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Builder
{
    public static class Entities
    {
        public static User GetUser()
        {
            return new User()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                UserName = "Admin",
                Email = "test@test.com",
            };
        }
        
        public static Collection GetCollection(Guid userId)
        {
            return new Collection()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Name = "Test collection",
                ParentId = null,
                OwnerId = userId,
                Subcollections = new List<Collection>(),
                Flashcards = new List<Flashcard>()
            };
        }

        public static Meaning GetMeaning(Guid flashcardBaseId)
        {
            return new Meaning()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Value = "Test vaue",
                FlashcardBaseId = flashcardBaseId
            };
        }

        public static FlashcardBase GetFlashcardBase()
        {
            return new FlashcardBase()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                WordOrPhrase = "Test",
                Meanings = new List<Meaning>()
            };
        }

        public static Flashcard GetFlashcard(Guid collectionId, Guid ownerId)
        {
            return new Flashcard()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                CollectionId = collectionId,
                OwnerId = ownerId,
                Status = FlashcardStatus.Active,
                LastStatusChangedOn = DateTime.UtcNow,
                LastSeenOn = DateTime.UtcNow,
            };
        }

        public static Attempt GetAttempt(Collection collection)
        {
            return new Attempt()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                CollectionId = collection.Id,
                Collection = collection,
                Name = "Test attempt",
                MaxFlashcardsPerStage = 10,
                MaxQuizTypesPerFlashcard = 3,
                MinCompletedQuizzesPerCent = 100,
                Order = FlashcardOrder.Random,
                IncludeMeanings = true,
            };
        }

        public static AttemptStage GetAttemptStage()
        {
            return new AttemptStage()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Stage = AttemptStageType.Init,
                Flashcards = new List<FlashcardState>()
            };
        }

        public static QuizType GetQuizType() 
        {
            return new QuizType()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Name = "Test quiz",
                Instruction = "Chose the right answer",
                IsSelect = true,
                IsMultiselect = false,
                IsSelectCorrect = true,
                IsSelectMissing = false,
                IsMatch = false,
                IsArrange = false,
                IsInput = false,
                IsFillInBlank = false
            };
        }

        public static FlashcardState GetInitFlashcardState(Flashcard flashcard)
        {
            return new InitFlashcardState(flashcard)
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            };
        }
    }
}
