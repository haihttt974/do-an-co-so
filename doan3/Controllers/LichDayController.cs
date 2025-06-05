using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using doan3.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace doan3.Controllers
{
    [Authorize(Roles = "2")] // Nếu RoleId 2 là giáo viên
    public class LichDayController : Controller
    {
        private readonly DacsGplxContext _context;

        public LichDayController(DacsGplxContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy UserId từ claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Truy vấn user để lấy GiaovienId (từ Referenceld)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null || user.Referenceld == null)
            {
                return Unauthorized();
            }

            int giaovienId = user.Referenceld.Value;

            // Truy vấn lịch dạy của giáo viên
            var lichDay = await _context.LichHocs
                .Include(l => l.Lop)
                .Where(l => l.Lop != null && l.Lop.GiaovienId == giaovienId)
                .Select(l => new LichDayViewModel
                {
                    TenLop = l.Lop.Tenlop,
                    TgBatDau = l.TgBatdau,
                    TgKetThuc = l.TgKetthuc,
                    HinhThucHoc = l.Hinhthuchoc,
                    NoiDung = l.Noidung
                })
                .ToListAsync();

            return View(lichDay);
        }
    }
}
