using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class GiayPhepLaiXe
{
    public int GplxId { get; set; }

    public int HosoId { get; set; }

    public int HangId { get; set; }

    public DateOnly Ngaycap { get; set; }

    public DateOnly Ngayhethan { get; set; }

    public string? Ghichu { get; set; }

    public virtual HangGplx Hang { get; set; } = null!;

    public virtual HoSoThiSinh Hoso { get; set; } = null!;
}
