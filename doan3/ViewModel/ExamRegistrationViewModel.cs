using doan3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace doan3.ViewModel
{
    public class ExamRegistrationViewModel
    {
        public int UserId { get; set; }
        public int HocVienId { get; set; }
        public string RegistrationMessage { get; set; }
        public CtDangKyThi RegisteredExam { get; set; }
        public List<LichThi> AvailableExams { get; set; } = new List<LichThi>();
        public int? SelectedLichThiId { get; set; } // Lịch thi được chọn

        // Thêm các thuộc tính để chọn kỳ thi
        public List<KyThi> AvailableKyThis { get; set; } = new List<KyThi>(); // Danh sách kỳ thi
        public int? SelectedKyThiId { get; set; } // Kỳ thi được chọn
    }
}