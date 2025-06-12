using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Mazina_Backend.Data;

public class DailyLogChecker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EmailService _emailService;

    public DailyLogChecker(IServiceProvider serviceProvider, EmailService emailService)
    {
        _serviceProvider = serviceProvider;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 6, 30, 0);

            if (now > nextRun)
                nextRun = nextRun.AddDays(1); // Ertesi güne kaydır

            var delay = nextRun - now;
            await Task.Delay(delay, stoppingToken); // Belirtilen saate kadar bekle

            await CheckLogsAndSendEmail();
        }
    }

    private async Task CheckLogsAndSendEmail()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();

            var today = DateTime.Today;
            var logEntry = await context.SPLogs
                .Where(l => l.ExecutionTime >= today && l.ExecutionTime < today.AddDays(1))
                .OrderByDescending(l => l.ExecutionTime)
                .FirstOrDefaultAsync();

            string recipientEmail = "admin@example.com"; // Alıcı maili
            string subject = "";
            string body = "";

            if (logEntry != null)
            {
                if (logEntry.Status == "Success")
                {
                    subject = "🔔 Başarılı Çalışma Raporu";

                    // ✅ Güncellenen Ürünleri Çek
                    var updatedProducts = await context.DailyProductsUpdated
                        .Where(p => p.Date >= today && p.Date < today.AddDays(1))
                        .ToListAsync();

                    // ✅ Yeni Eklenen Ürünleri Çek
                    var insertedProducts = await context.DailyProductsInserted
                        .Where(p => p.Date >= today && p.Date < today.AddDays(1))
                        .ToListAsync();

                    // 📝 Body içeriğini oluştur
                    body = $"Durum: {logEntry.Status}\n\n";

                    if (updatedProducts.Any())
                    {
                        body += "📌 **Bugün Güncellenen Ürünler:**\n";
                        foreach (var product in updatedProducts)
                        {
                            body += $"- {product.Name}: {product.OldPrice} ₺ → {product.NewPrice} ₺\n";
                        }
                    }
                    else
                    {
                        body += "✅ Bugün güncellenen ürün bulunmamaktadır.\n";
                    }

                    body += "\n";

                    if (insertedProducts.Any())
                    {
                        body += "📌 **Bugün Eklenen Ürünler:**\n";
                        foreach (var product in insertedProducts)
                        {
                            body += $"- {product.Name} ({product.Price} ₺)\n";
                        }
                    }
                    else
                    {
                        body += "✅ Bugün eklenen yeni ürün bulunmamaktadır.\n";
                    }
                }
                else
                {
                    subject = "⚠️ Hata Raporu";
                    body = "NarPosSisteminde aynı ID'ye sahip birden fazla kayıt var. Lütfen kontrol ediniz!\n\n" +
                           $"Durum: {logEntry.Status}\nMesaj: {logEntry.Message}";
                }

                await _emailService.SendEmailAsync(recipientEmail, subject, body);
            }
        }
    }

}
