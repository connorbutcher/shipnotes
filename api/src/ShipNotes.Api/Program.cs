using ShipNotes.Api.Extensions;
using ShipNotes.Application;
using ShipNotes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.MigrateDatabaseAsync();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

/// <summary>Entry point; exposed for integration tests.</summary>
public partial class Program;
