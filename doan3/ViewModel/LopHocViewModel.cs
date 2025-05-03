namespace doan3.ViewModel
{
    public class LopHocViewModel
    {
        public int LopId { get; set; }
        public string Tenlop { get; set; }
        public string LoaiLop { get; set; }
        public List<LichHocViewModel> LichHocs { get; set; } // Danh sách lịch học của lớp
    }
}
