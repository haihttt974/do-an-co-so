using doan3.Models;
using doan3.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

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

            // Nếu là học viên thì dùng ReferenceId làm NameIdentifier
            if (user.RoleId == 3) // giả sử 3 là học viên
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Referenceld.ToString())); // Đây là HocvienId
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())); // Giáo viên/admin
            }

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
        //[HttpPost]
        //public IActionResult Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Tạo học viên mới
        //        var hocVien = new HocVien
        //        {
        //            Tenhocvien = model.TenHocVien,
        //            Socccd = model.SoCCCD,
        //            Gioitinh = model.GioiTinh,
        //            Ngaysinh = model.NgaySinh
        //        };

        //        _context.HocViens.Add(hocVien);
        //        _context.SaveChanges();

        //        // Hash mật khẩu bằng BCrypt
        //        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

        //        // Tạo user mới
        //        var user = new User
        //        {
        //            Username = model.Username,
        //            Password = hashedPassword,
        //            RoleId = 3, // giả sử 3 là role học viên
        //            Referenceld = hocVien.HocvienId,
        //            Email = model.Email,
        //            Sdt = model.SDT,
        //            Diachi = model.DiaChi,
        //            Createat = DateTime.Now,
        //            Updateat = DateTime.Now,
        //            Isactive = true
        //        };

        //        _context.Users.Add(user);
        //        _context.SaveChanges();

        //        return RedirectToAction("Login");
        //    }

        //    return View(model);
        //}

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var otp = new Random().Next(100000, 999999).ToString();

                // Gửi OTP
                SendOtpEmail(model.Email, otp);

                // Lưu OTP + model + thời gian
                HttpContext.Session.SetString("RegisterModel", JsonConvert.SerializeObject(model));
                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("OTPTime", DateTime.UtcNow.ToString("o")); // ✅ Cái này bị thiếu trước đó

                return RedirectToAction("VerifyOtp");
            }

            return View(model);
        }


        public IActionResult VerifyOtp()
        {
            var otpTimeStr = HttpContext.Session.GetString("OTPTime");
            if (DateTime.TryParseExact(otpTimeStr, "o", null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime otpTime))
            {
                var elapsed = DateTime.UtcNow - otpTime;
                int timeLeft = Math.Max(0, 60 - (int)elapsed.TotalSeconds);
                ViewBag.TimeLeft = timeLeft;
            }
            else
            {
                ViewBag.TimeLeft = 60;
            }

            return View();
        }


        [HttpPost]
        public IActionResult VerifyOtp(string otp)
        {
            var expectedOtp = HttpContext.Session.GetString("OTP");
            var modelJson = HttpContext.Session.GetString("RegisterModel");
            var otpTimeStr = HttpContext.Session.GetString("OTPTime");

            if (otpTimeStr == null || !DateTime.TryParseExact(otpTimeStr, "o", null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime otpTime))
            {
                ModelState.AddModelError("", "Phiên OTP đã hết hạn. Vui lòng đăng ký lại.");
                ViewBag.TimeLeft = 0;
                return View();
            }

            var now = DateTime.UtcNow;
            var secondsElapsed = (now - otpTime).TotalSeconds;

            if (secondsElapsed > 60)
            {
                ModelState.AddModelError("", "Mã OTP đã hết hạn. Vui lòng đăng ký lại.");
                ViewBag.TimeLeft = 0;
                return View();
            }

            int timeLeft = Math.Max(0, 60 - (int)secondsElapsed);

            if (otp == expectedOtp && modelJson != null)
            {
                var model = JsonConvert.DeserializeObject<RegisterViewModel>(modelJson);

                var hocVien = new HocVien
                {
                    Tenhocvien = model.TenHocVien,
                    Socccd = model.SoCCCD,
                    Gioitinh = model.GioiTinh,
                    Ngaysinh = model.NgaySinh
                };
                _context.HocViens.Add(hocVien);
                _context.SaveChanges();

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var user = new User
                {
                    Username = model.Username,
                    Password = hashedPassword,
                    RoleId = 3,
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

                // Xóa session
                HttpContext.Session.Remove("OTP");
                HttpContext.Session.Remove("RegisterModel");
                HttpContext.Session.Remove("OTPTime");

                return RedirectToAction("Login");
            }

            // Trường hợp mã sai nhưng vẫn còn trong thời gian
            ModelState.AddModelError("", "Mã OTP không đúng.");
            ViewBag.TimeLeft = timeLeft;
            return View();
        }

        [HttpPost]
        public IActionResult ResendOtp()
        {
            var modelJson = HttpContext.Session.GetString("RegisterModel");

            if (modelJson == null)
            {
                return BadRequest("Hết phiên đăng ký. Vui lòng đăng ký lại.");
            }

            var model = JsonConvert.DeserializeObject<RegisterViewModel>(modelJson);

            // Tạo mã OTP mới
            var newOtp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", newOtp);
            HttpContext.Session.SetString("OTPTime", DateTime.UtcNow.ToString("o"));

            // Gửi lại email
            SendOtpEmail(model.Email, newOtp);

            return Ok("Đã gửi lại mã OTP.");
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

        private void SendOtpEmail(string toEmail, string otp)
        {
            var fromEmail = "drivingGPLX.shool@gmail.com";
            var fromPassword = "smqu btut ihjn czrq";
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "Trường Đào Tạo & Cấp Giấy Phép Lái Xe"),
                Subject = "Xác minh đăng ký tài khoản học viên",
                IsBodyHtml = true,
                Body = $@"
                <html>
                <head>
                  <meta charset='UTF-8'>
                  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                  <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet'>
                  <link href='https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap' rel='stylesheet'>
                  <style>
                    body {{
                      background-color: #f4f6f8;
                      font-family: 'Inter', sans-serif;
                      margin: 0;
                      padding: 0;
                      color: #374151;
                    }}
                    .email-wrapper {{
                      padding: 40px 16px;
                    }}
                    .email-card {{
                      max-width: 600px;
                      margin: 0 auto;
                      background: #ffffff;
                      border-radius: 20px;
                      padding: 48px 36px;
                      box-shadow: 0 20px 50px rgba(0, 0, 0, 0.06);
                      border: 1px solid #e5eaf1;
                    }}
                    .email-card h2 {{
                      font-size: 26px;
                      font-weight: 600;
                      color: #0f172a;
                      margin-bottom: 20px;
                    }}
                    .email-card p {{
                      font-size: 16px;
                      color: #374151;
                      line-height: 1.7;
                      margin-bottom: 16px;
                    }}
                    .otp-container {{
                      text-align: center;
                      margin: 36px 0 24px 0;
                    }}
                    .otp-code {{
                      font-size: 40px;
                      font-weight: 700;
                      letter-spacing: 10px;
                      color: #2563eb;
                      background: #f0f6ff;
                      padding: 22px 36px;
                      border-radius: 14px;
                      border: 1px solid #c3dafe;
                      display: inline-block;
                      box-shadow: 0 6px 14px rgba(0, 0, 0, 0.04);
                    }}
                    .footer {{
                      font-size: 13px;
                      color: #6b7280;
                      text-align: center;
                      margin-top: 40px;
                      line-height: 1.6;
                    }}

                    /* DARK MODE SUPPORT */
                    @media (prefers-color-scheme: dark) {{
                      body {{
                        background-color: #0d1117;
                        color: #cbd5e1;
                      }}
                      .email-card {{
                        background: #161b22;
                        border: 1px solid #30363d;
                        box-shadow: none;
                      }}
                      .email-card h2 {{
                        color: #f9fafb;
                      }}
                      .email-card p {{
                        color: #cbd5e1;
                      }}
                      .otp-code {{
                        color: #3b82f6;
                        background: #1e293b;
                        border-color: #3b82f6;
                      }}
                      .footer {{
                        color: #94a3b8;
                      }}
                    }}

                    @media (max-width: 600px) {{
                      .email-card {{
                        padding: 32px 24px;
                      }}
                      .otp-code {{
                        font-size: 30px;
                        letter-spacing: 6px;
                        padding: 18px 28px;
                      }}
                    }}
                  </style>
                </head>
                <body>
                  <div class='email-wrapper'>
                    <div class='email-card'>
                      <h2>Xác minh tài khoản học viên</h2>
                      <p>Chào bạn,</p>
                      <p>Bạn vừa đăng ký tài khoản tại <strong>Trường Đào Tạo & Cấp Giấy Phép Lái Xe - DRIVING SCHOOL</strong>.</p>
                      <p>Vui lòng sử dụng mã OTP dưới đây để xác minh tài khoản của bạn:</p>
                      <div class='otp-container'>
                        <div class='otp-code'>{otp}</div>
                      </div>
                      <p>Mã có hiệu lực trong <strong>60 giây</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                      <p>Nếu bạn không thực hiện yêu cầu này, bạn có thể bỏ qua email này.</p>
                      <p style='margin-top: 24px;'>Trân trọng,</p>
                      <p><strong>Trường Đào Tạo & Cấp Giấy Phép Lái Xe</strong></p>
                      <div class='footer'>
                        Đây là email tự động, vui lòng không phản hồi lại. <br>
                        © {DateTime.Now.Year} Trường Đào Tạo & Cấp Giấy Phép Lái Xe. All rights reserved.
                      </div>
                    </div>
                  </div>
                </body>
                </html>
                "
            };

            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);
        }
    }
}
