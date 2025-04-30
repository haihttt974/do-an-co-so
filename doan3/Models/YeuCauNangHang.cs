using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class YeuCauNangHang
{
    public int YeucauId { get; set; }

    public int HangId { get; set; }

    public string Yeucau { get; set; } = null!;

    public int SonamKn { get; set; }

    public virtual HangGplx Hang { get; set; } = null!;
}
