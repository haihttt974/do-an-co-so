using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class HoSoThiSinh
{
    public int HosoId { get; set; }
    public int HocvienId { get; set; }
    public string? ImgThisinh { get; set; }
    public string? LoaiHoso { get; set; }
    public int HangId { get; set; }
    public DateOnly Ngaydk { get; set; }
    public string? Khamsuckhoe { get; set; }
    public string? Ghichu { get; set; }

    public virtual ICollection<BaiThi> BaiThis { get; set; } = new List<BaiThi>();
    public virtual ICollection<CtDangKyThi> CtDangKyThis { get; set; } = new List<CtDangKyThi>();
    public virtual ICollection<CtPhieuThanhToan> CtPhieuThanhToans { get; set; } = new List<CtPhieuThanhToan>();
    public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();
    public virtual GiayPhepLaiXe? GiayPhepLaiXe { get; set; }
    public virtual HangGplx Hang { get; set; } = null!;
    public virtual ICollection<HocBu> HocBus { get; set; } = new List<HocBu>();
    public virtual HocVien Hocvien { get; set; } = null!;
    public virtual ICollection<KetQuaHocTap> KetQuaHocTaps { get; set; } = new List<KetQuaHocTap>(); // Sửa thành danh sách
    public virtual ICollection<LichTapLai> LichTapLais { get; set; } = new List<LichTapLai>();
}