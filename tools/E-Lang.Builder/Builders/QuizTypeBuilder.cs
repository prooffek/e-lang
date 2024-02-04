using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders
{
    public class QuizTypeBuilder : EntityBuilderBase<QuizType, BaseBuilder>
    {
        public QuizTypeBuilder(QuizType entity, BaseBuilder parentBuilder, IAppDbContext context, IDateTimeProvider dateTimeProvider) 
            : base(entity, parentBuilder, context, dateTimeProvider)
        {
        }

        public QuizTypeBuilder SetName(string name)
        {
            _entity.Name = name;
            return this;
        }

        public QuizTypeBuilder SetInstruction(string instruction)
        {
            _entity.Instruction = instruction;
            return this;
        }

        public QuizTypeBuilder SetIsSingleSelect(bool isSelectCorrect = true)
        {
            _entity.IsSelect = true;
            _entity.MaxAnswersToSelect = 1;
            _entity.IsSelectCorrect = isSelectCorrect;
            _entity.IsSelectMissing = false;
            _entity.IsMatch = false;
            _entity.IsArrange = false;
            _entity.IsInput = false;
            _entity.IsFillInBlank = false;
            return this;
        }

        public QuizTypeBuilder SetIsMultiselect(bool isSelectCorrect = true, int answersNumber = 3)
        {
            _entity.IsSelect = true;
            _entity.MaxAnswersToSelect = answersNumber;
            _entity.IsSelectCorrect = isSelectCorrect;
            _entity.IsSelectMissing = false;
            _entity.IsMatch = false;
            _entity.IsArrange = false;
            _entity.IsInput = false;
            _entity.IsFillInBlank = false;
            return this;
        }

        public QuizTypeBuilder SetIsSelectMissing()
        {
            _entity.IsSelect = false;
            _entity.MaxAnswersToSelect = 1;
            _entity.IsSelectCorrect = true;
            _entity.IsSelectMissing = true;
            _entity.IsMatch = false;
            _entity.IsArrange = false;
            _entity.IsInput = false;
            _entity.IsFillInBlank = false;
            return this;
        }

        public QuizTypeBuilder SetIsMatch()
        {
            _entity.IsSelect = false;
            _entity.IsSelectCorrect = true;
            _entity.IsSelectMissing = false;
            _entity.IsMatch = true;
            _entity.IsArrange = false;
            _entity.IsInput = false;
            _entity.IsFillInBlank = false;
            return this;
        }

        public QuizTypeBuilder SetIsArrange()
        {
            _entity.IsSelect = false;
            _entity.IsSelectCorrect = true;
            _entity.IsSelectMissing = false;
            _entity.IsMatch = false;
            _entity.IsArrange = true;
            _entity.IsInput = false;
            _entity.IsFillInBlank = false;
            return this;
        }

        public QuizTypeBuilder SetIsInput()
        {
            _entity.IsSelect = false;
            _entity.IsSelectCorrect = true;
            _entity.IsSelectMissing = false;
            _entity.IsMatch = false;
            _entity.IsArrange = false;
            _entity.IsInput = true;
            _entity.IsFillInBlank = false;
            return this;
        }

        public QuizTypeBuilder SetIsFillInBlank()
        {
            _entity.IsSelect = false;
            _entity.IsSelectCorrect = true;
            _entity.IsSelectMissing = false;
            _entity.IsMatch = false;
            _entity.IsArrange = false;
            _entity.IsInput = false;
            _entity.IsFillInBlank = true;
            return this;
        }

        public QuizTypeBuilder SetIsFirst()
        {
            _entity.IsFirst = true;
            _entity.IsDefault = true;
            return this;
        }

        public QuizTypeBuilder SetIsDefault()
        {
            _entity.IsDefault = true;
            return this;
        }
    }
}
