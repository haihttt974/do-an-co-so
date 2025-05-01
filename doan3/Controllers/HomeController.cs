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
                    .Take(4)
                    .Select(g => new GiaoVien
                    {
                        GiaovienId = g.GiaovienId,
                        Tengiaovien = g.Tengiaovien,
                        Chuyenmon = g.Chuyenmon,
                        HangDaotao = g.HangDaotao,
                        Ngaybatdaulamviec = g.Ngaybatdaulamviec,
                        ImgGv = g.ImgGv ?? "/img/logo.png"
                    })
                    .ToListAsync(),
                Courses = await _context.KhoaHocs
                    .OrderBy(k => k.Tenkhoahoc)
                    .Take(3)
                    .Select(k => new KhoaHoc
                    {
                        KhoahocId = k.KhoahocId,
                        HangId = k.HangId,
                        Tenkhoahoc = k.Tenkhoahoc,
                        Ngaybatdau = k.Ngaybatdau,
                        Ngayketthuc = k.Ngayketthuc,
                        SlToida = k.SlToida,
                        Trangthai = k.Trangthai,
                        Mota = k.Mota,
                        SoLuongConLai = k.SlToida - (from lh in _context.LopHocs
                                                     join kq in _context.KetQuaHocTaps on lh.LopId equals kq.LopId
                                                     where lh.KhoahocId == k.KhoahocId
                                                     select kq.HosoId)
                                                      .Distinct()
                                                      .Count()
                    })
                    .ToListAsync(),
                Testimonials = await _context.Phanhois
                    .Include(p => p.Hocvien)
                    .OrderByDescending(p => p.Thoigianph)
                    .Take(3)
                    .ToListAsync()
            };
            return View(model);
        }
    }
}