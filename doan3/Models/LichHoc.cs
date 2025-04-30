using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class LichHoc
{
    public int LichhocId { get; set; }

    public int? LopId { get; set; }

    public DateTime TgBatdau { get; set; }

    public DateTime TgKetthuc { get; set; }

    public string Hinhthuchoc { get; set; } = null!;

    public string? Noidung { get; set; }

    public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    public virtual ICollection<HocBu> HocBus { get; set; } = new List<HocBu>();

    public virtual LopHoc? Lop { get; set; }
}
