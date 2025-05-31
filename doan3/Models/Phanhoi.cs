using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace doan3.Models;

public partial class Phanhoi
{
    public int PhanhoiId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung phản hồi.")]
    public string? Noidung { get; set; }

    public DateTime Thoigianph { get; set; }

    public int HocvienId { get; set; }

    public virtual HocVien Hocvien { get; set; } = null!;
}
