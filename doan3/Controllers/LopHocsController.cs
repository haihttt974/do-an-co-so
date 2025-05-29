using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using System.Security.Claims;

namespace doan3.Controllers
{
    public class LopHocsController : Controller
    {
        private readonly DacsGplxContext _context;

        public LopHocsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: LopHocs
        //public async Task<IActionResult> Index()
        //{
        //    var dacsGplxContext = _context.LopHocs.Include(l => l.Giaovien).Include(l => l.Khoahoc);
        //    return View(await dacsGplxContext.ToListAsync());
        //}
        public async Task<IActionResult> Index()
        {
            // Get the logged-in user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Bạn cần đăng nhập để xem danh sách lớp học.");
            }

            // Find the user in _USER table and get their Referenceld (GiaovienId)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
            if (user == null || user.Referenceld == null)
            {
                return Unauthorized("Không tìm thấy thông tin giáo viên liên kết với tài khoản này.");
            }

            int giaovienId = user.Referenceld.Value;

            // Filter LopHoc by GiaovienId
            var dacsGplxContext = _context.LopHocs
                .Where(l => l.GiaovienId == giaovienId)
                .Include(l => l.Giaovien)
                .Include(l => l.Khoahoc);

            return View(await dacsGplxContext.ToListAsync());
        }
        // GET: LopHocs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHocs
                .Include(l => l.Giaovien)
                .Include(l => l.Khoahoc)
                .FirstOrDefaultAsync(m => m.LopId == id);
            if (lopHoc == null)
            {
                return NotFound();
            }

            return View(lopHoc);
        }

        // GET: LopHocs/Create
        public IActionResult Create()
        {
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId");
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId");
            return View();
        }

        // POST: LopHocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LopId,Tenlop,LoaiLop,KhoahocId,GiaovienId")] LopHoc lopHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lopHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lopHoc.GiaovienId);
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId", lopHoc.KhoahocId);
            return View(lopHoc);
        }

        // GET: LopHocs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc == null)
            {
                return NotFound();
            }
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lopHoc.GiaovienId);
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId", lopHoc.KhoahocId);
            return View(lopHoc);
        }

        // POST: LopHocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LopId,Tenlop,LoaiLop,KhoahocId,GiaovienId")] LopHoc lopHoc)
        {
            if (id != lopHoc.LopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lopHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.LopHocs.AnyAsync(e => e.LopId == lopHoc.LopId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lopHoc.GiaovienId);
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId", lopHoc.KhoahocId);
            return View(lopHoc);
        }

        // GET: LopHocs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHocs
                .Include(l => l.Giaovien)
                .Include(l => l.Khoahoc)
                .FirstOrDefaultAsync(m => m.LopId == id);
            if (lopHoc == null)
            {
                return NotFound();
            }

            return View(lopHoc);
        }

        // POST: LopHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc != null)
            {
                _context.LopHocs.Remove(lopHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ThongTinHocVien(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy danh sách hồ sơ thí sinh liên quan đến lớp học thông qua KET_QUA_HOC_TAP
            var hoSoThiSinhs = await _context.KetQuaHocTaps
                .Where(kq => kq.LopId == id)
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hang)
                .Select(kq => kq.Hoso)
                .ToListAsync();

            // Lấy thông tin lớp học để hiển thị tiêu đề
            var lopHoc = await _context.LopHocs
                .Include(l => l.Khoahoc)
                .Include(l => l.Giaovien)
                .FirstOrDefaultAsync(m => m.LopId == id);

            if (lopHoc == null)
            {
                return NotFound("Lớp học không tồn tại.");
            }

            ViewData["LopHoc"] = lopHoc;

            // Chỉ đặt thông báo nếu không có hồ sơ
            if (hoSoThiSinhs == null || !hoSoThiSinhs.Any())
            {
                ViewData["Message"] = "Không tìm thấy hồ sơ thí sinh nào cho lớp học này.";
            }
            else
            {
                // Đảm bảo ViewData["Message"] không tồn tại khi có hồ sơ
                ViewData.Remove("Message");
            }

            return View(hoSoThiSinhs ?? new List<HoSoThiSinh>());
        }
    }
}
