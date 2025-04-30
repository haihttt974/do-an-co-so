using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class BaiThi
{
    public int BaithiId { get; set; }

    public int KythiId { get; set; }

    public int HosoId { get; set; }

    public int LoaibaiId { get; set; }

    public string? Tenbaithi { get; set; }

    public int Thutu { get; set; }

    public int? Thoigian { get; set; }

    public virtual HoSoThiSinh Hoso { get; set; } = null!;

    public virtual KetQuaThi? KetQuaThi { get; set; }

    public virtual KyThi Kythi { get; set; } = null!;

    public virtual PhanLoaiBaiThi Loaibai { get; set; } = null!;

    public virtual ICollection<PhanCongGiamSat> PhanCongGiamSats { get; set; } = new List<PhanCongGiamSat>();
}
