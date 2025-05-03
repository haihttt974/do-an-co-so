using System;

namespace doan3.ViewModel
{
    public class LichHocViewModel
    {
        public int LichHocId { get; set; }
        public DateTime TgBatDau { get; set; }
        public DateTime TgKetThuc { get; set; }
        public string HinhThucHoc { get; set; }
        public string NoiDung { get; set; }
        public bool IsCompleted { get; set; }
    }
}