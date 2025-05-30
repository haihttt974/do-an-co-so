using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doan3.Models;

namespace doan3.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly DacsGplxContext _context;

        public ThanhToanController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: ThanhToan
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Bạn cần đăng nhập.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 3);

            if (user == null || user.Referenceld == null)
            {
                return Unauthorized("Chỉ học viên mới có quyền truy cập.");
            }

            var hosoIds = await _context.HoSoThiSinhs
                .Where(h => h.HocvienId == user.Referenceld)
                .Select(h => h.HosoId)
                .ToListAsync();

            var hocPhis = await _context.KetQuaHocTaps
                .Where(kq => hosoIds.Contains(kq.HosoId) && kq.Nhanxet == "Chưa thanh toán khóa học")
                .Include(kq => kq.Hoso)
                .Include(kq => kq.Hoso.Hocvien)
                .ToListAsync();

            return View(hocPhis);
        }

        //// GET: ThanhToan/Pay/5
        //public async Task<IActionResult> Pay(int id)
        //{
        //    var ketQua = await _context.KetQuaHocTaps
        //        .Include(k => k.Hoso)
        //        .FirstOrDefaultAsync(k => k.KetquaId == id && k.Nhanxet == "Chưa thanh toán khóa học");

        //    if (ketQua == null)
        //    {
        //        return NotFound("Không tìm thấy thông tin thanh toán.");
        //    }

        //    // Giả sử: Tạo mới Phiếu thanh toán và chi tiết
        //    var thanhToan = new ThanhToan
        //    {
        //        TenThanhToan = $"Học phí hồ sơ #{ketQua.HosoId}",
        //        Sotien = 1000000, // Cập nhật theo logic thực tế
        //        Trangthai = "Đã thanh toán",
        //        Phuongthuc = "Chuyển khoản",
        //        Ghichu = $"Thanh toán học phí hồ sơ #{ketQua.HosoId}"
        //    };

        //    _context.ThanhToans.Add(thanhToan);
        //    await _context.SaveChangesAsync();

        //    var ct = new CtPhieuThanhToan
        //    {
        //        HosoId = ketQua.HosoId,
        //        ThanhtoanId = thanhToan.ThanhtoanId,
        //        Thoigianthanhtoan = DateTime.Now,
        //        Loaiphi = "Học phí"
        //    };

        //    _context.CtPhieuThanhToans.Add(ct);

        //    // Cập nhật trạng thái KetQuaHocTap
        //    ketQua.Nhanxet = null;
        //    await _context.SaveChangesAsync();

        //    TempData["Success"] = "Thanh toán thành công!";
        //    return RedirectToAction(nameof(Index));
        //}
        //// GET: ThanhToan/ConfirmPayment/5
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var ketQua = await _context.KetQuaHocTaps
                .Include(k => k.Hoso)
                .ThenInclude(h => h.Hocvien)
                .FirstOrDefaultAsync(k => k.KetquaId == id && k.Nhanxet == "Chưa thanh toán khóa học");

            if (ketQua == null)
            {
                return NotFound();
            }

            ViewBag.KetQuaHocTapId = id;
            ViewBag.HocVienName = ketQua.Hoso.Hocvien?.Tenhocvien;
            ViewBag.HoSoId = ketQua.HosoId;
            ViewBag.Amount = 1000000; // Số tiền giả định

            return View();
        }

        // POST: ThanhToan/ShowQRCode
        [HttpPost]
        public IActionResult ShowQRCode(string tenNguoiThanhToan, string phuongThuc, int hosoId, int ketQuaHocTapId, decimal soTien)
        {
            var qrCode = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
            ViewBag.QRCode = qrCode;

            ViewBag.TenNguoiThanhToan = tenNguoiThanhToan;
            ViewBag.PhuongThuc = phuongThuc;
            ViewBag.HoSoId = hosoId;
            ViewBag.KetQuaHocTapId = ketQuaHocTapId;
            ViewBag.SoTien = soTien;

            return View();
        }

        // POST: ThanhToan/CompletePayment
        [HttpPost]
        public async Task<IActionResult> CompletePayment(int hosoId, int ketQuaHocTapId, string tenNguoiThanhToan, string phuongThuc, decimal soTien)
        {
            var thanhToan = new ThanhToan
            {
                TenThanhToan = $"Thanh toán học phí bởi {tenNguoiThanhToan}",
                Sotien = soTien,
                Trangthai = "Đã thanh toán",
                Phuongthuc = phuongThuc,
                Ghichu = $"Thanh toán hồ sơ {hosoId}"
            };

            _context.ThanhToans.Add(thanhToan);
            await _context.SaveChangesAsync();

            var ct = new CtPhieuThanhToan
            {
                HosoId = hosoId,
                ThanhtoanId = thanhToan.ThanhtoanId,
                Thoigianthanhtoan = DateTime.Now,
                Loaiphi = "Học phí"
            };

            _context.CtPhieuThanhToans.Add(ct);

            var ketQua = await _context.KetQuaHocTaps.FindAsync(ketQuaHocTapId);
            if (ketQua != null)
            {
                ketQua.Nhanxet = null;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Thanh toán thành công!";
            return RedirectToAction("Index");
        }
    }
}
