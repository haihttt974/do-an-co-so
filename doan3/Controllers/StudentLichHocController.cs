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
            // Get the logged-in user's ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Bạn cần đăng nhập để xem lịch học.");
            }

            // Verify user is a student (RoleId = 3) and get their HocVien ID
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 3);
            if (user == null || user.Referenceld == null)
            {
                return Unauthorized("Chỉ học viên được phép xem lịch học.");
            }

            // Get the student's profiles (HoSoThiSinh)
            var hosoIds = await _context.HoSoThiSinhs
                .Where(hs => hs.HocvienId == user.Referenceld)
                .Select(hs => hs.HosoId)
                .ToListAsync();

            // Get classes the student is enrolled in via KetQuaHocTap
            var enrolledClasses = await _context.KetQuaHocTaps
                .Where(kq => hosoIds.Contains(kq.HosoId) && kq.LopId != null)
                .Select(kq => kq.LopId.Value)
                .Distinct()
                .ToListAsync();

            // Get schedules for those classes
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
    }
}