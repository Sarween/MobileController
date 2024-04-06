using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileController.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;
using MobileController.Services;
using MobileController.Data;
using MobileController.DTO;
using System.Net.Mail;
using System.Net;
using System.Net.Mail;
using NuGet.Protocol.Core.Types;
using System.Globalization;
using System.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;
        private readonly IShiftService _shiftService;

        public ShiftController(MyDBContext myDBContext, IShiftService shiftService)
        {
            _myDBContext = myDBContext;
            _shiftService = shiftService;
        }

        // GET api/<ShiftController>/5
        [HttpGet]
        public async Task<IActionResult> GetShifts()
        {
            var shifts = await _myDBContext.Shift.ToListAsync();
            return Ok(shifts);
        }

        // Get all future shifts by student 
        [HttpGet("StudentShift")]
        public async Task<IActionResult> GetShifts([FromQuery] int studentId)
        {

            //var dateTime = DateTime.UtcNow.AddHours(8).Date;
            //var currentDate = DateOnly.FromDateTime(dateTime);

            //var oriDate = new DateTime(2000, 1, 1, 0, 0, 0, 0);

            //////var studentShifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId).ToListAsync();

            //////What if the student didnt check in

            //var studentFutureJobs = await _myDBContext.Shift.Join(
            //    _myDBContext.Recruitment,
            //    s => s.RecruitmentID,
            //    r => r.RecruitmentID,
            //    (s, r) => new
            //    {
            //        Shift = s,
            //        Recruitment = r
            //    })                                                     // Check if The student has not checked out
            //    .Where(sr => sr.Shift.StudentID == studentId && sr.Recruitment.JobShiftDate >= currentDate) //Today check in is still visible       //&& sr.Recruitment.JobShiftDate.Date >= currentDate
            //    .Select(sr => sr.Recruitment)
            //    .OrderBy(sr => sr.JobShiftDate)
            //    .ToListAsync();

            //var studentJob = (from s in _myDBContext.Shift join r in _myDBContext.Recruitment on s.RecruitmentID equals r.RecruitmentID select new ShiftJoinRecruitment { s.}).ToListAsync();

            // Get all the students shifts that haven't been checked out and display them in ascending order.
            //var studentShifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.IsCheckOut == false).OrderBy(s => s.StartDate).ToListAsync();
            //var studentJob = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.IsCheckOut == false).Include(s => s.Recruitment).OrderBy(s => s.Recruitment.StartTime).ToListAsync();

            //        var studentJob = await _myDBContext.Shift.Join(
            //_myDBContext.Recruitment,
            //s => s.RecruitmentID,
            //r => r.RecruitmentID,
            //(s, r) => new
            //{
            //    Shift = s,
            //    Recruitment = r
            //})
            //.Where(sr => sr.Shift.StudentID == studentId)
            //.OrderBy(sr => sr.Recruitment.JobShiftDate)
            //.ToListAsync();



            //return Ok(studentFutureJobs.Select(recruitment => recruitment.Recruitment).ToList());
            // Using service
            //var result = await _shiftService.GetFutureShiftByStuID(studentId);
            //return Ok(result);

            var dateTime = DateTime.UtcNow.AddHours(8).Date;
            var currentDate = DateOnly.FromDateTime(dateTime);

            DateTime notCheckOut = new DateTime(2000, 1, 1, 0, 0, 0);

            var eachStudentShiftsRecord = await _myDBContext.Shift.Join(
                            _myDBContext.Recruitment,
                            s => s.RecruitmentID,
                            r => r.RecruitmentID,
                            (s, r) => new
                            {
                                Shift = s,
                                Recruitment = r
                            })
                            .Where(sr => sr.Shift.StudentID == studentId && // Check if The student has not checked out
                            sr.Recruitment.JobShiftDate >= currentDate &&
                            sr.Shift.IsCancelled == false &&
                            sr.Shift.CheckOutTime == notCheckOut
                           ) //Today check in is still visible       //sr.Shift.CheckOutTime == checkOut
                            .Select(sr => sr.Recruitment)
                            .OrderBy(sr => sr.JobShiftDate)
                            .ToListAsync();
                            



            List<Recruitment> futureShifts = new List<Recruitment>();

            foreach (var shift in eachStudentShiftsRecord)
            {
                if (shift.JobShiftDate >= currentDate)
                {
                    futureShifts.Add(shift);
                }
            }

            if (futureShifts.Count > 0)
            {

                return Ok(futureShifts);
            }
            else
            {
                return Ok("No shifts left");
            }

            //return Ok(studentFutureJobs);
        }

        //Add public holiday.
        // Admin must register parent email.
        //Supervisor edit student check in
        // Supervisor must pass temporary password to student.

        private double getArea(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
        }

        // When student press check in
        [HttpPut("boundaryTest")]
        public async Task<IActionResult> testBoundary([FromQuery] double latitude, [FromQuery] double longitude)
        {

            double x1 = 2.98852; double y1 = 101.50244;
            double x2 = 2.98852; double y2 = 101.50948;
            double x3 = 2.98663; double y3 = 101.50365;

            double wholeTriangleArea = Math.Round(getArea(x1, y1, x2, y2, x3, y3), 7);
            double subTriangle1Area = Math.Round(getArea(latitude, longitude, x1, y1, x2, y2), 7);
            double subTriangle2Area = Math.Round(getArea(latitude, longitude, x2, y2, x3, y3), 7);
            double subTriangle3Area = Math.Round(getArea(latitude, longitude, x3, y3, x1, y1), 7);

            double sumSubTriangle = subTriangle1Area + subTriangle2Area + subTriangle3Area;

            System.Diagnostics.Debug.WriteLine(wholeTriangleArea.ToString("0.000000"));
            System.Diagnostics.Debug.WriteLine(sumSubTriangle.ToString("0.000000"));

            // 
            if (wholeTriangleArea == sumSubTriangle)
            {
                return Ok("Inside");
            }
            else
            {
                return Ok("Outside");
            }

        }

        // When student press check in
        [HttpPut("Approve/{recruitmentId}/{studentId}/{comment}/{rating}/{isOvertime}")]
        public async Task<IActionResult> Approve(int recruitmentId, int studentId, string comment, int rating, bool isOvertime, [FromQuery] string action, [FromQuery] int duration) //, string checkInTime, string checkOutTime, string comment, int rating
        {

            var shift = await _myDBContext.Shift.FindAsync(recruitmentId, studentId);

            var recruitment = await _myDBContext.Recruitment.FindAsync(recruitmentId);

            //shift.StaffReview = comment;
            shift.Rating = rating;
            shift.IsOvertime = isOvertime;
            shift.IsAuthorized = true;


            if (action.ToLower() == "add")
            {
                shift.CheckOutTime = shift.CheckOutTime.AddMinutes(duration);
                shift.StaffReview = "ADDED " + duration + " minutes | " + comment;

            }
            else if (action.ToLower() == "deduct")
            {
                shift.CheckInTime = shift.CheckInTime.AddMinutes(duration);
                shift.StaffReview = "DEDUCTED " + duration + " minutes | " + comment;
            }
            else if (action.ToLower() == "acknowledge")
            {
                TimeOnly timeOnly = recruitment.EndTime;
                var referenceDate = new DateTime(recruitment.JobShiftDate.Value.Year, recruitment.JobShiftDate.Value.Month, recruitment.JobShiftDate.Value.Day);
                referenceDate += timeOnly.ToTimeSpan();
                shift.CheckOutTime = referenceDate;
                shift.StaffReview = comment;
            }
            else if (action.ToLower() == "dismiss")
            {
                shift.IsCancelled = true;
            }
            else
            {
                shift.StaffReview = comment;
            }

            await _myDBContext.SaveChangesAsync();
            return Ok(shift);
        }

            // When student press check in
            [HttpPut("CheckIn")]
        public async Task<IActionResult> CheckInTimeAsync([FromQuery] int recruitmentId, [FromQuery] int studentId, [FromQuery] double latitude, [FromQuery] double longitude)
        {
            //var latitude = location.latitude;
            //var longitude = location.longitude;

            // Get the Shift object from the database.

            var shift = await _myDBContext.Shift.FindAsync(recruitmentId, studentId);

            var recruitment = await _myDBContext.Recruitment.FindAsync(recruitmentId);

            var currentTimeStamp = DateTime.UtcNow.AddHours(8);
            currentTimeStamp = currentTimeStamp.AddTicks(-(currentTimeStamp.Ticks % TimeSpan.TicksPerSecond));

            //var checkInTime = currentTimeStamp.TimeOfDay; 
            var checkInTime = TimeOnly.FromDateTime(currentTimeStamp); 

            var shiftEndTime = recruitment.EndTime; 
            var shiftStartTime = recruitment.StartTime;
            var shiftStartTimeplus10mins = shiftStartTime.Add(new TimeSpan(0, 10, 0));

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            // Check if today is the day to check in/////////////////////////////////////////////////////
            if (recruitment.JobShiftDate != today)
            {
                //You have already checked in
                return BadRequest("Shift is not today");
            }


            // Check if checkinTime has already been updated
            if (shift.CheckInTime != new DateTime(2000, 1, 1, 0, 0, 0, 0))
            {
                //You have already checked in
                return BadRequest("You have checked in already");
            }

            // Early check in

            // Student wants to check in but already passed EndTime // Only visible for that specific day
            if (checkInTime >= shiftEndTime)
            {
                return BadRequest("Too late to check in");
            }

            double x1 = 2.98852; double y1 = 101.50244;
            double x2 = 2.98852; double y2 = 101.50948;
            double x3 = 2.98663; double y3 = 101.50365;

            double wholeTriangleArea = Math.Round(getArea(x1, y1, x2, y2, x3, y3), 7);
            double subTriangle1Area = Math.Round(getArea(latitude, longitude, x1, y1, x2, y2), 7);
            double subTriangle2Area = Math.Round(getArea(latitude, longitude, x2, y2, x3, y3), 7);
            double subTriangle3Area = Math.Round(getArea(latitude, longitude, x3, y3, x1, y1), 7);

            double sumSubTriangle = subTriangle1Area + subTriangle2Area + subTriangle3Area;

            System.Diagnostics.Debug.WriteLine(wholeTriangleArea);
            System.Diagnostics.Debug.WriteLine(sumSubTriangle);

            // Check if inside triangle PKT Logistics area
            if (wholeTriangleArea == sumSubTriangle)
            {
                System.Diagnostics.Debug.WriteLine("Inside");
            }
            else
            {
                return BadRequest("You're not at company");
            }
            //// Check if the student is within the boundary.
            //if (latitude < 1 || latitude > 40 || longitude < 90 || longitude > 110)
            //{
            //    System.Diagnostics.Debug.WriteLine("You're not at company!");
            //    // 10 , 80
            //    // The student is not within the boundary.
            //    //return new JsonResult(new { CheckInStatus = CheckInStatus.OutsideBoundary, checkInTime });
            //    return BadRequest("You're not at company");
            //}

            //30, 100

            System.Diagnostics.Debug.WriteLine(shiftStartTime);


            // Either time is 8.30 -12.30 pm || 2 pm - 6pm || 8.30 - 6pm



            //TimeSpan morningShift = new TimeSpan(8, 30, 0);
            //TimeSpan afternoonShift = new TimeSpan(14, 0, 0);


            // Remove check in late 10 mins

            var student = await _myDBContext.Student.FindAsync(studentId);
            // Check if student is late (exceed 10 mins)
            if (checkInTime > shiftStartTimeplus10mins)  //Shift must disappear after shiftEndTime
            {
                System.Diagnostics.Debug.WriteLine("You're late!");
                shift.IsLate = true;
                shift.CheckInTime = currentTimeStamp;

                // Save the Shift object to the database.
                await _myDBContext.SaveChangesAsync();

                checkInEmail(student);
                return Ok(shift);
            }

            System.Diagnostics.Debug.WriteLine("ON TIME");
            // 30 , 100
            // Update the CheckInTime property of the Shift object.
            shift.CheckInTime = currentTimeStamp;


            // Save the Shift object to the database.
            await _myDBContext.SaveChangesAsync();

            checkInEmail(student);
            return Ok(shift);
        }

        private void checkInEmail(Student student)
        {
            // Get parent email
            string toAddress = student.ParentEmail;
            string fromAddress = student.Email;

            var currentTime = DateTime.Now.ToString("h:mm tt");
            var subject = "Child Check-in Notification";
            var body = $@"
        <p>Dear Parent,</p>
        <p>Your child has checked in:</p>
        <p><b>Check-in Time: {currentTime}</b></p>
        <p>Thank you!</p>
    ";
            using (var mailMessage = new MailMessage(fromAddress, toAddress))
            {
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("srw2799@gmail.com", "curfglljmqstifal");
                    smtpClient.EnableSsl = true;

                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to send email: " + ex.Message);
                    }
                }
            }
        }


        // When student press check out
        [HttpPut("CheckOut")]
        public async Task<IActionResult> CheckOutTimeAsync([FromQuery] int recruitmentId, [FromQuery] int studentId)
        {
            //DateTime testDateTime = new DateTime(2024, 1, 11, 18, 30, 0, 0, 0); // January 11, 2024, 6:30 PM

            var shift = await _myDBContext.Shift.FindAsync(recruitmentId, studentId);
            var recruitment = await _myDBContext.Recruitment.FindAsync(recruitmentId);
            var student = await _myDBContext.Student.FindAsync(studentId);

            var shiftEndTime = recruitment.EndTime;
            System.Diagnostics.Debug.WriteLine(shiftEndTime);
            var shiftEndtTimePlus1Hr = shiftEndTime.AddHours(1);
            System.Diagnostics.Debug.WriteLine(shiftEndtTimePlus1Hr);
            var shiftEndTimeMinus10Mins = shiftEndTime.AddMinutes(-10);
            System.Diagnostics.Debug.WriteLine(shiftEndTimeMinus10Mins);

            /////////////////////////////////////////////////////////////////////////////////////////////////
            //var currentTimeStamp = DateTime.UtcNow.AddHours(8);
            var currentTimeStamp = DateTime.UtcNow.AddHours(8);
            // Adding 9 hours for testing
            currentTimeStamp = currentTimeStamp.AddHours(9);
            /////////////////////////////////////////////////////////////////////////////////////////////////
            currentTimeStamp = currentTimeStamp.AddTicks(-(currentTimeStamp.Ticks % TimeSpan.TicksPerSecond));
            var checkOutTime = TimeOnly.FromDateTime(currentTimeStamp);

            // Checkout before check in
            if (shift.CheckInTime == new DateTime(2000, 1, 1, 0, 0, 0, 0))
            {
                //You have already checked checked out
                return BadRequest("You have not checked in yet");
            }

            // Check if checkoutTime has already been updated 
            if (shift.CheckOutTime != new DateTime(2000, 1, 1, 0, 0, 0, 0))
            {
                //You have already checked checked out
                return BadRequest("You have already checked out");
            }
            ////////////////////////////////////////////////////////////////////////////
            // Check if check out more than 10 mins early 
            //if (checkOutTime < shiftEndTimeMinus10Mins)
            //{
            //    return BadRequest("You cannot check out early");
            //}
            ///////////////////////////////////////////////////////////////////////////
            // Check if student overtime
            if (checkOutTime >= shiftEndtTimePlus1Hr)  //Shift must disappear after shiftEndTime
            {
                System.Diagnostics.Debug.WriteLine("You have worked overtime");
                shift.IsOvertime = true;
            }

            shift.CheckOutTime = currentTimeStamp;

            // Save the Shift object to the database.
            await _myDBContext.SaveChangesAsync();

            checkOutEmail(student);
            return Ok(shift);
        }

        // When student press check out
        [HttpPut("Comment")]
        public async Task<IActionResult> Comment([FromQuery] int recruitmentId, [FromQuery] int studentId, [FromQuery] string comment)
        {
            var shift = await _myDBContext.Shift.FindAsync(recruitmentId, studentId);
            shift.StudentComment = comment;
            // Save the Shift object to the database.
            await _myDBContext.SaveChangesAsync();
            return Ok(shift);
        }


            private void checkOutEmail(Student student)
        {
            // Get parent email
            string toAddress = student.ParentEmail;
            string fromAddress = student.Email;

            var currentTime = DateTime.Now.ToString("h:mm tt");
            var subject = "Child Check-out Notification";
            var body = $@"
        <p>Dear Parent,</p>
        <p>Your child has checked out:</p>
        <p><b>Check-out Time: {currentTime}</b></p>
        <p>Thank you!</p>
    ";
            using (var mailMessage = new MailMessage(fromAddress, toAddress))
            {
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("srw2799@gmail.com", "curfglljmqstifal");
                    smtpClient.EnableSsl = true;

                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to send email: " + ex.Message);
                    }
                }
            }
        }


        ////Get all shift details (Able to show job location, time, descripton and pic)
        //[HttpGet("{studentId}/{month}/{year}/getShiftDetails")]
        //public async Task<IActionResult> GetShiftDetails([FromRoute] int studentId, int month, int year)
        //{
        //    var shifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.CheckInTime.Year == year && s.CheckInTime.Month == month).ToListAsync();
        //    List<Recruitment> recruitments = new List<Recruitment>();
        //    List<Staff> shiftPICs = new List<Staff>();

        //    foreach (var shift in shifts)
        //    {
        //        var recruitmentIDAsString = shift.RecruitmentID.ToString();
        //        // For each shift get the recruitment
        //        var recruitment = await _myDBContext.Recruitment.FindAsync(recruitmentIDAsString);

        //        if (recruitment != null)
        //        {
        //            var shiftPIC = await _myDBContext.Staff.FindAsync(recruitment.StaffID);
        //            recruitments.Add(recruitment);
        //            shiftPICs.Add(shiftPIC);

        //        }

        //    }

        //    return Ok(new
        //    {
        //        shifts,
        //        recruitments,
        //        shiftPICs
        //    });
        //}


        //// POST api/<ShiftController>
        //[HttpPost]
        //public async Task<IActionResult> ShiftIn(Shift newShift)
        //{
        //    _myDBContext.Shift.Add(newShift);
        //    await _myDBContext.SaveChangesAsync();
        //    return Ok(newShift);
        //}

        //// PUT api/<ShiftController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ShiftController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
