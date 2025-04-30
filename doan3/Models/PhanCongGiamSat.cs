using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class PhanCongGiamSat
{
    public int PhancongGsId { get; set; }

    public int NguoigsId { get; set; }

    public int BaithiId { get; set; }

    public string Vaitro { get; set; } = null!;

    public string? Ghichu { get; set; }

    public virtual BaiThi Baithi { get; set; } = null!;

    public virtual NguoiGiamSat Nguoigs { get; set; } = null!;
}
