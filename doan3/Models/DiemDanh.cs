using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class DiemDanh
{
    public int DiemdanhId { get; set; }

    public DateTime Ngayhoc { get; set; }

    public string Trangthai { get; set; } = null!;

    public int HosoId { get; set; }

    public int LichhocId { get; set; }

    public virtual HoSoThiSinh Hoso { get; set; } = null!;

    public virtual LichHoc Lichhoc { get; set; } = null!;
}
