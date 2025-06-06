using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using doan3.ViewModels;
using doan3.Models;
using System.Security.Claims;

namespace doan3.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DacsGplxContext _context;

        public PaymentController(DacsGplxContext context)
        {
            _context = context;
        }
        //[]
        // GET: Payment/Thi
        public async Task<IActionResult> Thi()
        {
            var registrations = await _context.CtDangKyThis
                .Include(ct => ct.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(ct => ct.Kythi)
                    .ThenInclude(kt => kt.Hang)
                .Include(ct => ct.Lichthi)
                .Select(ct => new ThiViewModel
                {
                    ChiTietDangKyThiId = ct.CtDktId,
                    HoTenThiSinh = ct.Hoso.Hocvien.Tenhocvien,
                    TenKyThi = ct.Kythi.Tenkythi,
                    HangGPLX = ct.Kythi.Hang.Tenhang,
                    NgayHoc = ct.Lichthi != null ? ct.Lichthi.Thoigianthi : default, // Xử lý null cho Lichthi
                    ThanhToan = ct.thanhtoan,
                    PhiThi = ct.Kythi.Hang.PhiThi ?? 0 // Xử lý null cho PhiThi
                })
                .ToListAsync();

            var viewModel = new ThiPaymentViewModel
            {
                UnpaidRegistrations = registrations.Where(r => !r.ThanhToan).ToList(),
                PaidRegistrations = registrations.Where(r => r.ThanhToan).ToList()
            };

            return View(viewModel);
        }
        // GET: Payment/UserThi
        public async Task<IActionResult> UserThi()
        {
            // Lấy HocvienId từ claims
            var hocvienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                return Unauthorized("Không tìm thấy thông tin học viên.");
            }

            var registrations = await _context.CtDangKyThis
                .Include(ct => ct.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(ct => ct.Kythi)
                    .ThenInclude(kt => kt.Hang)
                .Include(ct => ct.Lichthi)
                .Where(ct => ct.Hoso.HocvienId == hocvienId) // Lọc theo HocvienId
                .Select(ct => new ThiViewModel
                {
                    ChiTietDangKyThiId = ct.CtDktId,
                    HoTenThiSinh = ct.Hoso.Hocvien.Tenhocvien,
                    TenKyThi = ct.Kythi.Tenkythi,
                    HangGPLX = ct.Kythi.Hang.Tenhang,
                    NgayHoc = ct.Lichthi != null ? ct.Lichthi.Thoigianthi : default,
                    ThanhToan = ct.thanhtoan,
                    PhiThi = ct.Kythi.Hang.PhiThi ?? 0
                })
                .ToListAsync();

            var viewModel = new ThiPaymentViewModel
            {
                UnpaidRegistrations = registrations.Where(r => !r.ThanhToan).ToList(),
                PaidRegistrations = registrations.Where(r => r.ThanhToan).ToList()
            };

            return View(viewModel);
        }
        // POST: Payment/ProcessPayment/{id}
        // POST: Payment/ProcessPayment/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int id)
        {
            var registration = await _context.CtDangKyThis.FindAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            // Kiểm tra xem học viên có quyền thanh toán bản ghi này không
            var hocvienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                var isValidUser = await _context.CtDangKyThis
                    .Include(ct => ct.Hoso)
                    .AnyAsync(ct => ct.CtDktId == id && ct.Hoso.HocvienId == hocvienId);
                if (!isValidUser)
                {
                    return Unauthorized("Bạn không có quyền thanh toán bản ghi này.");
                }
            }

            if (!registration.thanhtoan)
            {
                registration.thanhtoan = true;
                _context.Update(registration);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserThi));
        }
    }
}