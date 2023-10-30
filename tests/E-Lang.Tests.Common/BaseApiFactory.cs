using E_Lang.Application.Common.Interfaces;
using E_Lang.Persistence;
using E_Lang.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace E_Lang.Tests.Common
{
    public class BaseApiFactory : WebApplicationFactory<Program>
    {
        private const string DATABASE_NAME = "test_Db";
        private readonly IConfiguration _config;

        public BaseApiFactory(IConfiguration config)
        {
            _config = config;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer().ConfigureTestServices(MockServices);
        }

        protected virtual void MockServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                var optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseInMemoryDatabase(DATABASE_NAME);
                var context = new AppDbContext(optionsBuilder.Options);
                return context;
            }));

            services.Replace(ServiceDescriptor.Scoped<IAppDbContext>(serviceProvider =>
                serviceProvider.GetRequiredService<AppDbContext>()
                    ?? throw new InvalidOperationException($"Service {nameof(AppDbContext)} not found.")));
        }
    }
}
