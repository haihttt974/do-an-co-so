using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using System;

namespace doan3.Controllers
{
    public class StudentLichHocController : Controller
    {
        private readonly DacsGplxContext _context;

        public StudentLichHocController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: StudentLichHoc
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Bạn cần đăng nhập để xem lịch học.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 3);
            if (user == null || user.Referenceld == null)
            {
                return Unauthorized("Chỉ học viên được phép xem lịch học.");
            }

            var hosoIds = await _context.HoSoThiSinhs
                .Where(hs => hs.HocvienId == user.Referenceld)
                .Select(hs => hs.HosoId)
                .ToListAsync();

            // Check if any record has "Chưa thanh toán khóa học"
            bool hasUnpaid = await _context.KetQuaHocTaps
                .AnyAsync(kq => hosoIds.Contains(kq.HosoId)
                                && kq.Nhanxet == "Chưa thanh toán khóa học");

            if (hasUnpaid)
            {
                TempData["ErrorMessage"] = "Bạn cần thanh toán khóa học trước khi xem lịch học.";
                return RedirectToAction("PaymentRequired"); // hoặc trả về view thông báo
            }

            var enrolledClasses = await _context.KetQuaHocTaps
                .Where(kq => hosoIds.Contains(kq.HosoId) && kq.LopId != null)
                .Select(kq => kq.LopId.Value)
                .Distinct()
                .ToListAsync();

            var lichHocs = await _context.LichHocs
                .Include(l => l.Lop)
                .Where(l => enrolledClasses.Contains(l.LopId.Value))
                .OrderBy(l => l.TgBatdau)
                .ToListAsync();

            return View(lichHocs);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Bạn cần đăng nhập để xem chi tiết lịch học.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 3);
            if (user == null || user.Referenceld == null)
            {
                return Unauthorized("Chỉ học viên được phép xem chi tiết lịch học.");
            }

            var hosoIds = await _context.HoSoThiSinhs
                .Where(hs => hs.HocvienId == user.Referenceld)
                .Select(hs => hs.HosoId)
                .ToListAsync();

            var enrolledClasses = await _context.KetQuaHocTaps
                .Where(kq => hosoIds.Contains(kq.HosoId) && kq.LopId != null)
                .Select(kq => kq.LopId.Value)
                .Distinct()
                .ToListAsync();

            var lichHoc = await _context.LichHocs
                .Include(l => l.Lop)
                .FirstOrDefaultAsync(m => m.LichhocId == id && enrolledClasses.Contains(m.LopId.Value));
            if (lichHoc == null)
            {
                return NotFound();
            }

            return View(lichHoc);
        }
        public IActionResult PaymentRequired()
        {
            return View();
        }

    }
}