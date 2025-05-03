using System.Collections.Generic;

namespace doan3.ViewModel
{
    public class KhoaHocLichHocViewModel
    {
        public int KhoaHocId { get; set; }
        public string TenKhoaHoc { get; set; }
        public int LopId { get; set; }
        public string TenLop { get; set; }
        public string LoaiLop { get; set; }
        public List<LichHocViewModel> LichHocs { get; set; }
    }
}