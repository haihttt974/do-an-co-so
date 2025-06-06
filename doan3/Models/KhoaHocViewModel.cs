namespace doan3.Models
{
    public class KhoaHocViewModel
    {
        public int KetQuaHocTapId { get; set; }
        public string HoTenThiSinh { get; set; }
        public string TenKhoaHoc { get; set; }
        public string TenLopHoc { get; set; }
        public string HangGPLX { get; set; }
        public DateTime NgayHoc { get; set; }
        public string NhanXet { get; set; }
        public decimal PhiDaoTao { get; set; }
    }

    public class KhoaHocPaymentViewModel
    {
        public List<KhoaHocViewModel> UnpaidKhoaHocs { get; set; }
        public List<KhoaHocViewModel> PaidKhoaHocs { get; set; }
    }
}