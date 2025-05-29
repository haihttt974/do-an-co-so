using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.ViewModel;
using doan3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace doan3.Controllers
{
    [Authorize(Roles = "3")]
    public class ExamRegistrationController : Controller
    {
        private readonly DacsGplxContext _context;

        private static readonly Dictionary<int, string> HangIdToCategory = new()
        {
            { 1, "B1" }, { 2, "B2" }, { 3, "C" }, { 4, "A1" }, { 5, "E" },
            { 6, "F" }, { 7, "FB2" }, { 8, "FC" }, { 9, "FD" }
        };

        public ExamRegistrationController(DacsGplxContext context)
        {
            _context = context;
        }

        private string ExtractCategoryFromGhichu(string ghichu)
        {
            if (string.IsNullOrEmpty(ghichu)) return string.Empty;
            var parts = ghichu.Split('-', StringSplitOptions.RemoveEmptyEntries);
            var mainPart = parts[0].Trim();
            var words = mainPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = words.Length - 1; i >= 0; i--)
            {
                var word = words[i].ToLower();
                if (HangIdToCategory.Values.Any(cat => cat.ToLower() == word))
                    return word;
            }
            return string.Empty;
        }

        private async Task<List<LichThi>> GetAvailableExams(int hocVienId, bool canRegisterGraduation, bool canRegisterOfficial, string category, int? selectedKyThiId)
        {
            if (!selectedKyThiId.HasValue) return new List<LichThi>();

            var selectedKyThi = await _context.KyThis
                .FirstOrDefaultAsync(k => k.KythiId == selectedKyThiId);

            if (selectedKyThi == null)
            {
                Console.WriteLine($"Không tìm thấy KyThi với KythiId = {selectedKyThiId}");
                return new List<LichThi>();
            }

            var allLichThis = await _context.LichThis.ToListAsync();
            Console.WriteLine($"Tổng số lịch thi: {allLichThis.Count}");

            var futureExams = allLichThis
                .Where(e => e.Thoigianthi >= DateTime.Now)
                .ToList();
            Console.WriteLine($"Số lịch thi trong tương lai (Thoigianthi >= {DateTime.Now}): {futureExams.Count}");

            var categoryFilteredExams = futureExams
                .Where(e => ExtractCategoryFromGhichu(e.Ghichu) == category)
                .ToList();
            Console.WriteLine($"Số lịch thi phù hợp với hạng {category}: {categoryFilteredExams.Count}");

            var availableExams = categoryFilteredExams
                .Where(e =>
                {
                    var examType = e.Ghichu.ToLower();
                    bool isGraduationExam = examType.Contains("tốt nghiệp");
                    bool isOfficialExam = examType.Contains("sát hạch") || examType.Contains("thực hành");
                    var result = (isGraduationExam && selectedKyThi.LoaithiId == 1 && canRegisterGraduation) ||
                                 (isOfficialExam && selectedKyThi.LoaithiId == 2 && canRegisterOfficial);
                    Console.WriteLine($"LichthiId = {e.LichthiId}, Ghichu = {e.Ghichu}, isGraduationExam = {isGraduationExam}, isOfficialExam = {isOfficialExam}, Result = {result}");
                    return result;
                })
                .ToList();

            Console.WriteLine($"Số lịch thi phù hợp sau tất cả điều kiện: {availableExams.Count}");
            return availableExams;
        }

        [HttpGet]
        public async Task<IActionResult> RegisterExam(int? selectedKyThiId)
        {
            var hocVienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hocVienIdClaim, out int hocVienId))
            {
                return Unauthorized("Vui lòng đăng nhập.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Referenceld == hocVienId && u.RoleId == 3);

            if (user == null)
            {
                return View(new ExamRegistrationViewModel
                {
                    UserId = 0,
                    HocVienId = hocVienId,
                    RegistrationMessage = "Tài khoản học viên không tồn tại hoặc không hợp lệ."
                });
            }

            var hoSo = await _context.HoSoThiSinhs
                .FirstOrDefaultAsync(h => h.HocvienId == hocVienId);

            if (hoSo == null)
            {
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    RegistrationMessage = "Bạn chưa có hồ sơ thi. Vui lòng đăng ký hồ sơ thi trước."
                });
            }

            var ketQua = await _context.KetQuaHocTaps
                .FirstOrDefaultAsync(k => k.HosoId == hoSo.HosoId);

            if (ketQua == null)
            {
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    RegistrationMessage = "Bạn chưa có kết quả học tập. Vui lòng hoàn thành khóa học."
                });
            }

            bool canRegisterGraduation = ketQua.Nhanxet == "Được thi tốt nghiệp";
            bool canRegisterOfficial = ketQua.Nhanxet == "Đậu tốt nghiệp" && ketQua.DauTn == "Đạt";
            string category = HangIdToCategory.TryGetValue(hoSo.HangId, out var cat) ? cat.ToLower() : string.Empty;

            // Lấy danh sách kỳ thi phù hợp, lọc theo HangId và điều kiện đăng ký
            var allKyThis = await _context.KyThis
                .Where(k => k.HangId == hoSo.HangId &&
                            ((canRegisterGraduation && k.LoaithiId == 1) || (canRegisterOfficial && k.LoaithiId == 2)))
                .ToListAsync();

            List<LichThi> availableExams = new List<LichThi>();
            if (selectedKyThiId.HasValue)
            {
                availableExams = await GetAvailableExams(hocVienId, canRegisterGraduation, canRegisterOfficial, category, selectedKyThiId);
            }

            return View(new ExamRegistrationViewModel
            {
                UserId = user.UserId,
                HocVienId = hocVienId,
                AvailableKyThis = allKyThis,
                SelectedKyThiId = selectedKyThiId,
                AvailableExams = availableExams,
                RegistrationMessage = !allKyThis.Any() ? "Hiện tại không có kỳ thi phù hợp với điều kiện của bạn." :
                                      (!availableExams.Any() && selectedKyThiId.HasValue ? "Hiện tại không có lịch thi phù hợp với kỳ thi bạn chọn." : null)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterExam(ExamRegistrationViewModel viewModel)
        {
            var hocVienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hocVienIdClaim, out int hocVienId))
            {
                Console.WriteLine("Lỗi: Không thể lấy HocVienId từ claim.");
                return Unauthorized("Vui lòng đăng nhập.");
            }

            Console.WriteLine($"Dữ liệu gửi lên: HocVienId = {viewModel.HocVienId}, UserId = {viewModel.UserId}, SelectedKyThiId = {viewModel.SelectedKyThiId}, SelectedLichThiId = {viewModel.SelectedLichThiId}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Referenceld == hocVienId && u.RoleId == 3);

            if (user == null)
            {
                Console.WriteLine($"Lỗi: Không tìm thấy user với HocVienId = {hocVienId}");
                return View(new ExamRegistrationViewModel
                {
                    RegistrationMessage = "Tài khoản học viên không tồn tại.",
                    AvailableKyThis = await _context.KyThis.ToListAsync()
                });
            }

            var hoSo = await _context.HoSoThiSinhs
                .FirstOrDefaultAsync(h => h.HocvienId == hocVienId);

            if (hoSo == null)
            {
                Console.WriteLine($"Lỗi: Không tìm thấy hồ sơ cho HocVienId = {hocVienId}");
                return View(new ExamRegistrationViewModel
                {
                    RegistrationMessage = "Bạn chưa có hồ sơ thi.",
                    AvailableKyThis = await _context.KyThis.ToListAsync()
                });
            }

            var ketQua = await _context.KetQuaHocTaps
                .FirstOrDefaultAsync(k => k.HosoId == hoSo.HosoId);

            if (ketQua == null)
            {
                Console.WriteLine($"Lỗi: Không tìm thấy kết quả học tập cho HosoId = {hoSo.HosoId}");
                return View(new ExamRegistrationViewModel
                {
                    RegistrationMessage = "Bạn chưa có kết quả học tập.",
                    AvailableKyThis = await _context.KyThis.ToListAsync()
                });
            }

            bool canRegisterGraduation = ketQua.Nhanxet == "Được thi tốt nghiệp";
            bool canRegisterOfficial = ketQua.Nhanxet == "Đậu tốt nghiệp" && ketQua.DauTn == "Đạt";
            string category = HangIdToCategory.TryGetValue(hoSo.HangId, out var cat) ? cat.ToLower() : string.Empty;

            var availableKyThis = await _context.KyThis
                .Where(k => k.HangId == hoSo.HangId &&
                            ((canRegisterGraduation && k.LoaithiId == 1) || (canRegisterOfficial && k.LoaithiId == 2)))
                .ToListAsync();

            if (!viewModel.SelectedKyThiId.HasValue || !availableKyThis.Any(k => k.KythiId == viewModel.SelectedKyThiId))
            {
                Console.WriteLine($"Lỗi: SelectedKyThiId = {viewModel.SelectedKyThiId} không hợp lệ.");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    AvailableKyThis = availableKyThis,
                    RegistrationMessage = "Vui lòng chọn kỳ thi phù hợp."
                });
            }

            var availableExams = await GetAvailableExams(hocVienId, canRegisterGraduation, canRegisterOfficial, category, viewModel.SelectedKyThiId);

            if (!ModelState.IsValid)
            {

                var errorMessages = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                Console.WriteLine($"Lỗi: ModelState không hợp lệ. Các lỗi: {errorMessages}");

                // Tạm thời bỏ qua ModelState để debug
                // Console.WriteLine("Debug: Bỏ qua ModelState để tiếp tục xử lý...");

                //Console.WriteLine("Lỗi: ModelState không hợp lệ. Các lỗi: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                //return View(new ExamRegistrationViewModel
                //{
                //    UserId = user.UserId,
                //    HocVienId = hocVienId,
                //    AvailableKyThis = availableKyThis,
                //    SelectedKyThiId = viewModel.SelectedKyThiId,
                //    AvailableExams = availableExams,
                //    RegistrationMessage = "Dữ liệu gửi lên không hợp lệ. Vui lòng thử lại."
                //});
            }

            if (!viewModel.SelectedLichThiId.HasValue)
            {
                Console.WriteLine("Lỗi: SelectedLichThiId không được chọn.");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    AvailableKyThis = availableKyThis,
                    SelectedKyThiId = viewModel.SelectedKyThiId,
                    AvailableExams = availableExams,
                    RegistrationMessage = "Vui lòng chọn lịch thi."
                });
            }

            var selectedExam = await _context.LichThis
                .FirstOrDefaultAsync(l => l.LichthiId == viewModel.SelectedLichThiId);

            if (selectedExam == null)
            {
                Console.WriteLine($"Lỗi: Không tìm thấy lịch thi với LichthiId = {viewModel.SelectedLichThiId}");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    AvailableKyThis = availableKyThis,
                    SelectedKyThiId = viewModel.SelectedKyThiId,
                    AvailableExams = availableExams,
                    RegistrationMessage = "Lịch thi không hợp lệ."
                });
            }

            if (ExtractCategoryFromGhichu(selectedExam.Ghichu) != category)
            {
                Console.WriteLine($"Lỗi: Hạng không khớp. Ghichu = {selectedExam.Ghichu}, category = {category}");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    AvailableKyThis = availableKyThis,
                    SelectedKyThiId = viewModel.SelectedKyThiId,
                    AvailableExams = availableExams,
                    RegistrationMessage = "Lịch thi không phù hợp với hạng bằng lái của bạn."
                });
            }

            var existingRegistration = await _context.CtDangKyThis
                .FirstOrDefaultAsync(ct => ct.HosoId == hoSo.HosoId && ct.LichthiId == viewModel.SelectedLichThiId);

            if (existingRegistration != null)
            {
                Console.WriteLine($"Lỗi: Lịch thi đã được đăng ký trước đó với LichthiId = {viewModel.SelectedLichThiId}");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    AvailableKyThis = availableKyThis,
                    SelectedKyThiId = viewModel.SelectedKyThiId,
                    AvailableExams = availableExams,
                    RegistrationMessage = "Bạn đã đăng ký lịch thi này."
                });
            }

            try
            {
                var registration = new CtDangKyThi
                {
                    KythiId = viewModel.SelectedKyThiId.Value,
                    HosoId = hoSo.HosoId,
                    Thoigiandk = DateTime.Now,
                    Thoigianthi = selectedExam.Thoigianthi,
                    LichthiId = viewModel.SelectedLichThiId
                };

                _context.CtDangKyThis.Add(registration);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Thành công: Đăng ký lịch thi với KythiId = {viewModel.SelectedKyThiId}, LichthiId = {viewModel.SelectedLichThiId}");

                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    AvailableKyThis = availableKyThis,
                    SelectedKyThiId = viewModel.SelectedKyThiId,
                    AvailableExams = new List<LichThi> { selectedExam },
                    RegistrationMessage = "Đăng ký lịch thi thành công!",
                    RegisteredExam = registration
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu dữ liệu: {ex.Message}");
                return View(new ExamRegistrationViewModel
                {
                    UserId = user.UserId,
                    HocVienId = hocVienId,
                    AvailableKyThis = availableKyThis,
                    SelectedKyThiId = viewModel.SelectedKyThiId,
                    AvailableExams = availableExams,
                    RegistrationMessage = "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại."
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyExamSchedule()
        {
            var hocVienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hocVienIdClaim, out int hocVienId))
            {
                return Unauthorized("Vui lòng đăng nhập.");
            }

            var hoSo = await _context.HoSoThiSinhs
                .FirstOrDefaultAsync(h => h.HocvienId == hocVienId);

            if (hoSo == null)
            {
                return View(new ExamRegistrationViewModel
                {
                    HocVienId = hocVienId,
                    RegistrationMessage = "Bạn chưa có hồ sơ thi."
                });
            }

            var registration = await _context.CtDangKyThis
                .Include(ct => ct.Lichthi)
                .Include(ct => ct.Kythi)
                .FirstOrDefaultAsync(ct => ct.HosoId == hoSo.HosoId);

            if (registration == null)
            {
                return View(new ExamRegistrationViewModel
                {
                    HocVienId = hocVienId,
                    RegistrationMessage = "Bạn chưa đăng ký lịch thi nào."
                });
            }

            return View(new ExamRegistrationViewModel
            {
                HocVienId = hocVienId,
                RegisteredExam = registration
            });
        }
    }
}