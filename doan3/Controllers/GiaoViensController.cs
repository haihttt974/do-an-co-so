using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doan3.Models;

namespace doan3.Controllers
{
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

        // GET: GiaoViens/Details/5
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

        // GET: GiaoViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GiaoViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        // POST: GiaoViens/AddAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAccount(int id, string username, string password)
        {
            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            try
            {
                // Kiểm tra xem giáo viên đã có tài khoản chưa
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Referenceld == id);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Giáo viên này đã có tài khoản!");
                    ViewBag.GiaoVienId = id;
                    ViewBag.TenGiaoVien = giaoVien.Tengiaovien;
                    return View();
                }

                // Tạo tài khoản mới
                var user = new User
                {
                    Username = username,
                    Password = password, // Nên mã hóa mật khẩu trong thực tế
                    Referenceld = id, // Sử dụng Referenceld thay vì GiaovienId
                    RoleId = 2, // Ví dụ: RoleId 2 cho giáo viên (cần có bảng Role)
                    Createat = DateTime.Now,
                    Isactive = true // Mặc định tài khoản hoạt động
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo tài khoản: " + ex.Message);
                ViewBag.GiaoVienId = id;
                ViewBag.TenGiaoVien = giaoVien.Tengiaovien;
                return View();
            }
        }
        // POST: GiaoViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GiaovienId,Tengiaovien,Chuyenmon,HangDaotao,Ngaybatdaulamviec,ImgGv")] GiaoVien giaoVien)
        {
            if (id != giaoVien.GiaovienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaoVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiaoVienExists(giaoVien.GiaovienId))
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
            return View(giaoVien);
        }

        // GET: GiaoViens/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // GET: GiaoViens/AddAccount/5
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

            ViewBag.GiaoVienId = id;
            ViewBag.TenGiaoVien = giaoVien.Tengiaovien;
            return View();
        }
        // POST: GiaoViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien != null)
            {
                _context.GiaoViens.Remove(giaoVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiaoVienExists(int id)
        {
            return _context.GiaoViens.Any(e => e.GiaovienId == id);
        }
    }
}
