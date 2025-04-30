using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class LopHoc
{
    public int LopId { get; set; }

    public string Tenlop { get; set; } = null!;

    public string? LoaiLop { get; set; }

    public int? KhoahocId { get; set; }

    public int? GiaovienId { get; set; }

    public virtual GiaoVien? Giaovien { get; set; }

    public virtual ICollection<KetQuaHocTap> KetQuaHocTaps { get; set; } = new List<KetQuaHocTap>();

    public virtual KhoaHoc? Khoahoc { get; set; }

    public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();
}
