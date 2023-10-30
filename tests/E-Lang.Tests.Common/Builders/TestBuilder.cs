using E_Lang.Application.Common.Interfaces;
using E_Lang.Tests.Common.Interfaces;

namespace E_Lang.Tests.Common.Builders
{
    public class TestBuilder : ITestBuilder
    {
        private readonly IAppDbContext _context;

        public TestBuilder(IAppDbContext context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
