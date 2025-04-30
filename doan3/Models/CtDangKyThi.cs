using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class CtDangKyThi
{
    public int CtDktId { get; set; }

    public int KythiId { get; set; }

    public int HosoId { get; set; }

    public DateTime Thoigiandk { get; set; }

    public DateTime? Thoigianthi { get; set; }

    public int? LichthiId { get; set; }

    public virtual HoSoThiSinh Hoso { get; set; } = null!;

    public virtual KyThi Kythi { get; set; } = null!;

    public virtual LichThi? Lichthi { get; set; }
}
