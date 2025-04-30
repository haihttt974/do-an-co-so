using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class KhoaHoc
{
    public int KhoahocId { get; set; }

    public int? HangId { get; set; }

    public string Tenkhoahoc { get; set; } = null!;

    public DateOnly Ngaybatdau { get; set; }

    public DateOnly Ngayketthuc { get; set; }

    public int SlToida { get; set; }

    public string? Trangthai { get; set; }

    public string? Mota { get; set; }

    public virtual HangGplx? Hang { get; set; }

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
}
