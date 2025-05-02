using System.ComponentModel.DataAnnotations;

namespace doan3.ViewModel
{
    public class CreateHoSoViewModel
    {
        public int HocvienId { get; set; }  // Sẽ gán tự động

        [Required(ErrorMessage = "Loại hồ sơ là bắt buộc")]
        public string LoaiHoso { get; set; }

        [Required(ErrorMessage = "Chọn hạng GPLX")]
        public int HangId { get; set; }

        [Required(ErrorMessage = "Ảnh thí sinh bắt buộc")]
        public IFormFile ImgThisinhFile { get; set; }

        [Required(ErrorMessage = "Giấy khám sức khỏe bắt buộc")]
        public IFormFile KhamsuckhoeFile { get; set; }
    }
}
