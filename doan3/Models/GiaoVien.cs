using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace doan3.Models
{
    public partial class GiaoVien
    {
        public int GiaovienId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên giáo viên.")]
        public string Tengiaovien { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập chuyên môn của giáo viên.")]
        public string? Chuyenmon { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập hạng đào tạo của giáo viên.")]
        public string? HangDaotao { get; set; }

        public DateOnly? Ngaybatdaulamviec { get; set; }

        public string? ImgGv { get; set; }

        public virtual ICollection<LichTapLai> LichTapLais { get; set; } = new List<LichTapLai>();

        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    }
}