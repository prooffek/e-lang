using E_Lang.Application.Common.Interfaces;
using E_Lang.Builder.Builders;
using E_Lang.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Application.IntegrationTests
{
    public abstract class Setup
    {
        private static bool _isInitialised = false;

        protected static IConfiguration? _config;

        protected static IServiceScope? _contextScope;
        protected static IServiceScope? _applicationScope;

        protected static BaseBuilder _testBuilder;
        protected static AppDbContext _appDbContext;
        protected static IMediator _mediator;

        public static void InitClass()
        {
            if (_isInitialised) return;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            _config = builder.Build();

            var apiFactory = new TestApiFactory(_config);
            var scopeFactory = apiFactory.Services.GetRequiredService<IServiceScopeFactory>();

            _contextScope = scopeFactory.CreateScope();
            _appDbContext = (AppDbContext)_contextScope.ServiceProvider.GetRequiredService<IAppDbContext>();

            _testBuilder = new BaseBuilder(_appDbContext);

            _applicationScope = scopeFactory.CreateScope();
            _mediator = _applicationScope.ServiceProvider.GetRequiredService<IMediator>();

            _isInitialised = true;
        }

        public static void InitTest()
        {
        }

        public static void CleanupTest()
        {
            _appDbContext?.ChangeTracker.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void EnsureDataBase()
        {
            if (_appDbContext is null)
            {
                throw new NullReferenceException(nameof(AppDbContext));
            }

            _appDbContext.Database.EnsureDeleted();
            _appDbContext.Database.Migrate();
        }
    }
}
