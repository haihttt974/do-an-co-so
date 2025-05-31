using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using Microsoft.AspNetCore.Authorization; // Để kiểm tra đăng nhập
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering; // Để lấy thông tin người dùng

namespace doan3.Controllers
{
    [Authorize] // Yêu cầu người dùng phải đăng nhập
    public class LichTapLaisController : Controller
    {
        private readonly DacsGplxContext _context;

        public LichTapLaisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: LichTapLais
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng hiện tại
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return Unauthorized(); // Người dùng không hợp lệ
            }

            IQueryable<LichTapLai> lichTapLais = _context.LichTapLais
                .Include(l => l.Giaovien)
                .Include(l => l.Hoso)
                    .ThenInclude(h => h.Hocvien)
                .Include(l => l.Xe);

            // Nếu người dùng có vai trò "user" (_ROLE_ID = 3)
            if (user.RoleId == 3 && user.Referenceld.HasValue)
            {
                // Lọc lịch tập lái theo HOCVIEN_ID từ REFERENCELD
                lichTapLais = lichTapLais
                    .Where(l => l.Hoso.HocvienId == user.Referenceld.Value)
                    .OrderBy(l => l.Tgbatdau); // Sắp xếp theo thời gian bắt đầu
            }
            else
            {
                // Nếu không phải user (ví dụ: admin), hiển thị tất cả lịch
                lichTapLais = lichTapLais.OrderBy(l => l.Hoso.Hocvien.Tenhocvien);
            }

