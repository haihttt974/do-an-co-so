using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class QuyDinhHangGplx
{
    public int QuydinhId { get; set; }

    public int HangId { get; set; }

    public int? KmToithieu { get; set; }

    public int? SogioBandem { get; set; }

    public bool? LyThuyet { get; set; }

    public bool? MoPhong { get; set; }

    public bool? SaHinh { get; set; }

    public bool? DuongTruong { get; set; }

    public string? Ghichu { get; set; }

    public virtual HangGplx Hang { get; set; } = null!;
}
