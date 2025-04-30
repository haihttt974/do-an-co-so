using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class KetQuaThi
{
    public int KetquaId { get; set; }

    public int BaithiId { get; set; }

    public string Ketqua { get; set; } = null!;

    public string? Ghichu { get; set; }

    public decimal? Diem { get; set; }

    public virtual BaiThi Baithi { get; set; } = null!;
}
