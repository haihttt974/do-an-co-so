using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using doan3.Models;

namespace doan3.Controllers
{
    public class ReportController : Controller
    {
        private readonly DacsGplxContext _context;

        public ReportController(DacsGplxContext context)
        {
            _context = context;
        }

        // Trang chính (chuyển hướng)
        public IActionResult Index()
        {
            return View();
        }

        // Báo cáo thống kê lớp học (Điểm danh)
        public IActionResult LopHocStats()
        {
            var lopHocData = _context.LopHocs
                .Select(l => new
                {
                    LopId = l.LopId,
                    TenLop = l.Tenlop,
                    SoHocVien = l.KetQuaHocTaps.Count(k => k.Hoso.Hocvien != null),
                    TotalSessions = _context.LichHocs.Count(lh => lh.LopId == l.LopId),
                    AttendedSessions = _context.DiemDanhs.Count(d => d.Lichhoc.LopId == l.LopId && d.Trangthai == "Có mặt")
                })
                .ToList();

            var model = new ReportViewModel
            {
                LopHocStats = lopHocData
                    .Select(l => new LopHocStat
                    {
                        LopId = l.LopId,
                        TenLop = l.TenLop,
                        SoHocVien = l.SoHocVien,
                        TiLeDiemDanh = l.TotalSessions == 0 ? 0 : (double)l.AttendedSessions / l.TotalSessions * 100
                    })
                    .ToList()
            };
            return View(model);
        }

        // Báo cáo thống kê lớp học mới (Số buổi, Học viên đạt yêu cầu)
        public IActionResult LopHocStatsNew()
        {
            var lopHocData = _context.LopHocs
                .Select(l => new
                {
                    LopId = l.LopId,
                    TenLop = l.Tenlop,
                    SoBuoiHoc = _context.LichHocs.Count(lh => lh.LopId == l.LopId),
                    SoHocVienDat = l.KetQuaHocTaps.Count(k => k.Sobuoiodahoc >= k.Sobuoitoithieu && k.Hoso.Hocvien != null)
                })
                .ToList();

            var model = new LopHocStatsNewViewModel
            {
                LopHocStatsNew = lopHocData
                    .Select(l => new LopHocStatNew
                    {
                        LopId = l.LopId,
                        TenLop = l.TenLop,
                        SoBuoiHoc = l.SoBuoiHoc,
                        SoHocVienDat = l.SoHocVienDat
                    })
                    .ToList()
            };
            return View(model);
        }

        // Báo cáo thống kê giấy phép
        public IActionResult GiayPhepStats()
        {
            var giayPhepData = _context.GiayPhepLaiXes
                .GroupBy(g => new { g.Ngaycap.Year, g.Ngaycap.Month, g.Hang.Tenhang })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Hang = g.Key.Tenhang,
                    SoLuong = g.Count()
                })
                .ToList();

            var model = new GiayPhepViewModel
            {
                GiayPhepStats = giayPhepData
                    .Select(g => new GiayPhepStat
                    {
                        Nam = g.Year,
                        Thang = g.Month,
                        Hang = g.Hang,
                        SoLuong = g.SoLuong
                    })
                    .ToList()
            };
            return View(model);
        }

        // Báo cáo hiệu quả đào tạo
        public IActionResult HieuQuaDaoTao()
        {
            var hieuQuaData = _context.LopHocs
                .Select(l => new
                {
                    LopId = l.LopId,
                    TenLop = l.Tenlop,
                    TotalHocVien = l.KetQuaHocTaps.Count(k => k.Hoso.Hocvien != null),
                    DatYeuCau = l.KetQuaHocTaps.Count(k => k.DauTn == "Đậu" && k.Sobuoiodahoc >= k.Sobuoitoithieu)
                })
                .ToList();

            var model = new HieuQuaDaoTaoViewModel
            {
                HieuQuaStats = hieuQuaData
                    .Select(h => new HieuQuaStat
                    {
                        LopId = h.LopId,
                        TenLop = h.TenLop,
                        TongHocVien = h.TotalHocVien,
                        TiLeDat = h.TotalHocVien == 0 ? 0 : (double)h.DatYeuCau / h.TotalHocVien * 100
                    })
                    .ToList()
            };
            return View(model);
        }

        // Báo cáo điểm danh theo giáo viên
        public IActionResult DiemDanhTheoGiaoVien()
        {
            var diemDanhData = _context.LopHocs
                .Where(l => l.GiaovienId != null)
                .Select(l => new
                {
                    GiaoVienId = l.GiaovienId,
                    TenGiaoVien = l.Giaovien.Tengiaovien,
                    TotalSessions = _context.LichHocs.Count(lh => lh.LopId == l.LopId),
                    AttendedSessions = _context.DiemDanhs.Count(d => d.Lichhoc.LopId == l.LopId && d.Trangthai == "Có mặt")
                })
                .ToList();

            var model = new DiemDanhGiaoVienViewModel
            {
                DiemDanhStats = diemDanhData
                    .Select(d => new DiemDanhStat
                    {
                        GiaoVienId = d.GiaoVienId ?? 0,
                        TenGiaoVien = d.TenGiaoVien,
                        TiLeDiemDanh = d.TotalSessions == 0 ? 0 : (double)d.AttendedSessions / d.TotalSessions * 100
                    })
                    .ToList()
            };
            return View(model);
        }
    }
}