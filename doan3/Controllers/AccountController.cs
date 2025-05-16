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
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background-color: #f9fafb;
                    margin: 0;
                    padding: 0;
                }}
                .email-container {{
                    padding: 50px 20px;
                }}
                .email-box {{
                    max-width: 650px;
                    margin: 0 auto;
                    background: #ffffff;
                    border-radius: 15px;
                    box-shadow: 0 8px 16px rgba(0,0,0,0.08);
                    padding: 40px;
                }}
                .email-box h2 {{
                    font-size: 26px;
                    color: #1a1a1a;
                    margin-bottom: 20px;
                }}
                .email-box p {{
                    font-size: 16px;
                    color: #333333;
                    line-height: 1.7;
                    margin-bottom: 15px;
                }}
                .otp-container {{
                    text-align: center;
                    margin: 30px 0;
                }}
                .otp-code {{
                    font-size: 38px;
                    font-weight: bold;
                    color: #007bff;
                    letter-spacing: 8px;
                    background: #e7f1ff;
                    border: 1px solid #cce4ff;
                    border-radius: 10px;
                    padding: 20px;
                    display: inline-block;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
                }}
                .copy-btn {{
                    display: inline-flex;
                    align-items: center;
                    font-size: 15px;
                    color: #ffffff !important;
                    background: linear-gradient(90deg, #007bff, #0056b3);
                    padding: 10px 18px;
                    border-radius: 25px;
                    text-decoration: none;
                    transition: transform 0.2s, background 0.2s;
                    margin-left: 15px;
                    cursor: pointer;
                }}
                .copy-btn:hover {{
                    transform: scale(1.05);
                    background: linear-gradient(90deg, #0056b3, #003d80);
                }}
                .copy-btn span {{
                    margin-right: 6px;
                }}
                .footer {{
                    font-size: 13px;
                    color: #6c757d;
                    text-align: center;
                    margin-top: 40px;
                    line-height: 1.6;
                }}
                @media (max-width: 600px) {{
                    .email-box {{
                        padding: 25px;
                    }}
                    .otp-code {{
                        font-size: 30px;
                        letter-spacing: 5px;
                        padding: 15px;
                    }}
                    .copy-btn {{
                        padding: 8px 15px;
                        font-size: 14px;
                    }}
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-box'>
                    <h2>Xác minh tài khoản học viên</h2>
                    <p>Chào bạn,</p>
                    <p>Bạn vừa thực hiện đăng ký tài khoản tại 
                        <strong>Trường Đào Tạo & Cấp Giấy Phép Lái Xe - DRIVING SCHOOL</strong>.</p>
                    <p>Vui lòng sử dụng mã OTP dưới đây để xác minh tài khoản của bạn:</p>
                    <div class='otp-container'>
                        <span class='otp-code' id='otp'>{otp}</span>
                    </div>
                    <p>Nhấn để sao chép hoặc chọn mã thủ công. Mã có hiệu lực trong <strong>60 giây</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                    <p style='margin-top: 20px;'>Trân trọng,</p>
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
