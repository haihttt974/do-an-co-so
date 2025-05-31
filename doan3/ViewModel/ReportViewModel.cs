using System.Collections.Generic;

namespace doan3.Models
{
    public class ReportViewModel
    {
        public List<LopHocStat> LopHocStats { get; set; }
    }

    public class LopHocStat
    {
        public int LopId { get; set; }
        public string TenLop { get; set; }
        public int SoHocVien { get; set; }
        public double TiLeDiemDanh { get; set; }
    }

    public class LopHocStatsNewViewModel
    {
        public List<LopHocStatNew> LopHocStatsNew { get; set; }
    }

    public class LopHocStatNew
    {
        public int LopId { get; set; }
        public string TenLop { get; set; }
        public int SoBuoiHoc { get; set; }
        public int SoHocVienDat { get; set; }
    }

    public class GiayPhepViewModel
    {
        public List<GiayPhepStat> GiayPhepStats { get; set; }
    }

    public class GiayPhepStat
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public string Hang { get; set; }
        public int SoLuong { get; set; }
    }

    public class HieuQuaDaoTaoViewModel
    {
        public List<HieuQuaStat> HieuQuaStats { get; set; }
    }

    public class HieuQuaStat
    {
        public int LopId { get; set; }
        public string TenLop { get; set; }
        public int TongHocVien { get; set; }
        public double TiLeDat { get; set; }
    }

    public class DiemDanhGiaoVienViewModel
    {
        public List<DiemDanhStat> DiemDanhStats { get; set; }
    }

    public class DiemDanhStat
    {
        public int GiaoVienId { get; set; }
        public string TenGiaoVien { get; set; }
        public double TiLeDiemDanh { get; set; }
    }
}