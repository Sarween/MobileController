namespace MobileController.Models
{
    public class JBSBWorksheet
    {

        public DateTime shiftdates { get; set; }
        public DateTime shiftCheckInTime { get; set; }
        public DateTime shiftCheckOutTime { get; set; }
        public int shiftDuration { get; set; }
        public double shiftsWage { get; set; }
        public string jobLocation { get; set; }
        public int jobPIC { get; set; }
        public bool jobVerification { get; set; }

        public JBSBWorksheet() { }

        public JBSBWorksheet(DateTime shiftdates, DateTime shiftCheckInTime, DateTime shiftCheckOutTime, int shiftDuration, double shiftsWage, string jobLocation, int jobPIC, bool jobVerification)
        {
            this.shiftdates = shiftdates;
            this.shiftCheckInTime = shiftCheckInTime;
            this.shiftCheckOutTime = shiftCheckOutTime;
            this.shiftDuration = shiftDuration;
            this.shiftsWage = shiftsWage;
            this.jobLocation = jobLocation;
            this.jobPIC = jobPIC;
            this.jobVerification = jobVerification;
        }
    }
}
