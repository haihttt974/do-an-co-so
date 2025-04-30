using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class KyThi
{
    public int KythiId { get; set; }

    public string Tenkythi { get; set; } = null!;

    public int HangId { get; set; }

    public int LoaithiId { get; set; }

    public virtual ICollection<BaiThi> BaiThis { get; set; } = new List<BaiThi>();

    public virtual ICollection<CtDangKyThi> CtDangKyThis { get; set; } = new List<CtDangKyThi>();

    public virtual HangGplx Hang { get; set; } = null!;

    public virtual LoaiThi Loaithi { get; set; } = null!;
}
