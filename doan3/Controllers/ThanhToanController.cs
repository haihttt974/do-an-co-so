using Microsoft.AspNetCore.Mvc;
using doan3.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace doan3.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly DacsGplxContext _context;

        public ThanhToanController(DacsGplxContext context)
        {
            _context = context;
        }

        // Thanh toán khóa học
        public async Task<IActionResult> KhoaHoc()
        {
            var ketQuaList = await _context.KetQuaHocTaps
                .Include(k => k.Hoso).ThenInclude(h => h.Hocvien)
                .Include(k => k.Lop).ThenInclude(l => l.Khoahoc)
                .Where(k => k.Nhanxet == "Chưa thanh toán khóa học")
                .ToListAsync();

            return View(ketQuaList);
        }

        // Thanh toán thi
        public async Task<IActionResult> Thi()
        {
            var danhSach = await _context.CtDangKyThis
                .Include(ct => ct.Hoso).ThenInclude(h => h.Hocvien)
                .Include(ct => ct.Lichthi)
                .Where(ct => ct.thanhtoan == false)
                .ToListAsync();

            return View(danhSach);
        }

        // Xác nhận thanh toán
        public async Task<IActionResult> ConfirmPayment(int? ketQuaHocTapId, int? ctDangKyThiId)
        {
            if (ketQuaHocTapId != null)
            {
                var ketQua = await _context.KetQuaHocTaps
                    .Include(k => k.Hoso).ThenInclude(h => h.Hocvien)
                    .Include(k => k.Lop).ThenInclude(l => l.Khoahoc)
                    .FirstOrDefaultAsync(k => k.KetquaId == ketQuaHocTapId);

                if (ketQua == null) return NotFound();

                var hang = await _context.HangGplxes.FirstOrDefaultAsync(h => h.HangId == ketQua.Lop.Khoahoc.HangId);
                var amount = hang?.PhiDaotao ?? 0;

                ViewBag.HoSoId = ketQua.HosoId;
                ViewBag.KetQuaHocTapId = ketQua.KetquaId;
                ViewBag.HocVienName = ketQua.Hoso?.Hocvien?.Tenhocvien;
                ViewBag.Amount = amount;
            }
            else if (ctDangKyThiId != null)
            {
                var ct = await _context.CtDangKyThis
                    .Include(c => c.Hoso).ThenInclude(h => h.Hocvien)
                    .Include(c => c.Lichthi)
                    .FirstOrDefaultAsync(c => c.CtDktId == ctDangKyThiId);

                if (ct == null) return NotFound();

                var ghiChu = ct.Lichthi?.Ghichu?.Trim();
                var tenHang = ghiChu?.Split(' ').LastOrDefault()?.ToUpper();
                var hang = await _context.HangGplxes.FirstOrDefaultAsync(h => h.Tenhang.ToUpper() == tenHang);
                var amount = hang?.PhiThi ?? 0;

                ViewBag.CtDangKyThiId = ct.CtDktId;
                ViewBag.HoSoId = ct.HosoId;
                ViewBag.HocVienName = ct.Hoso?.Hocvien?.Tenhocvien;
                ViewBag.Amount = amount;
            }

            return View();
        }

        [HttpPost]
        public IActionResult ShowQRCode(int? hosoId, int? ketQuaHocTapId, int? ctDangKyThiId, string tenNguoiThanhToan, string phuongThuc, decimal soTien)
        {
            var maGiaoDich = Guid.NewGuid().ToString();

            ViewBag.HoSoId = hosoId;
            ViewBag.KetQuaHocTapId = ketQuaHocTapId;
            ViewBag.CtDangKyThiId = ctDangKyThiId;
            ViewBag.TenNguoiThanhToan = tenNguoiThanhToan;
            ViewBag.PhuongThuc = phuongThuc;
            ViewBag.SoTien = soTien;
            ViewBag.QRCode = maGiaoDich;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompletePayment(int? ketQuaHocTapId, int? ctDangKyThiId)
        {
            if (ketQuaHocTapId != null)
            {
                var ketQua = await _context.KetQuaHocTaps.FindAsync(ketQuaHocTapId);
                if (ketQua != null)
                {
                    ketQua.Nhanxet = "Đã thanh toán khóa học";
                }
            }
            else if (ctDangKyThiId != null)
            {
                var ct = await _context.CtDangKyThis.FindAsync(ctDangKyThiId);
                if (ct != null)
                {
                    ct.thanhtoan = true;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("PaymentSuccess");
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }
    }
}
