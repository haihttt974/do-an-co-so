using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class ThanhToan
{
    public int ThanhtoanId { get; set; }

    public string? TenThanhToan { get; set; }

    public decimal Sotien { get; set; }

    public string Trangthai { get; set; } = null!;

    public string? Ghichu { get; set; }

    public string Phuongthuc { get; set; } = null!;

    public virtual ICollection<CtPhieuThanhToan> CtPhieuThanhToans { get; set; } = new List<CtPhieuThanhToan>();
}
