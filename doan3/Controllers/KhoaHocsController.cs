using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using doan3.ViewModel;

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
            var khoaHocs = await _context.KhoaHocs
                .Include(k => k.Hang)
                .ToListAsync();

            // Calculate SoLuongConLai for each course
            foreach (var khoaHoc in khoaHocs)
            {
                var registeredStudents = await (from lh in _context.LopHocs
                                                join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                                where lh.KhoahocId == khoaHoc.KhoahocId
                                                select kq.HosoId)
                                              .Distinct()
                                              .CountAsync();
                khoaHoc.SoLuongConLai = khoaHoc.SlToida - registeredStudents;
            }

            return View(khoaHocs);
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
            var khoaHoc = await _context.KhoaHocs
                .Select(kh => new KhoaHoc
                {
                    KhoahocId = kh.KhoahocId,
                    Tenkhoahoc = kh.Tenkhoahoc,
                    HangId = kh.HangId,
                    Ngaybatdau = kh.Ngaybatdau,
                    SlToida = kh.SlToida
                })
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

            // Check if the course is eligible for registration
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var registrationDeadline = currentDate.AddDays(15);
            if (khoaHoc.SoLuongConLai <= 0 || khoaHoc.Ngaybatdau <= registrationDeadline)
            {
                TempData["ErrorMessage"] = khoaHoc.SoLuongConLai <= 0
                    ? "Khóa học đã đủ số lượng học viên."
                    : $"Khóa học sắp bắt đầu (trước ngày {registrationDeadline.ToString("dd/MM/yyyy")}), không thể đăng ký.";
                return RedirectToAction("Index", "Home");
            }

            var lopHocs = await _context.LopHocs
                .Where(lh => lh.KhoahocId == id)
                .Select(lh => new LopHocViewModel
                {
                    LopId = lh.LopId,
                    Tenlop = lh.Tenlop,
                    LoaiLop = lh.LoaiLop
                })
                .ToListAsync();

            var model = new RegisterViewModel
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload the view with validation errors
                model.KhoaHoc = await _context.KhoaHocs
                    .Select(kh => new KhoaHoc { KhoahocId = kh.KhoahocId, Tenkhoahoc = kh.Tenkhoahoc })
                    .FirstOrDefaultAsync(kh => kh.KhoahocId == model.KhoaHocId);
                model.LopHocs = await _context.LopHocs
                    .Where(lh => lh.KhoahocId == model.KhoaHocId)
                    .Select(lh => new LopHocViewModel { LopId = lh.LopId, Tenlop = lh.Tenlop, LoaiLop = lh.LoaiLop })
                    .ToListAsync();
                return View(model);
            }

            // Retrieve the KhoaHoc to get HangId and verify eligibility
            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(kh => kh.KhoahocId == model.KhoaHocId);

            if (khoaHoc == null)
            {
                ModelState.AddModelError(string.Empty, "Khóa học không tồn tại.");
                model.KhoaHoc = new KhoaHoc { KhoahocId = model.KhoaHocId };
                model.LopHocs = await _context.LopHocs
                    .Where(lh => lh.KhoahocId == model.KhoaHocId)
                    .Select(lh => new LopHocViewModel { LopId = lh.LopId, Tenlop = lh.Tenlop, LoaiLop = lh.LoaiLop })
                    .ToListAsync();
                return View(model);
            }

            // Recalculate SoLuongConLai to ensure accuracy
            var registeredStudents = await (from lh in _context.LopHocs
                                            join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                            where lh.KhoahocId == khoaHoc.KhoahocId
                                            select kq.HosoId)
                                          .Distinct()
                                          .CountAsync();
            khoaHoc.SoLuongConLai = khoaHoc.SlToida - registeredStudents;

            // Check eligibility again to prevent race conditions
            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var registrationDeadline = currentDate.AddDays(15);
            if (khoaHoc.SoLuongConLai <= 0 || khoaHoc.Ngaybatdau <= registrationDeadline)
            {
                ModelState.AddModelError(string.Empty, khoaHoc.SoLuongConLai <= 0
                    ? "Khóa học đã đủ số lượng học viên."
                    : $"Khóa học sắp bắt đầu (trước ngày {registrationDeadline.ToString("dd/MM/yyyy")}), không thể đăng ký.");
                model.KhoaHoc = khoaHoc;
                model.LopHocs = await _context.LopHocs
                    .Where(lh => lh.KhoahocId == model.KhoaHocId)
                    .Select(lh => new LopHocViewModel { LopId = lh.LopId, Tenlop = lh.Tenlop, LoaiLop = lh.LoaiLop })
                    .ToListAsync();
                return View(model);
            }

            // Create a new HO_SO_THI_SINH record
            var hoSo = new HoSoThiSinh
            {
                HocvienId = 1, // Replace with actual logged-in user's HocvienId
                ImgThisinh = null,
                LoaiHoso = model.LoaiHoSo,
                HangId = khoaHoc.HangId ?? 0,
                Ngaydk = DateOnly.FromDateTime(DateTime.Today),
                Khamsuckhoe = model.KhamSucKhoe,
                Ghichu = model.GhiChu
            };

            _context.HoSoThiSinhs.Add(hoSo);
            await _context.SaveChangesAsync();

            // Create a KET_QUA_HOC_TAP record for the selected class
            var ketQua = new KetQuaHocTap
            {
                LopId = model.LopId,
                HosoId = hoSo.HosoId,
                Nhanxet = null,
                Sobuoiodahoc = 0,
                Sobuoitoithieu = 0,
                KmHoanthanh = 0,
                GioBandem = 0,
                HtLythuyet = "Chưa hoàn thành",
                HtMophong = "Chưa hoàn thành",
                HtSahinh = "Chưa hoàn thành",
                HtDuongtruong = "Chưa hoàn thành",
                DauTn = "Không đạt",
                DuDkThisat = "Không đủ",
                Thoigiancapnhat = DateTime.Now
            };

            _context.KetQuaHocTaps.Add(ketQua);
            await _context.SaveChangesAsync();

            // Update SoLuongConLai in KhoaHoc
            khoaHoc.SoLuongConLai--;
            _context.Update(khoaHoc);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký khóa học thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}