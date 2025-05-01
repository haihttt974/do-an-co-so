using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using doan3.Models; // namespace DbContext của bạn
using System.Linq;

public class KhoaHocStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KhoaHocStatusUpdater> _logger;

    public KhoaHocStatusUpdater(IServiceProvider serviceProvider, ILogger<KhoaHocStatusUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DacsGplxContext>();

                var today = DateOnly.FromDateTime(DateTime.Today);
                var dsKhoaHoc = context.KhoaHocs.ToList();

                foreach (var kh in dsKhoaHoc)
                {
                    if (today < kh.Ngaybatdau)
                        kh.Trangthai = "Sắp mở";
                    else if (today >= kh.Ngaybatdau && today <= kh.Ngayketthuc)
                        kh.Trangthai = "Đang diễn ra";
                    else if (today > kh.Ngayketthuc)
                        kh.Trangthai = "Đã kết thúc";
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Cập nhật trạng thái khóa học xong lúc {time}", DateTime.Now);
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

}
