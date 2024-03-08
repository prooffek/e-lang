

namespace E_Lang.Seeder.Seeders
{
    public class QuizTypeSeeder : SeederBase
    {
        private readonly Guid _userId;

        public QuizTypeSeeder(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _userId = _userService.GetUserId();
        }

        public override async Task Seed()
        {
            await SeedQuizTypes();
        }

        public async Task SeedQuizTypes()
        {
            await Builder
                .AddQuizType(_userId, out var _)
                    .SetName("Select_correct.")
                    .SetInstruction("Select correct answer. Only one answer is correct.")
                    .SetIsSingleSelect()
                    .SetIsFirst()
                    .Build()
                .AddQuizType(_userId, out var _)
                    .SetName("Multiselect_correct.")
                    .SetInstruction("Select correct answers. At least one answer is incorrect.")
                    .SetIsMultiselect()
                    .Build()
                .AddQuizType(_userId, out var _)
                    .SetName("SelectMissing_correct.")
                    .SetInstruction("Select the missing word or phrase.")
                    .SetIsSingleSelect()
                    .Build()
                .AddQuizType(_userId, out var _)
                    .SetName("Select_incorrect.")
                    .SetInstruction("Select incorrect answer. Only one answer is correct.")
                    .SetIsSingleSelect(false)
                    .Build()
                .AddQuizType(_userId, out var _)
                    .SetName("MatchWithMeaning_correct.")
                    .SetInstruction("Match words/phrases with their meanings.")
                    .SetIsMatch()
                    .Build()
                .AddQuizType(_userId, out var _)
                    .SetName("ArrangeWords.")
                    .SetInstruction("Put words in the correct order to create the correct phrase.")
                    .SetIsArrange()
                    .Build()
                .AddQuizType(_userId, out var _)
                    .SetName("Input.")
                    .SetInstruction("Enter the correct answer.")
                    .SetIsInput()
                    .Build()
                .SaveAsync();
        }
    }
}
