using System.ComponentModel.DataAnnotations;

namespace doan3.ViewModel
{
    public class RegisterViewModel
    {
        // Thông tin học viên
        [Required]
        public string TenHocVien { get; set; }

        [Required]
        public string SoCCCD { get; set; }

        [Required]
        public string GioiTinh { get; set; }


        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        // Thông tin tài khoản
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Email { get; set; }

        public string DiaChi { get; set; }

        public string SDT { get; set; }
    }

}
