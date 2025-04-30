using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class PhanLoaiBaiThi
{
    public int LoaibaiId { get; set; }

    public string Tenloaibai { get; set; } = null!;

    public string? Mota { get; set; }

    public virtual ICollection<BaiThi> BaiThis { get; set; } = new List<BaiThi>();
}
