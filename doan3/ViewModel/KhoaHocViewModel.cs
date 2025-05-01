namespace doan3.ViewModel
{
    public class KhoaHocViewModel
    {
        public int KhoahocId { get; set; }
        public int HangId { get; set; }
        public string Tenkhoahoc { get; set; }
        public DateTime Ngaybatdau { get; set; }
        public DateTime Ngayketthuc { get; set; }
        public int SlToida { get; set; }
        public string Trangthai { get; set; }
        public string Mota { get; set; }
        public int SoLuongConLai { get; set; } // Add this property
    }
}
