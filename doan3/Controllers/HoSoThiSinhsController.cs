using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using doan3.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace doan3.Controllers
{
    public class HoSoThiSinhsController : Controller
    {
        private readonly DacsGplxContext _context;

        public HoSoThiSinhsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: HoSoThiSinhs
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.HoSoThiSinhs.Include(h => h.Hang).Include(h => h.Hocvien);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: HoSoThiSinhs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoThiSinh = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .Include(h => h.Hocvien)
                .FirstOrDefaultAsync(m => m.HosoId == id);
            if (hoSoThiSinh == null)
            {
                return NotFound();
            }

            return View(hoSoThiSinh);
        }

        // GET: HoSoThiSinhs/Create
        public IActionResult Create()
        {
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId");
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId");
            return View();
        }

        // POST: HoSoThiSinhs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HocvienId,ImgThisinh,LoaiHoso,HangId,Khamsuckhoe")] HoSoThiSinh hoSoThiSinh)
        {
            hoSoThiSinh.Ghichu = "Chưa được duyệt";
            hoSoThiSinh.Ngaydk = DateOnly.FromDateTime(DateTime.Now);

            if (ModelState.IsValid)
            {
                _context.Add(hoSoThiSinh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyHoSo), new { hocVienId = hoSoThiSinh.HocvienId });
            }

            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", hoSoThiSinh.HangId);
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", hoSoThiSinh.HocvienId);
            return View(hoSoThiSinh);
        }

        // GET: HoSoThiSinhs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoThiSinh = await _context.HoSoThiSinhs.FindAsync(id);
            if (hoSoThiSinh == null)
            {
                return NotFound();
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", hoSoThiSinh.HangId);
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", hoSoThiSinh.HocvienId);
            return View(hoSoThiSinh);
        }

        // POST: HoSoThiSinhs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HosoId,HocvienId,ImgThisinh,LoaiHoso,HangId,Ngaydk,Khamsuckhoe,Ghichu")] HoSoThiSinh hoSoThiSinh)
        {
            if (id != hoSoThiSinh.HosoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoSoThiSinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoSoThiSinhExists(hoSoThiSinh.HosoId))
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
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", hoSoThiSinh.HangId);
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", hoSoThiSinh.HocvienId);
            return View(hoSoThiSinh);
        }

        // GET: HoSoThiSinhs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoThiSinh = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .Include(h => h.Hocvien)
                .FirstOrDefaultAsync(m => m.HosoId == id);
            if (hoSoThiSinh == null)
            {
                return NotFound();
            }

            return View(hoSoThiSinh);
        }

        // POST: HoSoThiSinhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoSoThiSinh = await _context.HoSoThiSinhs.FindAsync(id);
            if (hoSoThiSinh != null)
            {
                _context.HoSoThiSinhs.Remove(hoSoThiSinh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoSoThiSinhExists(int id)
        {
            return _context.HoSoThiSinhs.Any(e => e.HosoId == id);
        }

        [Authorize(Roles = "3")]
        public async Task<IActionResult> MyHoSo()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Không có thông tin người dùng");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.RoleId != 3)
                return Content("Không tìm thấy thông tin người dùng hoặc không phải học viên.");

            if (user.Referenceld == null)
                return Content("Không tìm thấy thông tin học viên.");

            // Fetch HocVien data
            var hocVien = await _context.HocViens
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HocvienId == user.Referenceld);

            if (hocVien == null)
                return Content("Không tìm thấy thông tin học viên.");

            // Set ViewBag data
            ViewBag.UserInfo = new
            {
                user.Username,
                user.Email,
                Diachi = user.Diachi ?? "Chưa cập nhật",
                Sdt = user.Sdt ?? "Chưa cập nhật"
            };

            ViewBag.HocVienInfo = new
            {
                hocVien.Tenhocvien,
                hocVien.Socccd,
                hocVien.Gioitinh,
                hocVien.Ngaysinh
            };

            var hoSos = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .Where(h => h.HocvienId == user.Referenceld && h.Ghichu == "Duyệt")
                .ToListAsync();

            var allHoSos = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .Where(h => h.HocvienId == user.Referenceld)
                .ToListAsync();

            var hoSosDaDuyet = allHoSos.Where(h => h.Ghichu == "Duyệt").ToList();
            ViewBag.CoHoSo = allHoSos.Any();
            ViewBag.DaDuocDuyet = hoSosDaDuyet.Any();

            return View("MyHoSo", hoSosDaDuyet);
            //return View("MyHoSo", hoSos);
        }

        // GET: Duyet
        public async Task<IActionResult> Duyet()
        {
            var hoSos = await _context.HoSoThiSinhs
                .Include(h => h.Hocvien)
                .Include(h => h.Hang)
                .Where(h => h.Ghichu == "Chưa được duyệt")
                .ToListAsync();

            return View(hoSos);
        }

        // POST: DuyetHoSo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetHoSo(int id, bool passed)
        {
            var hoso = await _context.HoSoThiSinhs.FindAsync(id);
            if (hoso == null)
            {
                return NotFound();
            }

            if (passed)
            {
                hoso.Ghichu = "Duyệt";
                _context.Update(hoso);
            }
            else
            {
                _context.HoSoThiSinhs.Remove(hoso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Duyet");
        }


        [Authorize]
        public IActionResult CreateUser()
        {
            var hangList = _context.HangGplxes.ToList();
            ViewBag.HangList = new SelectList(hangList, "HangId", "Tenhang");
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateHoSoViewModel model, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.HangList = new SelectList(_context.HangGplxes, "HangId", "Tenhang");
                return View(model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 3);
            if (user == null || user.Referenceld == null)
            {
                return Unauthorized();
            }

            var hocVienId = user.Referenceld.Value;
            var hoSo = new HoSoThiSinh
            {
                HocvienId = hocVienId,
                LoaiHoso = model.LoaiHoso,
                HangId = model.HangId,
                Ngaydk = DateOnly.FromDateTime(DateTime.Now),
                Ghichu = "Chưa được duyệt"
            };

            _context.HoSoThiSinhs.Add(hoSo);
            await _context.SaveChangesAsync();

            var profilePath = Path.Combine(webHostEnvironment.WebRootPath, "img", "Profile");
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "img", "file");
            if (!Directory.Exists(profilePath))
            {
                Directory.CreateDirectory(profilePath);
            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var imgFileName = $"img{hoSo.HosoId}.jpg";
            var kskFileExtension = Path.GetExtension(model.KhamsuckhoeFile.FileName);
            var kskFileName = $"ksk{hoSo.HosoId}{kskFileExtension}";
            var imgPath = Path.Combine(profilePath, imgFileName);
            var kskPath = Path.Combine(filePath, kskFileName);

            using (var stream = new FileStream(imgPath, FileMode.Create))
            {
                await model.ImgThisinhFile.CopyToAsync(stream);
            }

            using (var stream = new FileStream(kskPath, FileMode.Create))
            {
                await model.KhamsuckhoeFile.CopyToAsync(stream);
            }

            hoSo.ImgThisinh = $"/img/Profile/{imgFileName}";
            hoSo.Khamsuckhoe = $"/img/file/{kskFileName}";
            _context.HoSoThiSinhs.Update(hoSo);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyHoSo");
        }
    }
}