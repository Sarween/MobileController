namespace MobileController.Models
{
    public class Student
    {

        public Student() { }

        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentPassword { get; set; }
        public string Email { get; set; }
        public string ParentEmail { get; set; }
        //public int ParentID { get; set; }
        //public Parent Parent { get; set; }
    }
}
