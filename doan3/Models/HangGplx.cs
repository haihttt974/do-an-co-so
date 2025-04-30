using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class HangGplx
{
    public int HangId { get; set; }

    public string Tenhang { get; set; } = null!;

    public string? Mota { get; set; }

    public string? Loaiphuongtien { get; set; }

    public int Tuoitoithieu { get; set; }

    public int? Tuoitoida { get; set; }

    public string? Suckhoe { get; set; }

    public string? Hocvan { get; set; }

    public string? TgDtLythuyet { get; set; }

    public string? TgDtThuchanh { get; set; }

    public decimal? PhiDaotao { get; set; }

    public decimal? PhiThi { get; set; }

    public decimal? PhiCapphep { get; set; }

    public bool? OnlySatHach { get; set; }

    public virtual ICollection<GiayPhepLaiXe> GiayPhepLaiXes { get; set; } = new List<GiayPhepLaiXe>();

    public virtual ICollection<HoSoThiSinh> HoSoThiSinhs { get; set; } = new List<HoSoThiSinh>();

    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();

    public virtual ICollection<KyThi> KyThis { get; set; } = new List<KyThi>();

    public virtual ICollection<QuyDinhHangGplx> QuyDinhHangGplxes { get; set; } = new List<QuyDinhHangGplx>();

    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();

    public virtual ICollection<YeuCauNangHang> YeuCauNangHangs { get; set; } = new List<YeuCauNangHang>();
}
