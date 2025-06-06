using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using doan3.ViewModels;
using doan3.Models;
using System.Security.Claims;
using doan3.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace doan3.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DacsGplxContext _context;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(DacsGplxContext context, ILogger<PaymentController> logger)
        {
            _context = context;
            _logger = logger;
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
        [Authorize]
        public async Task<IActionResult> UserKhoaHoc()
        {
            var hocvienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                _logger.LogError("Không tìm thấy HocvienId trong claims.");
                return Unauthorized("Không tìm thấy thông tin học viên.");
            }

            _logger.LogInformation($"HocvienId từ claims: {hocvienId}");

            var results = await _context.KetQuaHocTaps
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(kq => kq.Lop)
                    .ThenInclude(lh => lh != null ? lh.Khoahoc : null)
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hang)
                .Where(kq => kq.Hoso != null && kq.Hoso.HocvienId == hocvienId)
                .Select(kq => new Models.KhoaHocViewModel
                {
                    KetQuaHocTapId = kq.KetquaId,
                    TenKhoaHoc = kq.Lop != null && kq.Lop.Khoahoc != null ? kq.Lop.Khoahoc.Tenkhoahoc : "Không có khóa học",
                    TenLopHoc = kq.Lop != null ? kq.Lop.Tenlop : "Không có lớp",
                    HangGPLX = kq.Hoso != null && kq.Hoso.Hang != null ? kq.Hoso.Hang.Tenhang : "Không có hạng",
                    NgayHoc = kq.Lop != null && kq.Lop.Khoahoc != null ? kq.Lop.Khoahoc.Ngaybatdau.ToDateTime(TimeOnly.MinValue) : default,
                    NhanXet = kq.Nhanxet ?? "Chưa có nhận xét",
                    PhiDaoTao = kq.Hoso != null && kq.Hoso.Hang != null ? kq.Hoso.Hang.PhiDaotao ?? 0 : 0,
                    HoTenThiSinh = kq.Hoso != null && kq.Hoso.Hocvien != null ? kq.Hoso.Hocvien.Tenhocvien : "Không có học viên"
                })
                .ToListAsync();

            _logger.LogInformation($"Số bản ghi KetQuaHocTap tìm thấy cho HocvienId {hocvienId}: {results.Count}");
            if (!results.Any(r => r.NhanXet == "Chưa thanh toán khóa học"))
            {
                _logger.LogWarning($"Không tìm thấy khóa học chưa thanh toán cho HocvienId {hocvienId}.");
            }

            var viewModel = new KhoaHocPaymentViewModel
            {
                UnpaidKhoaHocs = results.Where(r => r.NhanXet == "Chưa thanh toán khóa học").ToList(),
                PaidKhoaHocs = results.Where(r => r.NhanXet != "Chưa thanh toán khóa học").ToList()
            };

            return View(viewModel);
        }

        // POST: Payment/ProcessUserKhoaHocPayment/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessUserKhoaHocPayment(int id)
        {
            var ketQua = await _context.KetQuaHocTaps
                .Include(kq => kq.Hoso)
                .FirstOrDefaultAsync(kq => kq.KetquaId == id);
            if (ketQua == null)
            {
                return NotFound();
            }

            var hocvienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                if (ketQua.Hoso.HocvienId != hocvienId)
                {
                    return Unauthorized("Bạn không có quyền thanh toán bản ghi này.");
                }
            }

            if (ketQua.Nhanxet == "Chưa thanh toán khóa học")
            {
                ketQua.Nhanxet = "Đã thanh toán khóa học";
                _context.Update(ketQua);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserKhoaHoc));
        }

        // POST: Payment/ProcessKhoaHocPayment/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessKhoaHocPayment(int id)
        {
            var ketQua = await _context.KetQuaHocTaps
                .Include(kq => kq.Hoso)
                .FirstOrDefaultAsync(kq => kq.KetquaId == id);
            if (ketQua == null)
            {
                return NotFound();
            }

            var hocvienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                if (ketQua.Hoso.HocvienId != hocvienId)
                {
                    return Unauthorized("Bạn không có quyền thanh toán bản ghi này.");
                }
            }

            if (ketQua.Nhanxet == "Chưa thanh toán khóa học")
            {
                ketQua.Nhanxet = "Đã thanh toán khóa học";
                _context.Update(ketQua);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(KhoaHoc));
        }
        // GET: Payment/AdminKhoaHoc
        [Authorize(Roles = "1")] // Giả sử RoleId = 1 là admin
        public async Task<IActionResult> KhoaHoc()
        {
            var results = await _context.KetQuaHocTaps
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(kq => kq.Lop)
                    .ThenInclude(lh => lh.Khoahoc)
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hang)
                .Select(kq => new Models.KhoaHocViewModel
                {
                    KetQuaHocTapId = kq.KetquaId,
                    TenKhoaHoc = kq.Lop.Khoahoc.Tenkhoahoc,
                    TenLopHoc = kq.Lop.Tenlop,
                    HangGPLX = kq.Hoso.Hang.Tenhang,
                    NgayHoc = kq.Lop.Khoahoc.Ngaybatdau.ToDateTime(TimeOnly.MinValue),
                    NhanXet = kq.Nhanxet ?? "Chưa có nhận xét",
                    PhiDaoTao = kq.Hoso.Hang.PhiDaotao ?? 0,
                    HoTenThiSinh = kq.Hoso.Hocvien.Tenhocvien
                })
                .ToListAsync();

            var viewModel = new KhoaHocPaymentViewModel
            {
                UnpaidKhoaHocs = results.Where(r => r.NhanXet == "Chưa thanh toán khóa học").ToList(),
                PaidKhoaHocs = results.Where(r => r.NhanXet != "Chưa thanh toán khóa học").ToList()
            };

            return View(viewModel);
        }

        // POST: Payment/ProcessAdminKhoaHocPayment/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")] // Chỉ admin có quyền
        public async Task<IActionResult> ProcessAdminKhoaHocPayment(int id)
        {
            var ketQua = await _context.KetQuaHocTaps.FindAsync(id);
            if (ketQua == null)
            {
                return NotFound();
            }

            if (ketQua.Nhanxet == "Chưa thanh toán khóa học")
            {
                ketQua.Nhanxet = "Đã thanh toán khóa học";
                _context.Update(ketQua);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(KhoaHoc));
        }
    }
}