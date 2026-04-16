using System.Text.Json;
using Application.Contracts;
using Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Services;

public sealed class DistributedCacheService : ICacheService, IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConnectionMultiplexer? _redis;
    private readonly IDatabase? _database;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public DistributedCacheService(IMemoryCache memoryCache, IOptions<RedisSettings> redisSettings)
    {
        _memoryCache = memoryCache;
        var settings = redisSettings.Value;
        if (!settings.Enabled)
        {
            return;
        }

        try
        {
            _redis = ConnectionMultiplexer.Connect(settings.ConnectionString);
            _database = _redis.GetDatabase();
        }
        catch
        {
            _redis = null;
            _database = null;
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_database is not null)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
            }
        }

        return _memoryCache.TryGetValue(key, out T? cached) ? cached : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        if (_database is not null)
        {
            await _database.StringSetAsync(key, JsonSerializer.Serialize(value, _jsonOptions), ttl);
        }
        else
        {
            _memoryCache.Set(key, value, ttl);
        }
    }

    public async Task<long> IncrementAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        if (_database is not null)
        {
            var count = await _database.StringIncrementAsync(key);
            await _database.KeyExpireAsync(key, ttl);
            return count;
        }

        var current = _memoryCache.TryGetValue(key, out long countValue) ? countValue + 1 : 1;
        _memoryCache.Set(key, current, ttl);
        return current;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_database is not null)
        {
            return _database.KeyDeleteAsync(key);
        }

        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public void Dispose() => _redis?.Dispose();
}
