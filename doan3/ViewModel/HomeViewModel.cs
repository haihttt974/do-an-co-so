using doan3.Models;

namespace doan3.ViewModel
{
    public class HomeViewModel
    {
        public List<GiaoVien> Instructors { get; set; } = new List<GiaoVien>();
        public List<KhoaHoc> Courses { get; set; } = new List<KhoaHoc>();
        public List<Phanhoi> Testimonials { get; set; } = new List<Phanhoi>();
    }
}
