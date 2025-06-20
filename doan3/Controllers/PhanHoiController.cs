﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.Models;
using Microsoft.AspNetCore.Authorization;

namespace doan3.Controllers
{
    [Authorize]
    public class PhanHoiController : Controller
    {
        private readonly DacsGplxContext _context;

        public PhanHoiController(DacsGplxContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phản hồi của học viên hiện tại
        public async Task<IActionResult> Index()
        {
            int hocvienId = GetCurrentHocVienId();

            var phanhois = await _context.Phanhois
                .Where(p => p.HocvienId == hocvienId)
                .OrderByDescending(p => p.Thoigianph)
                .ToListAsync();

            return View(phanhois);
        }

        // GET: Hiển thị form tạo phản hồi
        public IActionResult Create()
        {
            return View(new Phanhoi());
        }

        // POST: Tạo phản hồi mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Noidung")] Phanhoi phanhoi)
        {
            // Remove validation for fields set programmatically or not included in the form
            ModelState.Remove("PhanhoiId");
            ModelState.Remove("HocvienId");
            ModelState.Remove("Thoigianph");
            ModelState.Remove("Hocvien");

            if (!ModelState.IsValid)
            {
                return View(phanhoi);
            }

            try
            {
                phanhoi.HocvienId = GetCurrentHocVienId();
                phanhoi.Thoigianph = DateTime.Now;

                _context.Phanhois.Add(phanhoi);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Phản hồi đã gửi thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Có lỗi xảy ra khi gửi phản hồi: " + ex.Message;
                return View(phanhoi);
            }
        }

        // Lấy Id Học viên hiện tại dựa vào user login
        private int GetCurrentHocVienId()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new Exception("Bạn chưa đăng nhập!");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.Referenceld == null)
            {
                throw new Exception("Không tìm thấy học viên tương ứng.");
            }

            return user.Referenceld.Value;
        }
    }
}