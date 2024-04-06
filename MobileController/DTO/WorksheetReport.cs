namespace MobileController.DTO
{
    public class WorksheetReport
    {
        public List<JBSBWorksheetShift> Worksheet { get; set; }
        public int totalDuration { get; set; }
        public double totalPay { get; set; }

        public WorksheetReport()
        {

        }

        public WorksheetReport(List<JBSBWorksheetShift> worksheet, int totalDuration, double totalPay)
        {
            Worksheet = worksheet;
            this.totalDuration = totalDuration;
            this.totalPay = totalPay;
        }
    }


}
