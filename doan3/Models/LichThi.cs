using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class LichThi
{
    public int LichthiId { get; set; }

    public DateTime Thoigianthi { get; set; }

    public string Diadiemthi { get; set; } = null!;

    public string? Ghichu { get; set; }

    public virtual ICollection<CtDangKyThi> CtDangKyThis { get; set; } = new List<CtDangKyThi>();
}
