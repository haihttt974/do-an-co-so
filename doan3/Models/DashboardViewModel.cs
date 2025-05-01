namespace doan3.Models
{
    public class DashboardViewModel
    {
        // Existing properties
        public int CoursesCount { get; set; }
        public int DocumentsCount { get; set; }
        public int ExamsCount { get; set; }
        public int ExamSchedulesCount { get; set; }
        public int SupervisorAssignmentsCount { get; set; }
        public int DrivingSchedulesCount { get; set; }
        public int FeedbackCount { get; set; }
        public int TeachersCount { get; set; }
        public int StudentsCount { get; set; }
        public int CandidateProfilesCount { get; set; }
        public int ClassCount { get; set; }

        // New properties for System Activity
        public int TodayExamSchedulesCount { get; set; }
        public int TodayPaymentsCount { get; set; }
        public decimal TodayRevenue { get; set; }

        // New properties for Recent Payments and Top Courses
        public List<Payment> RecentPayments { get; set; }
        //public List<Course> TopCourses { get; set; }
    }

    public class Payment
    {
        public int CT_PHIEUTT_ID { get; set; }
        public int HOSO_ID { get; set; }
        public string LOAIPHI { get; set; }
        public DateTime? THOIGIANTHANHTOAN { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int EnrolledStudents { get; set; }
        public bool IsActive { get; set; }
    }
}