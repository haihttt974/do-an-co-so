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
        public List<LichThi> AvailableExams { get; set; } = new List<LichThi>();
        [Required(ErrorMessage = "Vui lòng chọn lịch thi")]
        public int SelectedLichThiId { get; set; }
        public string RegistrationMessage { get; set; }
        public CtDangKyThi RegisteredExam { get; set; }
    }
}