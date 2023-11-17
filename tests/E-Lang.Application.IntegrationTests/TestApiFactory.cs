using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Tests.Common;
using E_Lang.Tests.Common.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            services.Replace(ServiceDescriptor.Transient<IUserService>(sp => MockUserService.GetInstance()));
            services.Replace(ServiceDescriptor.Transient<IDateTimeProvider>(sp => MockDateTimeProvider.GetInstance()));
        }
    }
}
