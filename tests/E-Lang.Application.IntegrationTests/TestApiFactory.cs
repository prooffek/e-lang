using E_Lang.Tests.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Application.IntegrationTests
{
    public class TestApiFactory : BaseApiFactory
    {
        public TestApiFactory(IConfiguration config) : base(config)
        {
        }

        protected override void MockServices(IServiceCollection services)
        {
            base.MockServices(services);
        }
    }
}
