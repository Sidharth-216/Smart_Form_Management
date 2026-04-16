using Application.Contracts;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));
        services.Configure<StorageSettings>(configuration.GetSection("Storage"));
        services.Configure<AiSettings>(configuration.GetSection("Ai"));

        services.AddSingleton<MongoDbContext>();
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, DistributedCacheService>();
        services.AddSingleton<IFormRepository, FormRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUploadRepository, UploadRepository>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddHttpClient<IFileStorageService, FileStorageService>();
        services.AddHttpClient<IAiClient, AiClient>();

        return services;
    }
}
