namespace doan3.ViewModels
{
    public class ThiViewModel
    {
        public int ChiTietDangKyThiId { get; set; }
        public string HoTenThiSinh { get; set; }
        public string TenKyThi { get; set; }
        public string HangGPLX { get; set; }
        public DateTime NgayHoc { get; set; }
        public bool ThanhToan { get; set; }
        public decimal PhiThi { get; set; }
    }

    public class ThiPaymentViewModel
    {
        public List<ThiViewModel> UnpaidRegistrations { get; set; }
        public List<ThiViewModel> PaidRegistrations { get; set; }
    }
}