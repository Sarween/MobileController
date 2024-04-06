namespace MobileController.DTO
{
    public class JBSBWorksheetShift
    {

        public DateOnly? shiftdates { get; set; }
        public DateTime shiftCheckInTime { get; set; }
        public DateTime shiftCheckOutTime { get; set; }
        public int shiftDuration { get; set; }
        public double shiftsWage { get; set; }
        public string jobLocation { get; set; }
        public string jobPIC { get; set; }
        public bool jobVerification { get; set; }
        public bool isLate {  get; set; }
        public bool isOvertime { get; set; }
        public string staffReview { get; set; }
        public int rating { get; set; }
        public string studentComment { get; set; }
        public JBSBWorksheetShift() { }

        public JBSBWorksheetShift(DateOnly? shiftdates, DateTime shiftCheckInTime, DateTime shiftCheckOutTime, int shiftDuration, double shiftsWage, string jobLocation, string jobPIC, bool jobVerification, bool isLate, bool isOvertime, string staffReview, int rating, string studentComment)
        {
            this.shiftdates = shiftdates;
            this.shiftCheckInTime = shiftCheckInTime;
            this.shiftCheckOutTime = shiftCheckOutTime;
            this.shiftDuration = shiftDuration;
            this.shiftsWage = shiftsWage;
            this.jobLocation = jobLocation;
            this.jobPIC = jobPIC;
            this.jobVerification = jobVerification;
            this.isLate = isLate;
            this.isOvertime = isOvertime;
            this.staffReview = staffReview;
            this.rating = rating;
            this.studentComment = studentComment;
        }
    }
}
