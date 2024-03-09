using E_Lang.Domain.Models;

namespace E_Lang.Domain.Entities
{
    public class InProgressFlashcardState : FlashcardState
    {
        private const int SEE_AGAIN_IN_MINUTES = 1;
        
        public Guid? CurrentQuizTypeId { get; set; }
        public QuizType? CurrentQuizType { get; set; }

        public DateTime? ShowAgainOn { get; set; }

        public ICollection<SeenQuizType> SeenQuizTypes { get; set; }
        
        public ICollection<CompletedQuizType> CompletedQuizTypes { get; set; }

        public ICollection<ExcludedQuizType> ExcludedQuizTypes { get; set; }

        public InProgressFlashcardState()
        {
            SeenQuizTypes = new List<SeenQuizType>();
            CompletedQuizTypes = new List<CompletedQuizType>();
            ExcludedQuizTypes = new List<ExcludedQuizType>();
        }

        public InProgressFlashcardState(Flashcard flashcard) : base(flashcard)
        {
            SeenQuizTypes = new List<SeenQuizType>();
            CompletedQuizTypes = new List<CompletedQuizType>();
            ExcludedQuizTypes = new List<ExcludedQuizType>();
        }

        public InProgressFlashcardState(FlashcardState flashcardState, DateTime utcNow)
        {
            Id = flashcardState.Id;
            CreatedOn = flashcardState.CreatedOn;
            ModifiedOn = flashcardState.ModifiedOn;
            FlashcardId = flashcardState.FlashcardId;
            Flashcard = flashcardState.Flashcard;
            Flashcard.LastSeenOn = utcNow;
            Flashcard.LastStatusChangedOn = utcNow;
            SeenQuizTypes = new List<SeenQuizType>();
            CompletedQuizTypes = new List<CompletedQuizType>();
            ExcludedQuizTypes = new List<ExcludedQuizType>();
        }

        public InProgressFlashcardState(FlashcardState flashcardState, NextStateData data)
            : this(flashcardState, data.UtcNow)
        {
            SeenQuizTypes = new List<SeenQuizType>();
            CompletedQuizTypes = new List<CompletedQuizType>();

            if (data.IsAnswerCorrect.HasValue && !data.IsAnswerCorrect.Value)
            {
                ShowAgainOn = data.UtcNow.AddMinutes(SEE_AGAIN_IN_MINUTES);
            }

            if (data.Attempt is not null)
            {
                SetInitQuizType(data.Attempt, data.IsAnswerCorrect);
            }
        }

        public override FlashcardState GetNextState(NextStateData data)
        {
            if (!data.IsAnswerCorrect.HasValue)
                throw new NullReferenceException("A flashcard in progress needs answer to get to the next stage.");

            Flashcard.LastSeenOn = data.UtcNow;

            if (!data.IsAnswerCorrect.Value)
            {
                ShowAgainOn = data.UtcNow.AddMinutes(SEE_AGAIN_IN_MINUTES);
            }

            if (CurrentQuizType is not null && data.IsAnswerCorrect.Value 
                && CompletedQuizTypes.All(q => q.QuizTypeId != CurrentQuizTypeId))
            {
                var quizType = new CompletedQuizType(Id, CurrentQuizType.Id);
                CompletedQuizTypes.Add(quizType);
            }

            if (CurrentQuizType is not null && SeenQuizTypes.All(q => q.QuizTypeId != CurrentQuizTypeId))
            {
                var quizType = new SeenQuizType(Id, CurrentQuizType.Id);
                SeenQuizTypes.Add(quizType);
            }

            SetNewCurrentQuiz(data.Attempt.QuizTypes, data.Attempt.MaxQuizTypesPerFlashcard);

            if (data.Attempt is not null && IsCompleted(data.Attempt))
                return new CompletedFlashcardState(this, data.UtcNow);

            return this;
        }

        public override QuizType GetQuiz(Attempt attempt)
        {
            return CurrentQuizType;
        }


        public bool IsCompleted(Attempt attempt)
        {
            var quizTypesNumber = attempt.QuizTypes?.Count() ?? 1;
            var seenQuizzesNumber = SeenQuizTypes.Count;
            var completedQuizzesNumber = CompletedQuizTypes.Count;
            var excludedQuizzesNumber = ExcludedQuizTypes.Count;
            var correctInPreCent = (int)((float)completedQuizzesNumber / seenQuizzesNumber * 100);

            return (seenQuizzesNumber + excludedQuizzesNumber == quizTypesNumber || seenQuizzesNumber == attempt.MaxQuizTypesPerFlashcard)
                   && correctInPreCent >= attempt.MinCompletedQuizzesPerCent;
        }

