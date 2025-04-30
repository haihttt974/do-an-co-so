using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class GiaoVien
{
    public int GiaovienId { get; set; }

    public string Tengiaovien { get; set; } = null!;

    public string? Chuyenmon { get; set; }

    public string? HangDaotao { get; set; }

    public DateOnly? Ngaybatdaulamviec { get; set; }

    public string? ImgGv { get; set; }

    public virtual ICollection<LichTapLai> LichTapLais { get; set; } = new List<LichTapLai>();

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
}
