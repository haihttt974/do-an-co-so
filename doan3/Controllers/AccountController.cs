using doan3.Models;
using doan3.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace doan3.Controllers
{
    public class AccountController : Controller
    {
        private readonly DacsGplxContext _context;

        public AccountController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Isactive == true);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu.");
                return View();
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }



        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo học viên mới
                var hocVien = new HocVien
                {
                    Tenhocvien = model.TenHocVien,
                    Socccd = model.SoCCCD,
                    Gioitinh = model.GioiTinh,
                    Ngaysinh = model.NgaySinh
                };

                _context.HocViens.Add(hocVien);
                _context.SaveChanges();

                // Hash mật khẩu bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Tạo user mới
                var user = new User
                {
                    Username = model.Username,
                    Password = hashedPassword,
                    RoleId = 3, // giả sử 3 là role học viên
                    Referenceld = hocVien.HocvienId,
                    Email = model.Email,
                    Sdt = model.SDT,
                    Diachi = model.DiaChi,
                    Createat = DateTime.Now,
                    Updateat = DateTime.Now,
                    Isactive = true
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }



        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [Authorize]
        public IActionResult Profile()
        {
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return NotFound();
            return View(user);
        }
        [Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) return NotFound();

            user.Email = updatedUser.Email;
            user.Sdt = updatedUser.Sdt;
            user.Diachi = updatedUser.Diachi;

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không đúng");
                return View();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var asBytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(asBytes);
                return Convert.ToBase64String(hash);
            }
        }

    }
}
