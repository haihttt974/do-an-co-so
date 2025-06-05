using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class KetQuaHocTap
{
    public int KetquaId { get; set; }

    public int? LopId { get; set; }

    public int HosoId { get; set; }

    public string? Nhanxet { get; set; }

    public int? Sobuoiodahoc { get; set; }

    public int? Sobuoitoithieu { get; set; }

    public decimal? KmHoanthanh { get; set; }

    public int? GioBandem { get; set; }

    public string? HtLythuyet { get; set; }

    public string? HtMophong { get; set; }

    public string? HtSahinh { get; set; }

    public string? HtDuongtruong { get; set; }

    public string? DauTn { get; set; }

    public string? DuDkThisat { get; set; }

    public DateTime? Thoigiancapnhat { get; set; }

    public virtual HoSoThiSinh Hoso { get; set; } = null!;

    public virtual LopHoc? Lop { get; set; }
}
