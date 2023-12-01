using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.DTOs
{
    public class MeaningDto : IMapper<Meaning>
    {
        public string Value { get; set; }
    }
}
