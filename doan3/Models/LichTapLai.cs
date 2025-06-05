using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class LichTapLai
{
    public int LichId { get; set; }

    public int GiaovienId { get; set; }

    public int HosoId { get; set; }

    public int XeId { get; set; }

    public DateTime Tgbatdau { get; set; }

    public DateTime Tgketthuc { get; set; }

    public string? Diadiem { get; set; }

    public string? Noidung { get; set; }

    public string? Ghichu { get; set; }

    public virtual GiaoVien? Giaovien { get; set; } = null!;

    public virtual HoSoThiSinh? Hoso { get; set; } = null!;

    public virtual XeTapLai? Xe { get; set; } = null!;
}
