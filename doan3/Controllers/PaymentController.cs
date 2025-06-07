using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using doan3.ViewModels;
using doan3.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Data.Common;

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

        // GET: Payment/Thi (Admin view only)
        [Authorize(Roles = "1")]
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

        // GET: Payment/UserThi
        [Authorize]
        public async Task<IActionResult> UserThi()
        {
            var hocvienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                _logger.LogError("Không tìm thấy HocvienId trong claims.");
                return Unauthorized("Không tìm thấy thông tin học viên.");
            }

            _logger.LogInformation($"HocvienId từ claims: {hocvienId}");

            var registrations = await _context.CtDangKyThis
                .Include(ct => ct.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(ct => ct.Kythi)
                    .ThenInclude(kt => kt.Hang)
                .Include(ct => ct.Lichthi)
                .Where(ct => ct.Hoso != null && ct.Hoso.HocvienId == hocvienId)
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

            _logger.LogInformation($"Số bản ghi CtDangKyThi tìm thấy cho HocvienId {hocvienId}: {registrations.Count}");

            var viewModel = new ThiPaymentViewModel
            {
                UnpaidRegistrations = registrations.Where(r => !r.ThanhToan).ToList(),
                PaidRegistrations = registrations.Where(r => r.ThanhToan).ToList()
            };

            return View(viewModel);
        }

        // GET: Payment/ConfirmPayment
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ConfirmPayment(int id, string type)
        {
            var hocvienIdClaim = User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .LastOrDefault()?.Value;

            if (!int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                _logger.LogError("Invalid HocvienId claim for ConfirmPayment. Id: {Id}, Type: {Type}", id, type);
                return Unauthorized("Không tìm thấy thông tin học viên.");
            }

            var model = new ConfirmPaymentViewModel
            {
                Id = id,
                Type = type,
                PaymentMethod = "Momo"
            };

            try
            {
                if (type == "Thi")
                {
                    var registration = await _context.CtDangKyThis
                        .Include(ct => ct.Hoso)
                        .Include(ct => ct.Kythi)
                            .ThenInclude(kt => kt.Hang)
                        .FirstOrDefaultAsync(ct => ct.CtDktId == id && ct.Hoso.HocvienId == hocvienId && !ct.thanhtoan);

                    if (registration == null)
                    {
                        _logger.LogError("No registration found for CtDktId: {Id}, HocvienId: {HocvienId}", id, hocvienId);
                        return NotFound("Không tìm thấy đăng ký thi hoặc đã thanh toán.");
                    }

                    _logger.LogInformation("Registration found. CtDktId: {Id}, HosoId: {HosoId}, HocvienId: {HocvienId}",
                        id, registration.HosoId, hocvienId);

                    model.Amount = registration.Kythi.Hang.PhiThi ?? 0;
                    model.Description = $"Thanh toán phí thi {registration.Kythi.Tenkythi}";
                    model.HosoId = registration.HosoId;
                }
                else if (type == "KhoaHoc")
                {
                    var ketQua = await _context.KetQuaHocTaps
                        .Include(kq => kq.Hoso)
                        .Include(kq => kq.Lop)
                            .ThenInclude(lh => lh.Khoahoc)
                        .Include(kq => kq.Hoso)
                            .ThenInclude(hs => hs.Hang)
                        .FirstOrDefaultAsync(kq => kq.KetquaId == id && kq.Hoso.HocvienId == hocvienId && kq.Nhanxet == "Chưa thanh toán khóa học");

                    if (ketQua == null)
                    {
                        _logger.LogError("No KetQuaHocTap found for KetquaId: {Id}, HocvienId: {HocvienId}", id, hocvienId);
                        return NotFound("Không tìm thấy kết quả học tập hoặc đã thanh toán.");
                    }

                    _logger.LogInformation("KetQuaHocTap found. KetquaId: {Id}, HosoId: {HosoId}, HocvienId: {HocvienId}, Nhanxet: {Nhanxet}",
                        id, ketQua.HosoId, hocvienId, ketQua.Nhanxet);

                    model.Amount = ketQua.Lop.Khoahoc.Hang.PhiDaotao ?? 0;
                    model.Description = $"Thanh toán học phí khóa học {ketQua.Lop.Khoahoc.Tenkhoahoc}";
                    model.HosoId = ketQua.HosoId;
                }
                else
                {
                    _logger.LogError("Invalid Type value: {Type}", type);
                    return BadRequest("Loại thanh toán không hợp lệ.");
                }

                return View(model);
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error in ConfirmPayment GET for Id: {Id}, Type: {Type}", id, type);
                return StatusCode(500, $"Lỗi cơ sở dữ liệu: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ConfirmPayment GET for Id: {Id}, Type: {Type}", id, type);
                return StatusCode(500, "Đã xảy ra lỗi không mong đợi.");
            }
        }

        //// POST: Payment/ConfirmPayment
        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ConfirmPayment(ConfirmPaymentViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogWarning("ModelState is invalid. Errors: {Errors}",
        //            string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        //        return View(model);
        //    }

        //    try
        //    {
        //        _logger.LogInformation("Processing ConfirmPayment POST. Id: {Id}, Type: {Type}, HosoId: {HosoId}, Amount: {Amount}, PaymentMethod: {PaymentMethod}, Description: {Description}",
        //            model.Id, model.Type, model.HosoId, model.Amount, model.PaymentMethod, model.Description);

        //        if (string.IsNullOrEmpty(model.Type) || (model.Type != "Thi" && model.Type != "KhoaHoc"))
        //        {
        //            _logger.LogError("Invalid Type value: {Type}", model.Type);
        //            ModelState.AddModelError("", "Loại thanh toán không hợp lệ.");
        //            return View(model);
        //        }

        //        if (model.HosoId <= 0)
        //        {
        //            _logger.LogError("Invalid HosoId: {HosoId}", model.HosoId);
        //            ModelState.AddModelError("", "Thông tin hồ sơ không hợp lệ.");
        //            return View(model);
        //        }

        //        if (model.Amount <= 0)
        //        {
        //            _logger.LogError("Invalid Amount: {Amount}", model.Amount);
        //            ModelState.AddModelError("", "Số tiền thanh toán phải lớn hơn 0.");
        //            return View(model);
        //        }

        //        if (string.IsNullOrEmpty(model.PaymentMethod))
        //        {
        //            _logger.LogError("PaymentMethod is empty");
        //            ModelState.AddModelError("PaymentMethod", "Vui lòng chọn phương thức thanh toán.");
        //            return View(model);
        //        }

        //        var hosoExists = await _context.HoSoThiSinhs.AnyAsync(h => h.HosoId == model.HosoId);
        //        if (!hosoExists)
        //        {
        //            _logger.LogError("HosoId {HosoId} does not exist in Hosos table", model.HosoId);
        //            ModelState.AddModelError("", "Hồ sơ không tồn tại trong hệ thống.");
        //            return View(model);
        //        }

        //        using var transaction = await _context.Database.BeginTransactionAsync();
        //        try
        //        {
        //            var thanhToan = new ThanhToan
        //            {
        //                TenThanhToan = model.Description ?? $"Thanh toán {model.Type}",
        //                Sotien = model.Amount,
        //                Trangthai = "Chưa thanh toán",
        //                Phuongthuc = model.PaymentMethod,
        //                Ghichu = $"Thanh toán {model.Type} qua {model.PaymentMethod}"
        //            };

        //            _logger.LogInformation("Attempting to insert ThanhToan: {ThanhToan}",
        //                Newtonsoft.Json.JsonConvert.SerializeObject(thanhToan));

        //            _context.ThanhToans.Add(thanhToan);
        //            await _context.SaveChangesAsync();
        //            _logger.LogInformation("ThanhToan record created. ThanhToanId: {ThanhToanId}", thanhToan.ThanhtoanId);

        //            var ctPhieu = new CtPhieuThanhToan
        //            {
        //                HosoId = model.HosoId,
        //                ThanhtoanId = thanhToan.ThanhtoanId,
        //                Thoigianthanhtoan = DateTime.Now,
        //                Loaiphi = model.Type
        //            };

        //            _logger.LogInformation("Attempting to insert CtPhieuThanhToan: {CtPhieuThanhToan}",
        //                Newtonsoft.Json.JsonConvert.SerializeObject(ctPhieu));

        //            _context.CtPhieuThanhToans.Add(ctPhieu);
        //            await _context.SaveChangesAsync();
        //            _logger.LogInformation("CtPhieuThanhToan record created. HosoId: {HosoId}, ThanhToanId: {ThanhtoanId}",
        //                ctPhieu.HosoId, ctPhieu.ThanhtoanId);

        //            await transaction.CommitAsync();

        //            _logger.LogInformation("Redirecting to ShowQRCode with Id: {Id}, Type: {Type}, ThanhToanId: {ThanhToanId}",
        //                model.Id, model.Type, thanhToan.ThanhtoanId);

        //            return RedirectToAction("ShowQRCode", new { id = model.Id, type = model.Type, thanhToanId = thanhToan.ThanhtoanId });
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            _logger.LogError(ex, "Error during transaction for Id: {Id}, Type: {Type}. Details: {Message}",
        //                model.Id, model.Type, ex.Message);
        //            throw;
        //        }
        //    }
        //    catch (DbUpdateException dbEx)
        //    {
        //        string errorDetails = dbEx.InnerException?.Message ?? dbEx.Message;
        //        if (dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
        //        {
        //            errorDetails += $"; SQL Error Number: {sqlEx.Number}; SQL State: {sqlEx.State}";
        //        }
        //        _logger.LogError(dbEx, "Database error in ConfirmPayment POST for Id: {Id}, Type: {Type}. Details: {Details}",
        //            model.Id, model.Type, errorDetails);
        //        ModelState.AddModelError("", $"Lỗi cơ sở dữ liệu: {errorDetails}");
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error in ConfirmPayment POST for Id: {Id}, Type: {Type}. Details: {Message}",
        //            model.Id, model.Type, ex.Message);
        //        ModelState.AddModelError("", $"Đã xảy ra lỗi không mong muốn: {ex.Message}");
        //        return View(model);
        //    }
        //}
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(ConfirmPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            try
            {
                _logger.LogInformation("Processing ConfirmPayment POST. Id: {Id}, Type: {Type}, HosoId: {HosoId}, Amount: {Amount}, PaymentMethod: {PaymentMethod}, Description: {Description}",
                    model.Id, model.Type, model.HosoId, model.Amount, model.PaymentMethod, model.Description);

                if (string.IsNullOrEmpty(model.Type) || (model.Type != "Thi" && model.Type != "KhoaHoc"))
                {
                    _logger.LogError("Invalid Type value: {Type}", model.Type);
                    ModelState.AddModelError("", "Loại thanh toán không hợp lệ.");
                    return View(model);
                }

                if (model.HosoId <= 0)
                {
                    _logger.LogError("Invalid HosoId: {HosoId}", model.HosoId);
                    ModelState.AddModelError("", "Thông tin hồ sơ không hợp lệ.");
                    return View(model);
                }

                if (model.Amount <= 0)
                {
                    _logger.LogError("Invalid Amount: {Amount}", model.Amount);
                    ModelState.AddModelError("", "Số tiền thanh toán phải lớn hơn 0.");
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.PaymentMethod))
                {
                    _logger.LogError("PaymentMethod is empty");
                    ModelState.AddModelError("PaymentMethod", "Vui lòng chọn phương thức thanh toán.");
                    return View(model);
                }

                // Lấy hocvienId từ claim
                var hocvienIdClaim = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).LastOrDefault()?.Value;
                if (!int.TryParse(hocvienIdClaim, out var hocvienId))
                {
                    _logger.LogError("Invalid HocvienId claim.");
                    ModelState.AddModelError("", "Không tìm thấy thông tin học viên.");
                    return View(model);
                }

                // Kiểm tra dữ liệu tùy loại thanh toán
                if (model.Type == "Thi")
                {
                    var registration = await _context.CtDangKyThis
                        .Include(ct => ct.Hoso)
                        .FirstOrDefaultAsync(ct => ct.CtDktId == model.Id && ct.Hoso.HocvienId == hocvienId);

                    if (registration == null)
                    {
                        _logger.LogError("Không tìm thấy đăng ký thi phù hợp. CtDktId: {Id}, HocvienId: {HocvienId}", model.Id, hocvienId);
                        ModelState.AddModelError("", "Không tìm thấy đăng ký thi phù hợp.");
                        return View(model);
                    }
                }
                else if (model.Type == "KhoaHoc")
                {
                    var ketQua = await _context.KetQuaHocTaps
                        .Include(kq => kq.Hoso)
                        .FirstOrDefaultAsync(kq => kq.KetquaId == model.Id && kq.Hoso.HocvienId == hocvienId && kq.Nhanxet == "Chưa thanh toán khóa học");

                    if (ketQua == null)
                    {
                        _logger.LogError("Không tìm thấy kết quả học tập phù hợp hoặc đã thanh toán. KetquaId: {Id}, HocvienId: {HocvienId}", model.Id, hocvienId);
                        ModelState.AddModelError("", "Không tìm thấy kết quả học tập phù hợp hoặc đã thanh toán.");
                        return View(model);
                    }
                }
                else
                {
                    _logger.LogError("Loại thanh toán không hợp lệ: {Type}", model.Type);
                    ModelState.AddModelError("", "Loại thanh toán không hợp lệ.");
                    return View(model);
                }

                // Kiểm tra tồn tại HosoId trong bảng HoSoThiSinhs
                var hosoExists = await _context.HoSoThiSinhs.AnyAsync(h => h.HosoId == model.HosoId);
                if (!hosoExists)
                {
                    _logger.LogError("HosoId {HosoId} does not exist in HoSoThiSinhs table", model.HosoId);
                    ModelState.AddModelError("", "Hồ sơ không tồn tại trong hệ thống.");
                    return View(model);
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var thanhToan = new ThanhToan
                    {
                        TenThanhToan = model.Description ?? $"Thanh toán {model.Type}",
                        Sotien = model.Amount,
                        Trangthai = "Chưa thanh toán",
                        Phuongthuc = model.PaymentMethod,
                        Ghichu = $"Thanh toán {model.Type} qua {model.PaymentMethod}"
                    };

                    _context.ThanhToans.Add(thanhToan);
                    await _context.SaveChangesAsync();

                    var ctPhieu = new CtPhieuThanhToan
                    {
                        HosoId = model.HosoId,
                        ThanhtoanId = thanhToan.ThanhtoanId,
                        Thoigianthanhtoan = DateTime.Now,
                        Loaiphi = model.Type
                    };

                    _context.CtPhieuThanhToans.Add(ctPhieu);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return RedirectToAction("ShowQRCode", new { id = model.Id, type = model.Type, thanhToanId = thanhToan.ThanhtoanId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during transaction for Id: {Id}, Type: {Type}. Details: {Message}",
                        model.Id, model.Type, ex.Message);
                    throw;
                }
            }
            catch (DbUpdateException dbEx)
            {
                string errorDetails = dbEx.InnerException?.Message ?? dbEx.Message;
                if (dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                {
                    errorDetails += $"; SQL Error Number: {sqlEx.Number}; SQL State: {sqlEx.State}";
                }
                _logger.LogError(dbEx, "Database error in ConfirmPayment POST for Id: {Id}, Type: {Type}. Details: {Details}",
                    model.Id, model.Type, errorDetails);
                ModelState.AddModelError("", $"Lỗi cơ sở dữ liệu: {errorDetails}");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ConfirmPayment POST for Id: {Id}, Type: {Type}. Details: {Message}",
                    model.Id, model.Type, ex.Message);
                ModelState.AddModelError("", $"Đã xảy ra lỗi không mong muốn: {ex.Message}");
                return View(model);
            }
        }

        // GET: Payment/ShowQRCode
        [Authorize]
        public async Task<IActionResult> ShowQRCode(int id, string type, int thanhToanId)
        {
            var hocvienIdClaim = User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .LastOrDefault()?.Value;
            if (!int.TryParse(hocvienIdClaim, out var hocvienId))
            {
                _logger.LogError("Invalid HocvienId claim for ShowQRCode. Id: {Id}, Type: {Type}, ThanhToanId: {ThanhToanId}",
                    id, type, thanhToanId);
                return Unauthorized("Không tìm thấy thông tin học viên.");
            }

            try
            {
                var thanhToan = await _context.ThanhToans.FindAsync(thanhToanId);
                if (thanhToan == null)
                {
                    _logger.LogError("ThanhToan record not found for ThanhToanId: {ThanhToanId}. Id: {Id}, Type: {Type}",
                        thanhToanId, id, type);
                    return NotFound("Giao dịch không tồn tại.");
                }

                if (thanhToan.Trangthai != "Chưa thanh toán")
                {
                    _logger.LogError("ThanhToan record has invalid status. ThanhToanId: {ThanhToanId}, Trangthai: {Trangthai}, Id: {Id}, Type: {Type}",
                        thanhToanId, thanhToan.Trangthai, id, type);
                    return NotFound("Giao dịch đã được xử lý.");
                }

                _logger.LogInformation("ThanhToan record found. ThanhToanId: {ThanhToanId}, Trangthai: {Trangthai}, Sotien: {Sotien}, Phuongthuc: {Phuongthuc}",
                    thanhToanId, thanhToan.Trangthai, thanhToan.Sotien, thanhToan.Phuongthuc);

                var model = new QRCodeViewModel
                {
                    Id = id,
                    Type = type,
                    ThanhToanId = thanhToanId,
                    PaymentMethod = thanhToan.Phuongthuc,
                    Amount = thanhToan.Sotien,
                    Description = thanhToan.TenThanhToan
                };

                var callbackUrl = Url.Action("Success", "Payment", new { type }, Request.Scheme);
                model.QRCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={Uri.EscapeDataString(callbackUrl)}";

                return View(model);
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error in ShowQRCode for ThanhToanId: {ThanhToanId}, Id: {Id}, Type: {Type}",
                    thanhToanId, id, type);
                return StatusCode(500, $"Lỗi cơ sở dữ liệu: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ShowQRCode for ThanhToanId: {ThanhToanId}, Id: {Id}, Type: {Type}",
                    thanhToanId, id, type);
                return StatusCode(500, "Đã xảy ra lỗi không mong đợi.");
            }
        }

        // GET: Payment/ProcessPaymentCallback
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProcessPaymentCallback(int id, string type, int thanhToanId, string hocvienId)
        {
            if (!int.TryParse(hocvienId, out var parsedHocvienId))
            {
                _logger.LogError("Invalid hocvienId: {HocvienId}. Id: {Id}, Type: {Type}, ThanhToanId: {ThanhToanId}",
                    hocvienId, id, type, thanhToanId);
                return Unauthorized("Thông tin học viên không hợp lệ.");
            }

            var currentUserHocvienId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (hocvienId != currentUserHocvienId)
            {
                _logger.LogError("HocvienId mismatch. Provided: {HocvienId}, Current User: {CurrentUserHocvienId}, Id: {Id}, Type: {Type}, ThanhToanId: {ThanhToanId}",
                    hocvienId, currentUserHocvienId, id, type, thanhToanId);
                return Unauthorized("Không có quyền xác nhận thanh toán này.");
            }

            try
            {
                var thanhToan = await _context.ThanhToans.FindAsync(thanhToanId);
                if (thanhToan == null)
                {
                    _logger.LogError("ThanhToan record not found for ThanhToanId: {ThanhToanId}. Id: {Id}, Type: {Type}, HocvienId: {HocvienId}",
                        thanhToanId, id, type, hocvienId);
                    return NotFound("Giao dịch không tồn tại.");
                }

                if (thanhToan.Trangthai != "Chưa thanh toán")
                {
                    _logger.LogError("ThanhToan record has invalid status. ThanhToanId: {ThanhToanId}, Trangthai: {Trangthai}, Id: {Id}, Type: {Type}, HocvienId: {HocvienId}",
                        thanhToanId, thanhToan.Trangthai, id, type, hocvienId);
                    return BadRequest("Giao dịch đã được xử lý.");
                }

                _logger.LogInformation("Processing payment confirmation. ThanhToanId: {ThanhToanId}, Id: {Id}, Type: {Type}, HocvienId: {HocvienId}",
                    thanhToanId, id, type, hocvienId);

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Update ThanhToan status
                    thanhToan.Trangthai = "Đã thanh toán";
                    _context.ThanhToans.Update(thanhToan);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("ThanhToan status updated to 'Đã thanh toán'. ThanhToanId: {ThanhToanId}", thanhToanId);

                    if (type == "Thi")
                    {
                        var registration = await _context.CtDangKyThis
                            .FirstOrDefaultAsync(ct => ct.CtDktId == id && ct.Hoso.HocvienId == parsedHocvienId);
                        if (registration == null)
                        {
                            _logger.LogError("No registration found for CtDktId: {Id}, HocvienId: {HocvienId}", id, parsedHocvienId);
                            throw new Exception("Không tìm thấy đăng ký thi.");
                        }

                        registration.thanhtoan = true;
                        _context.CtDangKyThis.Update(registration);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("CtDangKyThi updated. CtDktId: {Id}, thanhtoan: {thanhtoan}", id, registration.thanhtoan);
                    }
                    else if (type == "KhoaHoc")
                    {
                        var ketQua = await _context.KetQuaHocTaps
                            .FirstOrDefaultAsync(kq => kq.KetquaId == id);

                        if (ketQua == null)
                        {
                            _logger.LogError("Không tìm thấy kết quả học tập với KetquaId: {Id}", id);
                            throw new Exception("Không tìm thấy kết quả học tập.");
                        }

                        // Ghi log để đảm bảo đúng dữ liệu
                        _logger.LogInformation("Cập nhật nhận xét cho KetquaId: {Id}, HosoId: {HosoId}", ketQua.KetquaId, ketQua.HosoId);

                        // Cập nhật nhận xét
                        ketQua.Nhanxet = "Đã thanh toán khóa học";
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        _logger.LogError("Invalid Type value: {Type}. ThanhToanId: {ThanhToanId}, Id: {Id}, HocvienId: {HocvienId}",
                            type, thanhToanId, id, hocvienId);
                        throw new Exception("Loại thanh toán không hợp lệ.");
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("Payment confirmation completed. ThanhToanId: {ThanhToanId}, Id: {Id}, Type: {Type}, HocvienId: {HocvienId}",
                        thanhToanId, id, type, hocvienId);

                    // Redirect to a success page or UserKhoaHoc/UserThi
                    return RedirectToAction(type == "Thi" ? "UserThi" : "UserKhoaHoc");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during payment confirmation transaction. ThanhToanId: {ThanhToanId}, Id: {Id}, Type: {Type}, HocvienId: {HocvienId}",
                        thanhToanId, id, type, hocvienId);
                    throw;
                }
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error in ProcessPaymentCallback for ThanhToanId: {ThanhToanId}, Id: {Id}, Type: {Type}, HocvienId: {HocvienId}",
                    thanhToanId, id, type, hocvienId);
                return StatusCode(500, $"Lỗi cơ sở dữ liệu: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProcessPaymentCallback for ThanhToanId: {ThanhToanId}, Id: {Id}, Type: {Type}, HocvienId: {HocvienId}",
                    thanhToanId, id, type, hocvienId);
                return StatusCode(500, $"Đã xảy ra lỗi không mong đợi: {ex.Message}");
            }
        }

        // GET: Payment/Success
        [Authorize]
        public IActionResult Success(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        //// GET: Payment/UserKhoaHoc
        //[Authorize(Roles = "3")]
        //public async Task<IActionResult> UserKhoaHoc()
        //{
        //    var hocvienIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (!int.TryParse(hocvienIdClaim, out var hocvienId))
        //    {
        //        _logger.LogError("Không tìm thấy HocvienId trong claims.");
        //        return NotFound("Không tìm thấy thông tin học viên.");
        //    }

        //    _logger.LogInformation($"HocvienId: {hocvienId}");

        //    var results = await _context.KetQuaHocTaps
        //        .Include(kq => kq.Hoso)
        //            .ThenInclude(hs => hs.Hocvien)
        //        .Include(kq => kq.Lop)
        //            .ThenInclude(lh => lh != null ? lh.Khoahoc : null)
        //        .Include(kq => kq.Hoso)
        //            .ThenInclude(hs => hs.Hang)
        //        .Where(kq => kq.Hoso != null && kq.Hoso.HocvienId == hocvienId)
        //        .Select(kq => new KhoaHocViewModel
        //        {
        //            KetQuaHocTapId = kq.KetquaId,
        //            TenKhoaHoc = kq.Lop != null && kq.Lop.Khoahoc != null ? kq.Lop.Khoahoc.Tenkhoahoc : "Không có khóa học",
        //            TenLopHoc = kq.Lop != null ? kq.Lop.Tenlop : "Không có lớp",
        //            HangGPLX = kq.Hoso != null && kq.Hoso.Hang != null ? kq.Hoso.Hang.Tenhang : "Không có hạng",
        //            NgayHoc = kq.Lop != null && kq.Lop.Khoahoc != null ? kq.Lop.Khoahoc.Ngaybatdau.ToDateTime(TimeOnly.MinValue) : default,
        //            NhanXet = kq.Nhanxet ?? "Chưa có nhận xét",
        //            PhiDaoTao = kq.Hoso != null && kq.Hoso.Hang != null ? kq.Hoso.Hang.PhiDaotao ?? 0 : 0,
        //            HoTenThiSinh = kq.Hoso != null && kq.Hoso.Hocvien != null ? kq.Hoso.Hocvien.Tenhocvien : "Không có học viên"
        //        })
        //        .ToListAsync();

        //    _logger.LogInformation($"Số bản ghi KetQuaHocTap tìm thấy cho HocvienId {hocvienId}: {results.Count}");
        //    if (!results.Any(r => r.NhanXet == "Chưa thanh toán khóa học"))
        //    {
        //        _logger.LogWarning($"Không tìm thấy khóa học chưa thanh toán cho HocvienId {hocvienId}.");
        //    }

        //    var viewModel = new KhoaHocPaymentViewModel
        //    {
        //        UnpaidKhoaHocs = results.Where(r => r.NhanXet?.Trim() == "Chưa thanh toán khóa học").ToList(),
        //        PaidKhoaHocs = results.Where(r => r.NhanXet?.Trim() != "Chưa thanh toán khóa học").ToList()
        //    };

        //    return View(viewModel);
        //}

        [Authorize(Roles = "3")] // học viên
        public async Task<IActionResult> UserKhoaHoc()
        {
            var hocvienIdStr = User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .LastOrDefault()?.Value;

            if (string.IsNullOrEmpty(hocvienIdStr) || !int.TryParse(hocvienIdStr, out int hocvienId))
            {
                return Unauthorized();
            }

            var results = await _context.KetQuaHocTaps
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(kq => kq.Lop)
                    .ThenInclude(lh => lh.Khoahoc)
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hang)
                .Where(kq => kq.Hoso.HocvienId == hocvienId)
                .Select(kq => new KhoaHocViewModel
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

            var viewModel = new KhoaHocPaymentViewModel
            {
                UnpaidKhoaHocs = results.Where(r => r.NhanXet == "Chưa thanh toán khóa học").ToList(),
                PaidKhoaHocs = results.Where(r => r.NhanXet != "Chưa thanh toán khóa học").ToList()
            };

            return View("UserKhoaHoc", viewModel); // dùng chung View nếu cần
        }

        // GET: Payment/KhoaHoc (Admin view only)
        [Authorize(Roles = "1")]
        public async Task<IActionResult> KhoaHoc()
        {
            var results = await _context.KetQuaHocTaps
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hocvien)
                .Include(kq => kq.Lop)
                    .ThenInclude(lh => lh != null ? lh.Khoahoc : null)
                .Include(kq => kq.Hoso)
                    .ThenInclude(hs => hs.Hang)
                .Select(kq => new KhoaHocViewModel
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

            var viewModel = new KhoaHocPaymentViewModel
            {
                UnpaidKhoaHocs = results.Where(r => r.NhanXet == "Chưa thanh toán khóa học").ToList(),
                PaidKhoaHocs = results.Where(r => r.NhanXet != "Chưa thanh toán khóa học").ToList()
            };

            return View(viewModel);
        }
    }
}