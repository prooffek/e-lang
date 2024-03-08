using E_Lang.Domain.Models;

namespace E_Lang.Domain.Entities
{
    public class InProgressFlashcardState : FlashcardState
    {
        private const int SEE_AGAIN_IN_MINUTES = 1;

        private ICollection<SeenQuizType> _seenQuizTypes;
        
        public Guid? CurrentQuizTypeId { get; set; }
        public QuizType? CurrentQuizType { get; set; }

        public DateTime? ShowAgainOn { get; set; }

        public ICollection<SeenQuizType> SeenQuizTypes { get; set; }
        
        public ICollection<CompletedQuizType> CompletedQuizTypes { get; set; }

        public InProgressFlashcardState()
        {
            SeenQuizTypes = new List<SeenQuizType>();
            CompletedQuizTypes = new List<CompletedQuizType>();
        }

        public InProgressFlashcardState(Flashcard flashcard) : base(flashcard)
        {
            SeenQuizTypes = new List<SeenQuizType>();
            CompletedQuizTypes = new List<CompletedQuizType>();
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

            SetNewCurrentQuiz(data.Attempt.QuizTypes);

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
            var seenQuizzes = SeenQuizTypes.Count;
            var completedQuizzes = CompletedQuizTypes.Count;
            var correctInPreCent = (int)((float)completedQuizzes / seenQuizzes * 100);

            return seenQuizzes == quizTypesNumber && correctInPreCent >= attempt.MinCompletedQuizzesPerCent;
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

            CompleteMultiselectQuizType(attempt);

            SetNewCurrentQuiz(attempt.QuizTypes);
        }

        private void SetNewCurrentQuiz(IEnumerable<QuizType> quizzes)
        {
            var completedQuizIds = CompletedQuizTypes.Select(x => x.QuizTypeId).ToHashSet();
            var availableQuizzes = quizzes.Where(x => !completedQuizIds.Contains(x.Id) && (x.MaxAnswersToSelect == 1 || Flashcard.FlashcardBase.Meanings.Count > 1));


            var quiz = availableQuizzes?.FirstOrDefault(x => x.IsFirst)
                       ?? availableQuizzes?.FirstOrDefault(x => x.IsDefault)
                       ?? availableQuizzes?.MinBy(x => Guid.NewGuid());

            CurrentQuizTypeId = quiz?.Id;
            CurrentQuizType = quiz;
        }

        private void CompleteMultiselectQuizType(Attempt attempt)
        {
            var meanings = Flashcard?.FlashcardBase?.Meanings;

            if (meanings is null)
                return;

            if (meanings.Count == 1)
            {
                var multiselectQuizzes = attempt.QuizTypes
                    .Where(x => x is {IsSelectCorrect: true, MaxAnswersToSelect: > 1});

                CompleteMultiselectQuizzes(multiselectQuizzes);
                SetMultiselectQuizzesAsSeen(multiselectQuizzes);
            }
        }

        private void CompleteMultiselectQuizzes(IEnumerable<QuizType> multiselectQuizzes)
        {
            var completedQuizzes = CompletedQuizTypes.ToList();
            completedQuizzes.AddRange(multiselectQuizzes.Select(x => new CompletedQuizType(Id, x.Id)));
            CompletedQuizTypes = completedQuizzes;
        }

        private void SetMultiselectQuizzesAsSeen(IEnumerable<QuizType> multiselectQuizzes)
        {
            var seenQuizzes = SeenQuizTypes.ToList();
            seenQuizzes.AddRange(multiselectQuizzes.Select(x => new SeenQuizType(Id, x.Id)));
            SeenQuizTypes = seenQuizzes;
        }
    }
}
