using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using System.Globalization;
using System.Text;

namespace doan3.Controllers
{
    [Authorize] // Yêu cầu đăng nhập
    public class GiaoViensController : Controller
    {
        private readonly DacsGplxContext _context;

        public GiaoViensController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: GiaoViens
        public async Task<IActionResult> Index()
        {
            return View(await _context.GiaoViens.ToListAsync());
        }

        // GET: GiaoViens/AddAccount/5
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        [Authorize(Roles = "1")] // Chỉ admin
        public async Task<IActionResult> AddAccount(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            var user = new User
            {
                RoleId = 2,
                Referenceld = id,
                Username = RemoveDiacritics("gv." + giaoVien.Tengiaovien).Replace(" ", "").ToLower()
            };
            return View(user);
        }

        // POST: GiaoViens/AddAccount/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")] // Chỉ admin
        public async Task<IActionResult> AddAccount(int id, string Username, string Password, string Email)
        {
            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            // Kiểm tra giáo viên đã có tài khoản chưa
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Referenceld == id && u.RoleId == 2);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Giáo viên này đã có tài khoản.");
                return View(new User { Referenceld = id });
            }

            // Thêm tài khoản mới
            var user = new User
            {
                Username = Username,
                Password = BCrypt.Net.BCrypt.HashPassword(Password),
                RoleId = 2,
                Referenceld = id,
                Email = Email,
                Createat = DateTime.Now,
                Isactive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: GiaoViens/Create
        [Authorize(Roles = "1")] // Chỉ admin
        public IActionResult Create()
        {
            return View();
        }

        // POST: GiaoViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Create([Bind("GiaovienId,Tengiaovien,Chuyenmon,HangDaotao,Ngaybatdaulamviec,ImgGv")] GiaoVien giaoVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giaoVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(giaoVien);
        }

        // GET: GiaoViens/Edit/5
        [Authorize(Roles = "1")] // Chỉ admin
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien == null)
            {
                return NotFound();
            }
            return View(giaoVien);
        }

        // POST: GiaoViens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Edit(int id, [Bind("GiaovienId,Tengiaovien,Chuyenmon,HangDaotao,Ngaybatdaulamviec,ImgGv")] GiaoVien giaoVien)
        {
            if (id != giaoVien.GiaovienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(giaoVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(giaoVien);
        }

        // GET: GiaoViens/Delete/5
        [Authorize(Roles = "1")] // Chỉ admin
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(m => m.GiaovienId == id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            return View(giaoVien);
        }
        // GiaoViens/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens
                .FirstOrDefaultAsync(m => m.GiaovienId == id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            return View(giaoVien);
        }
        // POST: GiaoViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien != null)
            {
                _context.GiaoViens.Remove(giaoVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GiaoVienExists(int id)
        {
            return _context.GiaoViens.Any(e => e.GiaovienId == id);
        }
    }
}