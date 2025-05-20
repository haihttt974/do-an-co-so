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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            var hoSos = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .Where(h => h.HocvienId == user.Referenceld && h.Ghichu == "Duyệt")
                .ToListAsync();

            return View("MyHoSo", hoSos);
        }


        // GET: Duyet
        public async Task<IActionResult> Duyet()
        {
            var hoSos = await _context.HoSoThiSinhs
                .Include(h => h.Hocvien)
                .Where(h => h.Ghichu == "Chưa được duyệt")
                .ToListAsync();

            return View(hoSos);
        }

        // POST: DuyetHoSo
        [HttpPost]
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
            return RedirectToAction("Duyet"); // hoặc tên view phù hợp
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

            // Lấy UserId từ claim
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Lấy người dùng và kiểm tra RoleId = 3 (học viên)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 3);
            if (user == null || user.Referenceld == null)
            {
                return Unauthorized();
            }

            var hocVienId = user.Referenceld.Value;

            // Tạo hồ sơ ban đầu
            var hoSo = new HoSoThiSinh
            {
                HocvienId = hocVienId,
                LoaiHoso = model.LoaiHoso,
                HangId = model.HangId,
                Ngaydk = DateOnly.FromDateTime(DateTime.Now),
                Ghichu = "Chưa được duyệt"
            };

            _context.HoSoThiSinhs.Add(hoSo);
            await _context.SaveChangesAsync(); // Lưu để có HosoId

            // Đường dẫn đến thư mục wwwroot/img/Profile
            var profilePath = Path.Combine(webHostEnvironment.WebRootPath, "img", "Profile");

            // Kiểm tra và tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(profilePath))
            {
                Directory.CreateDirectory(profilePath);
            }

            // Tạo tên file
            var imgFileName = $"img{hoSo.HosoId}.jpg";
            var kskFileName = $"ksk{hoSo.HosoId}.jpg";

            // Đường dẫn đầy đủ tới file
            var imgPath = Path.Combine(profilePath, imgFileName);
            var kskPath = Path.Combine(profilePath, kskFileName);

            // Lưu ảnh
            using (var stream = new FileStream(imgPath, FileMode.Create))
            {
                await model.ImgThisinhFile.CopyToAsync(stream);
            }

            using (var stream = new FileStream(kskPath, FileMode.Create))
            {
                await model.KhamsuckhoeFile.CopyToAsync(stream);
            }

            // Cập nhật đường dẫn ảnh trong cơ sở dữ liệu
            hoSo.ImgThisinh = $"/img/Profile/{imgFileName}"; // Lưu đường dẫn tương đối
            hoSo.Khamsuckhoe = $"/img/Profile/{kskFileName}";

            _context.HoSoThiSinhs.Update(hoSo);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyHoSo");
        }
    }
}
