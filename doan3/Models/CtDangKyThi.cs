using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class CtDangKyThi
{
    public int CtDktId { get; set; }

    public int KythiId { get; set; }

    public int HosoId { get; set; }

    public DateTime Thoigiandk { get; set; }
    public bool thanhtoan {  get; set; }
    public DateTime? Thoigianthi { get; set; }

    public int? LichthiId { get; set; }
    //  ALTER TABLE CT_DANG_KY_THI ADD THANHTOAN BIT NOT NULL DEFAULT 0;
    public virtual HoSoThiSinh Hoso { get; set; } = null!;

    public virtual KyThi Kythi { get; set; } = null!;

    public virtual LichThi? Lichthi { get; set; }
}
