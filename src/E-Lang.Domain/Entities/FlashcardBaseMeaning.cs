namespace E_Lang.Domain.Entities
{
    public class FlashcardBaseMeaning : EntityBase
    {
        public Guid FlashcardBaseId { get; set; }
        public Guid MeaningId { get; set; }
    }
}
