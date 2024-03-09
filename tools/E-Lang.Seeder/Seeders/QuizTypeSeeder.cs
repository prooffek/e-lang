

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
                    .SetName("Select_correct.")
                    .SetInstruction("Select incorrect answer. Only one answer is correct.")
                    .SetIsSingleSelect(false)
                    .Build()
                .AddQuizType(_userId, out var _)
                    .SetName("Multiselect_correct.")
                    .SetInstruction("Select incorrect answers. At least one answer is incorrect.")
                    .SetIsMultiselect(false)
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
