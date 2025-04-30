using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class CtPhieuThanhToan
{
    public int CtPhieuttId { get; set; }

    public int HosoId { get; set; }

    public int ThanhtoanId { get; set; }

    public DateTime Thoigianthanhtoan { get; set; }

    public string? Loaiphi { get; set; }

    public virtual HoSoThiSinh Hoso { get; set; } = null!;

    public virtual ThanhToan Thanhtoan { get; set; } = null!;
}
