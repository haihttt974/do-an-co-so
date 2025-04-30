using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using doan3.Models;
using doan3.ViewModel;

namespace DrivingSchool.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DacsGplxContext _context;

        public HomeController(DacsGplxContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                Instructors = await _context.GiaoViens
                    .OrderBy(g => g.Tengiaovien)
                    .Take(4) // Limit to 4 instructors for the home page
                    .ToListAsync(),
                Courses = await _context.KhoaHocs
                    .OrderBy(k => k.Tenkhoahoc)
                    .Take(3) // Limit to 3 courses for the home page
                    .ToListAsync(),
                Testimonials = await _context.Phanhois
                    .Include(p => p.Hocvien) // Include HocVien to get student name
                    .OrderByDescending(p => p.Thoigianph) // Most recent first
                    .Take(3) // Limit to 3 testimonials for the home page
                    .ToListAsync()
            };
            return View(model);
        }
    }
}