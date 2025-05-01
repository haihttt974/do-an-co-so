using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using doan3.Data;  // Thay thế bằng namespace của bạn
using doan3.Models;  // Thay thế bằng namespace model của bạn

namespace doan3.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DacsGplxContext _context;

        public DashboardController(DacsGplxContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy số lượng các mục cần hiển thị trên Dashboard
            var dashboardData = new DashboardViewModel
            {
                CoursesCount = await _context.KhoaHocs.CountAsync(),
                DocumentsCount = await _context.TaiLieus.CountAsync(),
                ExamsCount = await _context.KyThis.CountAsync(),
                ExamSchedulesCount = await _context.LichThis.CountAsync(),
                SupervisorAssignmentsCount = await _context.PhanCongGiamSats.CountAsync(),
                DrivingSchedulesCount = await _context.LichTapLais.CountAsync(),
                FeedbackCount = await _context.Phanhois.CountAsync(),
                TeachersCount = await _context.GiaoViens.CountAsync(),
                StudentsCount = await _context.HocViens.CountAsync(),
                CandidateProfilesCount = await _context.HoSoThiSinhs.CountAsync(),
                ClassCount = await _context.LopHocs.CountAsync()
            };

            return View(dashboardData);
        }
    }
}