        private void SetInitQuizType(Attempt attempt, bool? isAnswerCorrect)
        {
            var quiz = attempt.QuizTypes?.SingleOrDefault(x => x.IsFirst);

            if (quiz is not null)
            {
                SeenQuizTypes.Add(new SeenQuizType(Id, quiz.Id));
            }

            if (quiz is not null && isAnswerCorrect.HasValue && isAnswerCorrect.Value)
            {
                CompletedQuizTypes.Add(new CompletedQuizType(Id, quiz.Id));
            }

            ExcludeQuizzes(attempt);

            SetNewCurrentQuiz(attempt.QuizTypes, attempt.MaxQuizTypesPerFlashcard);
        }

        private void SetNewCurrentQuiz(IEnumerable<QuizType> quizzes, int maxQuizzesPerFlashcard)
        {
            IEnumerable<QuizType> availableQuizzes;
            var completedQuizIds = CompletedQuizTypes.Select(x => x.QuizTypeId).ToHashSet();
            var incompleteQuizzes = quizzes.Where(x => !completedQuizIds.Contains(x.Id));

            if (SeenQuizTypes.Count == maxQuizzesPerFlashcard)
            {
                var seenQuizzesIds = SeenQuizTypes.Select(x => x.QuizTypeId).ToHashSet();
                availableQuizzes = incompleteQuizzes.Where(x => seenQuizzesIds.Contains(x.Id));
            }
            else
            {
                var excludedQuizIds = ExcludedQuizTypes.Select(x => x.QuizTypeId).ToHashSet();
                availableQuizzes = incompleteQuizzes.Where(x => !excludedQuizIds.Contains(x.Id));
            }

            var quiz = availableQuizzes?.FirstOrDefault(x => x.IsFirst)
                       ?? availableQuizzes?.Where(x => x.IsDefault).MinBy(x => Guid.NewGuid())
                       ?? availableQuizzes?.MinBy(x => Guid.NewGuid());

            CurrentQuizTypeId = quiz?.Id;
            CurrentQuizType = quiz;
        }

        private void ExcludeQuizzes(Attempt attempt)
        {
            ExcludeSingleSelectQuizzes(attempt);
            ExcludeMultiselectQuizType(attempt);
            ExcludeMultiWordQuizzes(attempt);
        }

        private void ExcludeSingleSelectQuizzes(Attempt attempt)
        {
            var meanings = Flashcard?.FlashcardBase?.Meanings;

            if (meanings is null || meanings.Count == 3)
                return;

            var quizzesToExclude = attempt.QuizTypes
                .Where(x => x is { IsSelect: true, IsSelectCorrect: false, MaxAnswersToSelect: 1 });

            var excludedQuizzes = ExcludedQuizTypes.ToList();
            excludedQuizzes.AddRange(quizzesToExclude.Select(x => new ExcludedQuizType(Id, x.Id)));
            ExcludedQuizTypes = excludedQuizzes;
        }

        private void ExcludeMultiselectQuizType(Attempt attempt)
        {
            var meanings = Flashcard?.FlashcardBase?.Meanings;

            if (meanings is null || meanings.Count > 1)
                return;

            var multiselectQuizzes = attempt.QuizTypes
                .Where(x => x is {IsSelect: true, IsSelectCorrect: true, MaxAnswersToSelect: > 1});

            var excludedQuizzes = ExcludedQuizTypes.ToList();
            excludedQuizzes.AddRange(multiselectQuizzes.Select(x => new ExcludedQuizType(Id, x.Id)));
            ExcludedQuizTypes = excludedQuizzes;
        }

        private void ExcludeMultiWordQuizzes(Attempt attempt)
        {
            if (Flashcard.FlashcardBase.WordOrPhrase.Split(' ').Length > 1)
                return;

            var multiWordQuizzes = attempt.QuizTypes
                .Where(x => x.IsArrange || x.IsFillInBlank)
                .Select(x => new ExcludedQuizType(Id, x.Id));
            
            var excludedQuizTypes = ExcludedQuizTypes.ToList();
            excludedQuizTypes.AddRange(multiWordQuizzes);
            ExcludedQuizTypes = excludedQuizTypes;
        }
    }
}