            return View(await lichTapLais.ToListAsync());
        }

        // GET: LichTapLais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichTapLai = await _context.LichTapLais
                .Include(l => l.Giaovien)
                .Include(l => l.Hoso)
                    .ThenInclude(h => h.Hocvien)
                .Include(l => l.Xe)
                .FirstOrDefaultAsync(m => m.LichId == id);

            if (lichTapLai == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập (chỉ user liên quan hoặc admin)
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user.RoleId == 3 && user.Referenceld.HasValue && lichTapLai.Hoso.HocvienId != user.Referenceld.Value)
            {
                return Unauthorized(); // Người dùng không có quyền xem lịch này
            }

            return View(lichTapLai);
        }

        // GET: LichTapLais/Create
        //[Authorize(Roles = "Admin")] // Chỉ admin được tạo lịch
        [Authorize]
        public async Task<IActionResult> Create()
        {
            // Lấy thông tin người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId.ToString() == userId);

            if (user == null)
            {
                return Unauthorized("Bạn chưa đăng nhập.");
            }

            if (user.RoleId == 3) // Học viên
            {
                if (user.Referenceld == null)
                {
                    return BadRequest("Tài khoản của bạn không liên kết với học viên nào.");
                }

                // Tìm hồ sơ thí sinh của học viên dựa trên _REFERENCELD
                var hoSo = await _context.HoSoThiSinhs
                    .Include(h => h.Hocvien)
                    .Include(h => h.Hang)
                    .FirstOrDefaultAsync(h => h.HocvienId == user.Referenceld);

                if (hoSo == null || hoSo.HangId == null)
                {
                    return BadRequest("Không tìm thấy hồ sơ thí sinh hoặc hạng GPLX cho học viên này.");
                }

                // Lấy tên hạng GPLX
                var tenHang = hoSo.Hang.Tenhang;

                // Lọc giáo viên theo hạng đào tạo
                var giaoViens = await _context.GiaoViens
                    .Where(g => g.HangDaotao.Contains(tenHang))
                    .ToListAsync();

                // Lọc xe tập lái theo loại xe
                var xeTapLais = await _context.XeTapLais
                    .Where(x => x.Loaixe.Contains(tenHang))
                    .ToListAsync();

                // Truyền dữ liệu cho view
                ViewData["GiaovienId"] = new SelectList(giaoViens, "GiaovienId", "Tengiaovien");
                ViewData["XeId"] = new SelectList(xeTapLais, "XeId", "Loaixe");
                ViewData["HosoId"] = hoSo.HosoId;
                ViewData["HocvienName"] = hoSo.Hocvien.Tenhocvien;
                ViewData["IsLearner"] = true;
            }
            else // Admin hoặc vai trò khác
            {
                // Lấy danh sách tất cả học viên, giáo viên, xe để admin chọn
                ViewData["HosoId"] = new SelectList(
                    _context.HoSoThiSinhs.Include(h => h.Hocvien),
                    "HosoId",
                    "Hocvien.Tenhocvien");
                ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "Tengiaovien");
                ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "Loaixe");
                ViewData["IsLearner"] = false;
            }

            return View();
        }

        // POST: LichTapLais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LichId,GiaovienId,HosoId,XeId,Tgbatdau,Tgketthuc,Diadiem,Noidung,Ghichu")] LichTapLai lichTapLai)
        {
            // Lấy thông tin người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId.ToString() == userId);

            if (user == null)
            {
                return Unauthorized("Bạn chưa đăng nhập.");
            }

            // Nếu là học viên, cố định HosoId từ _REFERENCELD
            if (user.RoleId == 3)
            {
                if (user.Referenceld == null)
                {
                    return BadRequest("Tài khoản của bạn không liên kết với học viên nào.");
                }

                var hoSo = await _context.HoSoThiSinhs
                    .FirstOrDefaultAsync(h => h.HocvienId == user.Referenceld);

                if (hoSo == null)
                {
                    return BadRequest("Không tìm thấy hồ sơ thí sinh cho học viên này.");
                }

                lichTapLai.HosoId = hoSo.HosoId; // Cố định HosoId
            }

            // Kiểm tra TGBATDAU < TGKETTHUC
            if (lichTapLai.Tgbatdau >= lichTapLai.Tgketthuc)
            {
                ModelState.AddModelError("Tgketthuc", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            }

            // Kiểm tra các trường bắt buộc
            if (lichTapLai.GiaovienId == null || lichTapLai.XeId == null || lichTapLai.Diadiem == null)
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ các trường bắt buộc.");
            }

            // Kiểm tra giáo viên và xe có phù hợp với hạng GPLX của học viên
            var hoSoSelected = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .FirstOrDefaultAsync(h => h.HosoId == lichTapLai.HosoId);

            if (hoSoSelected?.HangId != null)
            {
                var tenHang = hoSoSelected.Hang.Tenhang;
                var giaoVien = await _context.GiaoViens.FindAsync(lichTapLai.GiaovienId);
                var xe = await _context.XeTapLais.FindAsync(lichTapLai.XeId);

                if (giaoVien == null || !giaoVien.HangDaotao.Contains(tenHang))
                {
                    ModelState.AddModelError("GiaovienId", "Giáo viên không phù hợp với hạng GPLX của học viên.");
                }

                if (xe == null || !xe.Loaixe.Contains(tenHang))
                {
                    ModelState.AddModelError("XeId", "Xe tập lái không phù hợp với hạng GPLX của học viên.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(lichTapLai);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký lịch tập lái thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Trả lại dữ liệu cho dropdown và thông tin học viên
            if (user.RoleId == 3)
            {
                var hoSo = await _context.HoSoThiSinhs
                    .Include(h => h.Hocvien)
                    .Include(h => h.Hang)
                    .FirstOrDefaultAsync(h => h.HocvienId == user.Referenceld);

                if (hoSo?.HangId != null)
                {
                    var tenHang = hoSo.Hang.Tenhang;
                    ViewData["GiaovienId"] = new SelectList(
                        _context.GiaoViens.Where(g => g.HangDaotao.Contains(tenHang)),
                        "GiaovienId",
                        "Tengiaovien",
                        lichTapLai.GiaovienId);
                    ViewData["XeId"] = new SelectList(
                        _context.XeTapLais.Where(x => x.Loaixe.Contains(tenHang)),
                        "XeId",
                        "Loaixe",
                        lichTapLai.XeId);
                }
                ViewData["HosoId"] = hoSo?.HosoId;
                ViewData["HocvienName"] = hoSo?.Hocvien.Tenhocvien;
                ViewData["IsLearner"] = true;
            }
            else
            {
                ViewData["HosoId"] = new SelectList(
                    _context.HoSoThiSinhs.Include(h => h.Hocvien),
                    "HosoId",
                    "Hocvien.Tenhocvien",
                    lichTapLai.HosoId);
                ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "Tengiaovien", lichTapLai.GiaovienId);
                ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "Loaixe", lichTapLai.XeId);
                ViewData["IsLearner"] = false;
            }

            ViewData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin.";
            return View(lichTapLai);
        }

        // GET: LichTapLais/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichTapLai = await _context.LichTapLais.FindAsync(id);
            if (lichTapLai == null)
            {
                return NotFound();
            }

            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "Tengiaovien", lichTapLai.GiaovienId);
            ViewData["HosoId"] = new SelectList(
                _context.HoSoThiSinhs.Include(h => h.Hocvien),
                "HosoId",
                "Hocvien.Tenhocvien",
                lichTapLai.HosoId);
            ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "Loaixe", lichTapLai.XeId);
            return View(lichTapLai);
        }

        // POST: LichTapLais/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("LichId,GiaovienId,HosoId,XeId,Tgbatdau,Tgketthuc,Diadiem,Noidung,Ghichu")] LichTapLai lichTapLai)
        {
            if (id != lichTapLai.LichId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichTapLai);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichTapLaiExists(lichTapLai.LichId))
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

            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "Tengiaovien", lichTapLai.GiaovienId);
            ViewData["HosoId"] = new SelectList(
                _context.HoSoThiSinhs.Include(h => h.Hocvien),
                "HosoId",
                "Hocvien.Tenhocvien",
                lichTapLai.HosoId);
            ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "Loaixe", lichTapLai.XeId);
            return View(lichTapLai);
        }

        // GET: LichTapLais/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichTapLai = await _context.LichTapLais
                .Include(l => l.Giaovien)
                .Include(l => l.Hoso)
                    .ThenInclude(h => h.Hocvien)
                .Include(l => l.Xe)
                .FirstOrDefaultAsync(m => m.LichId == id);

            if (lichTapLai == null)
            {
                return NotFound();
            }

            return View(lichTapLai);
        }

        // POST: LichTapLais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichTapLai = await _context.LichTapLais.FindAsync(id);
            if (lichTapLai != null)
            {
                _context.LichTapLais.Remove(lichTapLai);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichTapLaiExists(int id)
        {
            return _context.LichTapLais.Any(e => e.LichId == id);
        }
        // Trong LichTapLaisController.cs
        [HttpGet]
        public async Task<IActionResult> GetGiaoVienXeByHoso(int hosoId)
        {
            var hoSo = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .FirstOrDefaultAsync(h => h.HosoId == hosoId);

            if (hoSo?.HangId == null)
            {
                return BadRequest("Không tìm thấy hạng GPLX cho học viên này.");
            }

            var tenHang = hoSo.Hang.Tenhang;

            var giaoViens = await _context.GiaoViens
                .Where(g => g.HangDaotao.Contains(tenHang))
                .Select(g => new { g.GiaovienId, g.Tengiaovien })
                .ToListAsync();

            var xeTapLais = await _context.XeTapLais
                .Where(x => x.Loaixe.Contains(tenHang))
                .Select(x => new { x.XeId, x.Loaixe })
                .ToListAsync();

            return Json(new { giaoViens, xeTapLais });
        }
    }
}