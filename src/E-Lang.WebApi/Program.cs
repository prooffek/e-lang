using E_Lang.Application;
using E_Lang.Domain;
using E_Lang.Infrastructure;
using E_Lang.Persistence;
using E_Lang.WebApi.Middleware;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddApplication();
services.AddDomain();
services.AddPersistence(config.GetConnectionString(AppDbContextFactory.CONNECTION_STRING));
services.AddInfrastructure();
services.AddEndpointsApiExplorer();
services.AddSwaggerDocument(c => c.Title = "E-Lang WebApi");
services.AddSwaggerGen();
services.AddControllers()
    .AddOData((options) => options.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100));
services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUi3(options =>
{
    options.DocExpansion = "list";
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(policy =>
    policy.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(config.GetSection("UiUrls").Get<string[]>()
                     ?? new string[] {"http://localhost:4200"}));

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


namespace E_Lang.WebApi
{
    public partial class Program { }
}
