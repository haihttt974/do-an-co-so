using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using doan3.ViewModel;
using System.Security.Claims;

namespace doan3.Controllers
{
    public class KhoaHocsController : Controller
    {
        private readonly DacsGplxContext _context;

        public KhoaHocsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: KhoaHocs
        public async Task<IActionResult> Index()
        {
            // Khởi tạo các biến mặc định
            doan3.Models.HocVien? hocVien = null;
            doan3.Models.HoSoThiSinh? hoSo = null;

            // Kiểm tra xem người dùng có đăng nhập hay không
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var userId))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                    if (user != null)
                    {
                        hocVien = await _context.HocViens.FirstOrDefaultAsync(hv => hv.HocvienId == user.Referenceld);
                        hoSo = hocVien != null
                            ? await _context.HoSoThiSinhs.FirstOrDefaultAsync(h => h.HocvienId == hocVien.HocvienId)
                            : null;
                    }
                }
            }

            // Lấy danh sách khóa học
            var khoaHocs = await _context.KhoaHocs
                .Include(k => k.Hang)
                .ToListAsync();

            var registrationDeadline = DateOnly.FromDateTime(DateTime.Today.AddDays(3));

            foreach (var khoaHoc in khoaHocs)
            {
                var registeredStudents = await (from lh in _context.LopHocs
                                                join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                                where lh.KhoahocId == khoaHoc.KhoahocId
                                                select kq.HosoId)
                                              .Distinct()
                                              .CountAsync();

                khoaHoc.SoLuongConLai = khoaHoc.SlToida - registeredStudents;
                // Chỉ đặt trạng thái "Có thể đăng ký" nếu người dùng đã đăng nhập và thỏa mãn điều kiện
                khoaHoc.Trangthai = (
                    hocVien != null &&
                    hoSo != null &&
                    khoaHoc.HangId == hoSo.HangId &&
                    khoaHoc.SoLuongConLai > 0 &&
                    khoaHoc.Ngaybatdau > registrationDeadline
                ) ? "Có thể đăng ký" : "Không thể đăng ký";
            }

            // Tạo ViewModel để truyền cả khoaHocs, hoSo và hocVien
            var viewModel = new KhoaHocViewModel
            {
                KhoaHocs = khoaHocs,
                HoSo = hoSo,
                HocVien = hocVien
            };

            return View(viewModel);
        }


        // GET: KhoaHocs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.Hang)
                .FirstOrDefaultAsync(m => m.KhoahocId == id);

            if (khoaHoc == null)
            {
                return NotFound();
            }

            // Calculate SoLuongConLai
            var registeredStudents = await (from lh in _context.LopHocs
                                            join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                            where lh.KhoahocId == khoaHoc.KhoahocId
                                            select kq.HosoId)
                                          .Distinct()
                                          .CountAsync();
            khoaHoc.SoLuongConLai = khoaHoc.SlToida - registeredStudents;

            return View(khoaHoc);
        }

        // GET: KhoaHocs/Create
        public IActionResult Create()
        {
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "Tenhang");
            return View();
        }

        // POST: KhoaHocs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HangId,Tenkhoahoc,Ngaybatdau,Ngayketthuc,SlToida,Mota")] KhoaHoc khoaHoc)
        {
            if (ModelState.IsValid)
            {
                khoaHoc.Trangthai = "Sắp mở";
                khoaHoc.SoLuongConLai = khoaHoc.SlToida; // Initialize SoLuongConLai
                _context.Add(khoaHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "Tenhang", khoaHoc.HangId);
            return View(khoaHoc);
        }

        // GET: KhoaHocs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "Tenhang", khoaHoc.HangId);
            return View(khoaHoc);
        }

        // POST: KhoaHocs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KhoahocId,HangId,Tenkhoahoc,Ngaybatdau,Ngayketthuc,SlToida,Trangthai,Mota")] KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.KhoahocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate SoLuongConLai to ensure consistency
                    var registeredStudents = await (from lh in _context.LopHocs
                                                    join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                                    where lh.KhoahocId == khoaHoc.KhoahocId
                                                    select kq.HosoId)
                                                  .Distinct()
                                                  .CountAsync();
                    khoaHoc.SoLuongConLai = khoaHoc.SlToida - registeredStudents;
                    _context.Update(khoaHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaHocExists(khoaHoc.KhoahocId))
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
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "Tenhang", khoaHoc.HangId);
            return View(khoaHoc);
        }

        // GET: KhoaHocs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.Hang)
                .FirstOrDefaultAsync(m => m.KhoahocId == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            return View(khoaHoc);
        }

        // POST: KhoaHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc != null)
            {
                _context.KhoaHocs.Remove(khoaHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhoaHocExists(int id)
        {
            return _context.KhoaHocs.Any(e => e.KhoahocId == id);
        }

        // GET: KhoaHocs/Register/1
        public async Task<IActionResult> Register(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Register", "KhoaHoc", new { id }) });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(hv => hv.HocvienId == user.Referenceld);
            var hoSo = hocVien != null
                ? await _context.HoSoThiSinhs.FirstOrDefaultAsync(h => h.HocvienId == hocVien.HocvienId)
                : null;

            if (hocVien == null || hoSo == null)
            {
                TempData["ErrorMessage"] = "Bạn cần tạo hồ sơ trước khi đăng ký khóa học.";
                return RedirectToAction("Index");
            }

            var khoaHoc = await _context.KhoaHocs
                .Include(kh => kh.Hang)
                .FirstOrDefaultAsync(kh => kh.KhoahocId == id);

            if (khoaHoc == null)
            {
                return NotFound();
            }

            // Calculate SoLuongConLai
            var registeredStudents = await (from lh in _context.LopHocs
                                            join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                            where lh.KhoahocId == khoaHoc.KhoahocId
                                            select kq.HosoId)
                                          .Distinct()
                                          .CountAsync();
            khoaHoc.SoLuongConLai = khoaHoc.SlToida - registeredStudents;

            // Check eligibility
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var registrationDeadline = currentDate.AddDays(15);
            if (khoaHoc.SoLuongConLai <= 0)
            {
                TempData["ErrorMessage"] = "Khóa học đã đủ số lượng học viên.";
                return RedirectToAction("Index");
            }
            if (khoaHoc.Ngaybatdau <= registrationDeadline)
            {
                TempData["ErrorMessage"] = $"Khóa học sắp bắt đầu (trước ngày {registrationDeadline.ToString("dd/MM/yyyy")}), không thể đăng ký.";
                return RedirectToAction("Index");
            }
            if (khoaHoc.HangId != hoSo.HangId)
            {
                TempData["ErrorMessage"] = "Khóa học này không phù hợp với hạng trong hồ sơ của bạn.";
                return RedirectToAction("Index");
            }

            var lopHocs = await _context.LopHocs
                .Where(lh => lh.KhoahocId == id)
                .Select(lh => new LopHocViewModel
                {
                    LopId = lh.LopId,
                    Tenlop = lh.Tenlop,
                    LoaiLop = lh.LoaiLop,
                    LichHocs = _context.LichHocs
                        .Where(lich => lich.LopId == lh.LopId)
                        .Select(lich => new LichHocViewModel
                        {
                            LichHocId = lich.LichhocId,
                            TgBatDau = lich.TgBatdau,
                            TgKetThuc = lich.TgKetthuc,
                            HinhThucHoc = lich.Hinhthuchoc,
                            NoiDung = lich.Noidung
                        })
                        .ToList()
                })
                .ToListAsync();

            var model = new RegisterKHViewModel
            {
                KhoaHocId = id,
                KhoaHoc = khoaHoc,
                LopHocs = lopHocs
            };

            return View(model);
        }

        // POST: KhoaHocs/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterKHViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Register", "KhoaHoc", new { id = model.KhoaHocId }) });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(hv => hv.HocvienId == user.Referenceld);
            var hoSo = hocVien != null
                ? await _context.HoSoThiSinhs.FirstOrDefaultAsync(h => h.HocvienId == hocVien.HocvienId)
                : null;

            if (hocVien == null || hoSo == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn cần tạo hồ sơ trước khi đăng ký khóa học.");
                await ReloadModel(model);
                return View(model);
            }

            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(kh => kh.KhoahocId == model.KhoaHocId);

            if (khoaHoc == null)
            {
                ModelState.AddModelError(string.Empty, "Khóa học không tồn tại.");
                await ReloadModel(model);
                return View(model);
            }

            // Recalculate SoLuongConLai
            var registeredStudents = await (from lh in _context.LopHocs
                                            join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                            where lh.KhoahocId == khoaHoc.KhoahocId
                                            select kq.HosoId)
                                          .Distinct()
                                          .CountAsync();
            khoaHoc.SoLuongConLai = khoaHoc.SlToida - registeredStudents;

            // Check eligibility
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var registrationDeadline = currentDate.AddDays(15);
            if (khoaHoc.SoLuongConLai <= 0)
            {
                ModelState.AddModelError(string.Empty, "Khóa học đã đủ số lượng học viên.");
                await ReloadModel(model);
                return View(model);
            }
            if (khoaHoc.Ngaybatdau <= registrationDeadline)
            {
                ModelState.AddModelError(string.Empty, $"Khóa học sắp bắt đầu (trước ngày {registrationDeadline.ToString("dd/MM/yyyy")}), không thể đăng ký.");
                await ReloadModel(model);
                return View(model);
            }
            if (khoaHoc.HangId != hoSo.HangId)
            {
                ModelState.AddModelError(string.Empty, "Khóa học này không phù hợp với hạng trong hồ sơ của bạn.");
                await ReloadModel(model);
                return View(model);
            }

            // Validate LopId and LichHocId
            var lopHoc = await _context.LopHocs.FirstOrDefaultAsync(lh => lh.LopId == model.LopId && lh.KhoahocId == model.KhoaHocId);
            var lichHoc = await _context.LichHocs.FirstOrDefaultAsync(lich => lich.LichhocId == model.LichHocId && lich.LopId == model.LopId);
            if (lopHoc == null || lichHoc == null)
            {
                ModelState.AddModelError(string.Empty, "Lớp học hoặc lịch học không hợp lệ.");
                await ReloadModel(model);
                return View(model);
            }

            // Create KetQuaHocTap record
            var ketQua = new KetQuaHocTap
            {
                LopId = model.LopId,
                HosoId = hoSo.HosoId,
                Nhanxet = null,
                Sobuoiodahoc = 0,
                Sobuoitoithieu = 0,
                KmHoanthanh = 0,
                GioBandem = 0,
                HtLythuyet = "Chưa học",
                HtMophong = "Chưa học",
                HtSahinh = "Chưa học",
                HtDuongtruong = "Chưa học",
                DauTn = "Chưa thi",
                DuDkThisat = "Chưa xét",
                Thoigiancapnhat = DateTime.Now
            };

            _context.KetQuaHocTaps.Add(ketQua);

            // Update SoLuongConLai
            khoaHoc.SoLuongConLai--;
            _context.Update(khoaHoc);

            await _context.SaveChangesAsync();

            // Calculate payment deadline (5 days before course start)
            var paymentDeadline = khoaHoc.Ngaybatdau.AddDays(-5);

            TempData["SuccessMessage"] = $"Đăng ký khóa học thành công! Vui lòng lưu ý:\n" +
                                        $"- Chắc chắn rằng lịch học đã phù hợp với bạn. Nếu lịch học không đáp ứng được thời gian của bạn thì có thể chọn lịch khác.\n" +
                                        $"- Khóa học của bạn sẽ bị hủy nếu thanh toán học phí sau ngày {paymentDeadline.ToString("dd/MM/yyyy")}.";
            return RedirectToAction("Index", "Home");
        }

        private async Task ReloadModel(RegisterKHViewModel model)
        {
            model.KhoaHoc = await _context.KhoaHocs
                .Select(kh => new KhoaHoc { KhoahocId = kh.KhoahocId, Tenkhoahoc = kh.Tenkhoahoc })
                .FirstOrDefaultAsync(kh => kh.KhoahocId == model.KhoaHocId);
            model.LopHocs = await _context.LopHocs
                .Where(lh => lh.KhoahocId == model.KhoaHocId)
                .Select(lh => new LopHocViewModel
                {
                    LopId = lh.LopId,
                    Tenlop = lh.Tenlop,
                    LoaiLop = lh.LoaiLop,
                    LichHocs = _context.LichHocs
                        .Where(lich => lich.LopId == lh.LopId)
                        .Select(lich => new LichHocViewModel
                        {
                            LichHocId = lich.LichhocId,
                            TgBatDau = lich.TgBatdau,
                            TgKetThuc = lich.TgKetthuc,
                            HinhThucHoc = lich.Hinhthuchoc,
                            NoiDung = lich.Noidung
                        })
                        .ToList()
                })
                .ToListAsync();
        }
    }
}