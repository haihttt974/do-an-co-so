using doan3.Models;
using System;
using System.Collections.Generic;

namespace doan3.ViewModel
{
    public class RegisterKHViewModel
    {
        public int KhoaHocId { get; set; }
        public KhoaHoc KhoaHoc { get; set; } // KhoaHoc includes SoLuongConLai
        public List<LopHocViewModel> LopHocs { get; set; }
        public int LopId { get; set; }
        public string LoaiHoSo { get; set; }
        public string KhamSucKhoe { get; set; }
        public string GhiChu { get; set; }
        public int LichHocId { get; set; }
    }
    public class LichHocViewModel
    {
        public int LichHocId { get; set; }
        public DateTime TgBatDau { get; set; }
        public DateTime TgKetThuc { get; set; }
        public string HinhThucHoc { get; set; }
        public string NoiDung { get; set; }
    }
}
