namespace doan3.Models
{
    public class HocVienLop
    {
        public int HocVienLopId { get; set; }
        public int UserId { get; set; }
        public int LopId { get; set; }

        public virtual User User { get; set; }
        public virtual LopHoc Lop { get; set; }
    }
}
