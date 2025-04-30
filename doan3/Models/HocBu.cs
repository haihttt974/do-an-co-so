using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class HocBu
{
    public int HocbuId { get; set; }

    public DateTime Ngayhoc { get; set; }

    public decimal Lephi { get; set; }

    public int HosoId { get; set; }

    public int LichhocId { get; set; }

    public virtual HoSoThiSinh Hoso { get; set; } = null!;

    public virtual LichHoc Lichhoc { get; set; } = null!;
}
