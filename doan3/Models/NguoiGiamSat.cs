using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class NguoiGiamSat
{
    public int NguoigsId { get; set; }

    public string Hoten { get; set; } = null!;

    public string? Donvi { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? NhomNguoigs { get; set; }

    public virtual ICollection<PhanCongGiamSat> PhanCongGiamSats { get; set; } = new List<PhanCongGiamSat>();
}
