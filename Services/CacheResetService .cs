namespace Mazina_Backend.Services;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CacheResetService : BackgroundService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheResetService> _logger;

    public CacheResetService(IMemoryCache cache, ILogger<CacheResetService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow.AddHours(3); // Türkiye saati (UTC+3)
            var targetTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0); // 06:00

            if (now > targetTime) // Eğer 06:00 geçmişse, bir sonraki günün 06:00'ına ayarla
            {
                targetTime = targetTime.AddDays(1);
            }

            var delay = targetTime - now;
            _logger.LogInformation($"Cache reset will happen in {delay.TotalHours} hours.");

            await Task.Delay(delay, stoppingToken);

            ClearProductCache();
        }
    }

    private void ClearProductCache()
    {
        _cache.Remove("all_products");
        _cache.Remove("some_other_cache_key"); // İstersen başka cache anahtarlarını da ekleyebilirsin
        _logger.LogInformation("Cache cleared at 06:00 AM.");
    }
}
