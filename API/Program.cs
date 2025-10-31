using System.Text;
using API.Data;
using API.Data.SeedData;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Humanizer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration); // This is the extension method from API/Extensions/ApplicationServiceExtensions.cs
builder.Services.AddIdentityServices(builder.Configuration); // This is the extension method from API/Extensions/IdentityServiceExtensions.cs
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>(); // This is the middleware from API/Middleware/ExceptionMiddleware.cs
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    // app.UseSwaggerUI();
    // app.UseSwagger();
}

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager);
    // await SeedStavbe.Seed(context);
    // await SeedMerilnaMesta.ImportMM(context);
    // await SeedMojElektroMMesta.ImportMM(context);

    // await SeedGeoTocke.ImportGeo(context);
    // await SeedMojElektro15minMeritve.Import15minMeritve(context);
    // await SeedOdcitki.ImportElektro(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
