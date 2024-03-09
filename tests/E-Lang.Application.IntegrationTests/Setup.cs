using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Builder.Builders;
using E_Lang.Persistence;
using E_Lang.Tests.Common.Mocks;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Application.IntegrationTests
{
    public abstract class Setup
    {
        protected static IConfiguration? _config;

        protected static IServiceScope? _contextScope;
        protected static IServiceScope? _applicationScope;

        protected static BaseBuilder _testBuilder;
        protected static AppDbContext _appDbContext;
        protected static IMediator _mediator;
        protected static IMapper _mapper;
        protected static IDateTimeProvider _dateTimeProvider;

        protected static ICollectionRepository _collectionRepository;
        protected static IFlashcardRepository _flashcardRepository;
        protected static IFlashcardBaseRepository _flashcardBaseRepository;
        protected static IMeaningRepository _meaningRepostory;
        protected static IAttemptRepository _attemptRepostory;
        protected static IAttemptStageRepository _attemptStageRepostory;
        protected static IFlashcardStateRepository _flashcardStateRepository;

        protected static IAttemptStageService _attemptStageService;

        protected static DateTime _now
        {
            get => _dateTimeProvider.Now;
            set => MockDateTimeProvider.MockNow = value;
        }

        public static void InitClass()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            _config = builder.Build();

            var apiFactory = new TestApiFactory(_config);
            var scopeFactory = apiFactory.Services.GetRequiredService<IServiceScopeFactory>();

            _contextScope = scopeFactory.CreateScope();
            _appDbContext = (AppDbContext)_contextScope.ServiceProvider.GetRequiredService<IAppDbContext>();
            
            _testBuilder = new BaseBuilder(_appDbContext, MockDateTimeProvider.GetInstance());

            _applicationScope = scopeFactory.CreateScope();
            _mediator = _applicationScope.ServiceProvider.GetRequiredService<IMediator>();
            _mapper = _applicationScope.ServiceProvider.GetRequiredService<IMapper>();
            _dateTimeProvider = _applicationScope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
            
            _collectionRepository = _applicationScope.ServiceProvider.GetRequiredService<ICollectionRepository>();
            _flashcardRepository = _applicationScope.ServiceProvider.GetRequiredService<IFlashcardRepository>();
            _flashcardBaseRepository = _applicationScope.ServiceProvider.GetRequiredService<IFlashcardBaseRepository>();
            _meaningRepostory = _applicationScope.ServiceProvider.GetRequiredService<IMeaningRepository>();
            _attemptRepostory = _applicationScope.ServiceProvider.GetRequiredService<IAttemptRepository>();
            _attemptStageRepostory = _applicationScope.ServiceProvider.GetRequiredService<IAttemptStageRepository>();
            _flashcardStateRepository = _applicationScope.ServiceProvider.GetRequiredService<IFlashcardStateRepository>();

            _attemptStageService = _applicationScope.ServiceProvider.GetRequiredService<IAttemptStageService>();
        }

        public static void InitTest()
        {
            _appDbContext.Database.EnsureDeleted();
            _appDbContext.Database.Migrate();
        }

        public static void CleanupTest()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            EnsureDataBase();
        }

        private static void EnsureDataBase()
        {
            if (_appDbContext is null)
            {
                throw new NullReferenceException(nameof(AppDbContext));
            }

            _appDbContext?.ChangeTracker.Clear();
            _appDbContext.Database.EnsureDeleted();
        }
    }
}
