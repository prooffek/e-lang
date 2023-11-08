// See https://aka.ms/new-console-template for more information

using System.Reflection;
using E_Lang.Domain;
using E_Lang.Persistence;
using E_Lang.Seeder.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Seeder
{
    public class Program
    {
        public static async Task Main()
        {
            try
            {
                await Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = 1;
                throw;
            }
        }

        private static async Task Execute()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddScoped<ISeederRunner, SeederRunner>()
                .AddDomain()
                .AddPersistence(config.GetConnectionString(AppDbContextFactory.CONNECTION_STRING));

            using (var serviceProvider = services.BuildServiceProvider())
            {
                await serviceProvider
                    .GetRequiredService<ISeederRunner>()
                    .Run();
            }
        }
    }
    
}