using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doan3.Models;

public partial class KhoaHoc
{
    public int KhoahocId { get; set; }

    public int? HangId { get; set; }

    public string Tenkhoahoc { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu.")]
    [CustomValidation(typeof(KhoaHocValidator), nameof(KhoaHocValidator.ValidateNgayBatDau))]
    public DateOnly Ngaybatdau { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc.")]
    [CustomValidation(typeof(KhoaHocValidator), nameof(KhoaHocValidator.ValidateNgayKetThuc))]
    public DateOnly Ngayketthuc { get; set; }

    public int SlToida { get; set; }

    public string? Trangthai { get; set; }

    public string? Mota { get; set; }

    public virtual HangGplx? Hang { get; set; }

    [NotMapped]
    public int SoLuongConLai { get; set; }

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
}
