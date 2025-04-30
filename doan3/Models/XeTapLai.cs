using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class XeTapLai
{
    public int XeId { get; set; }

    public string Loaixe { get; set; } = null!;

    public string Trangthai { get; set; } = null!;

    public virtual ICollection<LichTapLai> LichTapLais { get; set; } = new List<LichTapLai>();
}
