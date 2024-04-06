using MobileController.Models;

namespace MobileController.DTO
{
    public class StudentPerformanceItem
    {
        public Student student { get; set; }
        public Dictionary<string, int> DepartmentDurations { get; set; }
        public double aveRating { get; set; }
        public StudentPerformanceItem() {}

        public StudentPerformanceItem(Student student, Dictionary<string, int> departmentDurations, double aveRating)
        {
            this.student = student;
            DepartmentDurations = departmentDurations;
            this.aveRating = aveRating;
        }

    }
}
