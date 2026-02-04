using API.Data;
using API.Data.Repos;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPhotoService, PhotoService>();

        services.AddScoped<IStavbeRepository, StavbeRepository>();
        services.AddScoped<IMojElektroRepository, MojElektroRepository>();
        services.AddScoped<IMojElektroAgregiraniRepository, MojElektroAgregiraniRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));


        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        // services.AddEndpointsApiExplorer();
        // services.AddSwaggerGen();
        services.AddHttpClient<MojElektroApiService>();

        return services;

    }
}
