namespace doan3.ViewModels
{
    public class ConfirmPaymentViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Thi" hoặc "KhoaHoc"
        public int HosoId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string PaymentMethod { get; set; } // "Momo", "Ngân hàng", "VNPay", "ZaloPay"
    }

    public class QRCodeViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int ThanhToanId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string QRCodeUrl { get; set; }
    }

}