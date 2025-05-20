using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.ViewModel;
using doan3.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace doan3.Controllers
{
    [Authorize(Roles = "3")]
    public class ExamRegistrationController : Controller
    {
        private readonly DacsGplxContext _context;

        public ExamRegistrationController(DacsGplxContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> RegisterExam()
        {
            // Lấy HocvienId từ ClaimTypes.NameIdentifier (vì RoleId == 3)
            var hocVienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hocVienIdClaim, out int hocVienId))
            {
                return Unauthorized("Vui lòng đăng nhập.");
            }

            // Ghi log để debug
            Console.WriteLine($"HocvienId from claim: {hocVienId}");

            // Tìm user dựa trên Referenceld và RoleId
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Referenceld == hocVienId && u.RoleId == 3);

            if (user == null)
            {
                Console.WriteLine($"Không tìm thấy user với Referenceld: {hocVienId} và RoleId: 3");
                return View(new ExamRegistrationViewModel
                {
                    UserId = 0,
                    HocVienId = hocVienId,
                    RegistrationMessage = "Tài khoản học viên không tồn tại hoặc không hợp lệ."
                });
            }

            // Kiểm tra hồ sơ thi
            var hoSo = await _context.HoSoThiSinhs
                .FirstOrDefaultAsync(h => h.HocvienId == hocVienId);

            if (hoSo == null)
            {
                Console.WriteLine($"Không tìm thấy hồ sơ thi cho HocvienId: {hocVienId}");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    RegistrationMessage = "Bạn chưa có hồ sơ thi. Vui lòng đăng ký hồ sơ thi trước."
                });
            }

            // Kiểm tra kết quả học tập
            var ketQua = await _context.KetQuaHocTaps
                .FirstOrDefaultAsync(k => k.HosoId == hoSo.HosoId);

            if (ketQua == null)
            {
                Console.WriteLine($"Không tìm thấy kết quả học tập cho HosoId: {hoSo.HosoId}");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    RegistrationMessage = "Bạn chưa có kết quả học tập. Vui lòng hoàn thành khóa học."
                });
            }

            // Xác định điều kiện đăng ký thi
            bool canRegisterGraduation = ketQua.Nhanxet == "Được thi tốt nghiệp";
            bool canRegisterOfficial = ketQua.Nhanxet == "Đậu tốt nghiệp" && ketQua.DauTn == "Đạt";

            // Lọc lịch thi dựa trên HangId và loại thi
            var availableExams = await _context.LichThis
                .Where(l => l.Ghichu.Contains($"Hạng {hoSo.HangId}"))
                .Where(l => (canRegisterGraduation && l.Ghichu.Contains("Tốt nghiệp")) ||
                            (canRegisterOfficial && l.Ghichu.Contains("Sát hạch")))
                .ToListAsync();

            var viewModel = new ExamRegistrationViewModel
            {
                UserId = user.UserId,
                HocVienId = hocVienId,
                AvailableExams = availableExams
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterExam(ExamRegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableExams = await _context.LichThis
                    .Where(l => l.Ghichu.Contains("Hạng"))
                    .ToListAsync();
                return View(viewModel);
            }

            // Kiểm tra user dựa trên HocVienId
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Referenceld == viewModel.HocVienId && u.RoleId == 3);

            if (user == null)
            {
                viewModel.RegistrationMessage = "Tài khoản học viên không tồn tại.";
                viewModel.AvailableExams = await _context.LichThis
                    .Where(l => l.Ghichu.Contains("Hạng"))
                    .ToListAsync();
                return View(viewModel);
            }

            // Kiểm tra hồ sơ thi
            var hoSo = await _context.HoSoThiSinhs
                .FirstOrDefaultAsync(h => h.HocvienId == viewModel.HocVienId);

            if (hoSo == null)
            {
                viewModel.RegistrationMessage = "Bạn chưa có hồ sơ thi.";
                viewModel.AvailableExams = await _context.LichThis
                    .Where(l => l.Ghichu.Contains("Hạng"))
                    .ToListAsync();
                return View(viewModel);
            }

            // Kiểm tra kết quả học tập
            var ketQua = await _context.KetQuaHocTaps
                .FirstOrDefaultAsync(k => k.HosoId == hoSo.HosoId);

            if (ketQua == null)
            {
                viewModel.RegistrationMessage = "Bạn chưa có kết quả học tập.";
                viewModel.AvailableExams = await _context.LichThis
                    .Where(l => l.Ghichu.Contains("Hạng"))
                    .ToListAsync();
                return View(viewModel);
            }

            // Kiểm tra lịch thi được chọn
            var selectedExam = await _context.LichThis
                .FirstOrDefaultAsync(l => l.LichthiId == viewModel.SelectedLichThiId);

            if (selectedExam == null)
            {
                viewModel.RegistrationMessage = "Lịch thi không hợp lệ.";
                viewModel.AvailableExams = await _context.LichThis
                    .Where(l => l.Ghichu.Contains($"Hạng {hoSo.HangId}"))
                    .ToListAsync();
                return View(viewModel);
            }

            // Kiểm tra điều kiện đăng ký
            bool canRegisterGraduation = ketQua.Nhanxet == "Được thi tốt nghiệp";
            bool canRegisterOfficial = ketQua.Nhanxet == "Đậu tốt nghiệp" && ketQua.DauTn == "Đạt";

            bool isGraduationExam = selectedExam.Ghichu.Contains("Tốt nghiệp");
            bool isOfficialExam = selectedExam.Ghichu.Contains("Sát hạch");

            if ((isGraduationExam && !canRegisterGraduation) || (isOfficialExam && !canRegisterOfficial))
            {
                viewModel.RegistrationMessage = "Bạn không đủ điều kiện đăng ký lịch thi này.";
                viewModel.AvailableExams = await _context.LichThis
                    .Where(l => l.Ghichu.Contains($"Hạng {hoSo.HangId}"))
                    .ToListAsync();
                return View(viewModel);
            }

            // Kiểm tra xem đã đăng ký lịch thi này chưa
            var existingRegistration = await _context.CtDangKyThis
                .FirstOrDefaultAsync(ct => ct.HosoId == hoSo.HosoId && ct.LichthiId == viewModel.SelectedLichThiId);

            if (existingRegistration != null)
            {
                viewModel.RegistrationMessage = "Bạn đã đăng ký lịch thi này.";
                viewModel.AvailableExams = await _context.LichThis
                    .Where(l => l.Ghichu.Contains($"Hạng {hoSo.HangId}"))
                    .ToListAsync();
                return View(viewModel);
            }

            // Lưu đăng ký thi
            var registration = new CtDangKyThi
            {
                KythiId = isGraduationExam ? 1 : 2, // 1: Thi tốt nghiệp, 2: Thi sát hạch
                HosoId = hoSo.HosoId,
                Thoigiandk = DateTime.Now,
                Thoigianthi = selectedExam.Thoigianthi,
                LichthiId = viewModel.SelectedLichThiId
            };

            _context.CtDangKyThis.Add(registration);
            await _context.SaveChangesAsync();

            viewModel.RegistrationMessage = "Đăng ký lịch thi thành công!";
            viewModel.RegisteredExam = registration;
            viewModel.AvailableExams = new List<LichThi> { selectedExam };

            return View(viewModel);
        }
    }
}