using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class HocVien
{
    public int HocvienId { get; set; }

    public string Tenhocvien { get; set; } = null!;

    public string Socccd { get; set; } = null!;

    public string Gioitinh { get; set; } = null!;

    public DateTime? Ngaysinh { get; set; }

    public virtual ICollection<HoSoThiSinh> HoSoThiSinhs { get; set; } = new List<HoSoThiSinh>();

    public virtual ICollection<Phanhoi> Phanhois { get; set; } = new List<Phanhoi>();
}
