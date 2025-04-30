using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class LoaiThi
{
    public int LoaithiId { get; set; }

    public string Tenloaithi { get; set; } = null!;

    public virtual ICollection<KyThi> KyThis { get; set; } = new List<KyThi>();
}
