using E_Lang.Application;
using E_Lang.Domain;
using E_Lang.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddApplication();
services.AddDomain();
services.AddPersistence(config.GetConnectionString(AppDbContextFactory.CONNECTION_STRING));
services.AddEndpointsApiExplorer();
services.AddSwaggerDocument(c => c.Title = "E-Lang WebApi");
services.AddSwaggerGen();
services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUi3();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


namespace E_Lang.WebApi
{
    public partial class Program { }
}
