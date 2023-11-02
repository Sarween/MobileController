using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileController.Models.Data;
using static System.Net.Mime.MediaTypeNames;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileController.Models;
using MobileController.Models.Data;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;
        //private readonly ShiftService _shiftService;

        public ShiftController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
            //_shiftService = shiftService;
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

            var currentDate = DateTime.UtcNow.AddHours(8).Date;

            //var studentShifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId).ToListAsync();

            //What if the student didnt check in

            var studentFutureJobs = await _myDBContext.Shift.Join(
                _myDBContext.Recruitment,
                s => s.RecruitmentID,
                r => r.RecruitmentID,
                (s, r) => new
                {
                    Shift = s,
                    Recruitment = r
                })
                .Where(sr => sr.Shift.StudentID == studentId) //Today check in is still visible       //&& sr.Recruitment.JobShiftDate.Date >= currentDate
                .OrderBy(sr => sr.Recruitment.JobShiftDate) 
                .ToListAsync();


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



            return Ok(studentFutureJobs.Select(recruitment => recruitment.Recruitment).ToList());
        }

        public enum CheckInStatus
        {
            OnTime,         // 0
            Late,           // 1
            OutsideBoundary // 2
        }


        // When student press check in
        [HttpPut("CheckIn")]
        public async Task<IActionResult> CheckInTimeAsync([FromQuery] int recruitmentId, [FromQuery] int studentId, [FromQuery] Location location)
        {
            var latitude = location.latitude;
            var longitude = location.longitude;

            // Get the Shift object from the database.

            var shift = await _myDBContext.Shift.FindAsync(recruitmentId, studentId);

            var recruitment = await _myDBContext.Recruitment.FindAsync(recruitmentId);

            var currentTimeStamp = DateTime.UtcNow.AddHours(8);
            var checkInTime = currentTimeStamp.TimeOfDay;

            var shiftEndTime = recruitment.EndTime.TimeOfDay;
            var shiftStartTime = recruitment.StartTime.TimeOfDay;
            var shiftStartTimeplus10mins = shiftStartTime.Add(new TimeSpan(0, 10, 0));

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

            // Check if the student is within the boundary.
            if (latitude < 20 || latitude > 40 || longitude < 90 || longitude > 110)
            {
                System.Diagnostics.Debug.WriteLine("You're not at company!");
                // 10 , 80
                // The student is not within the boundary.
                //return new JsonResult(new { CheckInStatus = CheckInStatus.OutsideBoundary, checkInTime });
                return BadRequest("You're not at company");
            }


            System.Diagnostics.Debug.WriteLine(shiftStartTime);


            // Either time is 8.30 -12.30 pm || 2 pm - 6pm || 8.30 - 6pm

    

            //TimeSpan morningShift = new TimeSpan(8, 30, 0);
            //TimeSpan afternoonShift = new TimeSpan(14, 0, 0);



            // Check if student is late (exceed 10 mins)
            if (checkInTime > shiftStartTimeplus10mins)  //Shift must disappear after shiftEndTime
            {
                System.Diagnostics.Debug.WriteLine("You're late!");
                shift.IsLate = true;
                shift.CheckInTime = currentTimeStamp;

                // Save the Shift object to the database.
                await _myDBContext.SaveChangesAsync();
                //return new JsonResult(new { CheckInStatus = CheckInStatus.Late, checkInTime });
                return Ok(new
                {
                    CheckInTime = checkInTime,
                    IsLate = true
                });
            }

            System.Diagnostics.Debug.WriteLine("ON TIME");
            // 30 , 100
            // Update the CheckInTime property of the Shift object.
            shift.CheckInTime = currentTimeStamp;


            // Save the Shift object to the database.
            await _myDBContext.SaveChangesAsync();
            //return new JsonResult(new { CheckInStatus = CheckInStatus.OnTime, checkInTime });
            return Ok(new
            {
                CheckInTime = checkInTime,
                IsLate = false
            });
        }


        // When student press check out
        [HttpPut("CheckOut")]
        public async Task<IActionResult> CheckOutTimeAsync([FromQuery] int recruitmentId, [FromQuery] int studentId)
        {
            var shift = await _myDBContext.Shift.FindAsync(recruitmentId, studentId);
            var recruitment = await _myDBContext.Recruitment.FindAsync(recruitmentId);

            var shiftEndTime = recruitment.EndTime.TimeOfDay;
            System.Diagnostics.Debug.WriteLine(shiftEndTime);
            var shiftEndtTimePlus1Hr = shiftEndTime.Add(new TimeSpan(1, 0, 0));
            System.Diagnostics.Debug.WriteLine(shiftEndtTimePlus1Hr);
            var shiftEndTimeMinus10Mins = shiftEndTime.Subtract(new TimeSpan(0, 10, 0));
            System.Diagnostics.Debug.WriteLine(shiftEndTimeMinus10Mins);

            var currentTimeStamp = DateTime.UtcNow.AddHours(8);
            var checkOutTime = currentTimeStamp.TimeOfDay;

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

            // Check if check out more than 10 mins early 
            if (checkOutTime < shiftEndTimeMinus10Mins)
            {
                return BadRequest("You cannot check out early");
            }

            // Check if student overtime
            if (checkOutTime >= shiftEndtTimePlus1Hr)  //Shift must disappear after shiftEndTime
            {
                System.Diagnostics.Debug.WriteLine("Overtime!");
                shift.IsOvertime = true;
            }

            shift.CheckOutTime = currentTimeStamp;

            // Save the Shift object to the database.
            await _myDBContext.SaveChangesAsync();
            return Ok(new
            {
                CheckOutTime = checkOutTime,
                Overtime = shift.IsOvertime
            });
            //return new JsonResult(new { overtime = shift.IsOvertime, checkOutTime }); 
        }

        // When student press get pay worksheet //must check if they check in & check out // check in anytime time before checkin time is reset. // if check out less than 1 hour is reset
        [HttpGet("GetWorksheet")]
        public async Task<IActionResult> GetWorksheet([FromQuery] int studentId, [FromQuery] int month, [FromQuery] int year)
        {
            // Approach 1
            //var shifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.CheckInTime.Year == year && s.CheckInTime.Month == month).ToListAsync();
            // Approaach 2
            var studentMonthShifts = await _myDBContext.Shift.Join(
                _myDBContext.Recruitment,
                s => s.RecruitmentID,
                r => r.RecruitmentID,
                (s, r) => new
                {
                    Shift = s,
                    Recruitment = r
                })
                .Where(sr => sr.Shift.StudentID == studentId && sr.Shift.CheckInTime.Year == year && sr.Shift.CheckInTime.Month == month) // Add isCheck out //isApproved
                .OrderBy(sr => sr.Recruitment.JobShiftDate)
                .ToListAsync();


            // Access the StartTime property of the Recruitment object.
            //var startTime = studentMonthShifts.First().Recruitment.StartTime.TimeOfDay;
            //System.Diagnostics.Debug.WriteLine(startTimeList);

            //var endTime = studentMonthShifts.First().Recruitment.EndTime.TimeOfDay;
            //System.Diagnostics.Debug.WriteLine(endTime);

            List<double> shiftWage = new List<double>();
            List<int> shiftDuration = new List<int>();
            //TimeSpan totalDuration = TimeSpan.Zero;
            int totalDuration = 0;

            DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            TimeSpan duration, start, end;


            var jbsbWorksheet = new List<JBSBWorksheet>();
            List<JBSBWorksheet> studentMonthlyJob = new List<JBSBWorksheet>();  

            //List<object> listofShifts = studentMonthShifts.SelectMany(recruitment => recruitment.Recruitment.JobShiftDate, shift => shift.Shift.CheckInTime, shift => shift.Shift.CheckOutTime, recruitment => recruitment.Recruitment.JobLocation, recruitment => recruitment.Recruitment.StaffID, shift => shift.Shift.IsAuthorized);
            //var listOfShifts = studentMonthShifts.SelectMany(
            //    recruitment => 
            //    new[] { (
            //    recruitment.Recruitment.RecruitmentID, 
            //    recruitment.Recruitment.JobShiftDate, 
            //    recruitment.Shift.CheckInTime, 
            //    recruitment.Shift.CheckOutTime, 
            //    recruitment.Recruitment.JobLocation, 
            //    recruitment.Recruitment.StaffID, 
            //    recruitment.Shift.IsAuthorized) 
            //    });

            foreach (var shift in studentMonthShifts)
            {
                // Get the start and end time of shift
                var startTime = shift.Recruitment.StartTime.TimeOfDay ;
                var endTime = shift.Recruitment.EndTime.TimeOfDay;
                // Check if student check in and check out
                if (shift.Shift.CheckInTime == ori || shift.Shift.CheckOutTime == ori)
                {
                    continue;
                }

                // If student check in more than 1 hour or later 
                if (shift.Shift.CheckInTime.TimeOfDay >= startTime.Add(new TimeSpan(1, 0, 0)))
                {
                    start = shift.Shift.CheckInTime.TimeOfDay;
                }
                // If student check in early then reset to starttime or late not exceeding 1 hr
                else
                {
                    start = startTime;
                }

                // If student works overtime (1 hour and above) // Even if student forget to checkout, the admin can reset IsOvertime to prevent overcalculation
                if (shift.Shift.CheckOutTime.TimeOfDay >= endTime.Add(new TimeSpan(1,0,0)) && shift.Shift.IsOvertime == true)
                {
                    end = shift.Shift.CheckOutTime.TimeOfDay;
                }
                else
                {
                    end = endTime;
                }

                // end will be default unless exceed 1 hour
                // start will be dafault unless late 1 hr
                duration = end - start;

     
                //Rounding of to hours
                int hours = duration.Hours;

                // Minus 1 hr for full shift due to break time
                if (shift.Recruitment.EndTime.TimeOfDay == new TimeSpan(17, 30, 0))
                {
                    hours = hours - 1;
                }


                double wage = hours * 8.75;

                shiftWage.Add(wage);
                shiftDuration.Add(hours);

                totalDuration += hours;

                //if (duration >= new TimeSpan(5, 0, 0))
                //{
                //    totalDuration += duration;
                //}

                System.Diagnostics.Debug.WriteLine(duration);
                
                System.Diagnostics.Debug.WriteLine("a" + totalDuration);


                //Adding every shift to worksheet
                var job = new JBSBWorksheet(
                    shift.Recruitment.JobShiftDate,
                    shift.Shift.CheckInTime, 
                    shift.Shift.CheckOutTime, 
                    hours, 
                    wage, 
                    shift.Recruitment.JobLocation, 
                    shift.Recruitment.StaffID, 
                    shift.Shift.IsAuthorized);

                studentMonthlyJob.Add(job);

            }
            var totalPay = Math.Round(totalDuration * 8.75);


            //Check if the shift is authorized

            // Get the duration 

            // Return all the shifts.
            return Ok(new
            {            
                studentId,
                month,
                year,
                studentMonthlyJob, // List of shifts in month
                totalPay = totalPay,
                totalDuration = totalDuration,
            });
            //shiftdates = studentMonthShifts.Select(recruitment => recruitment.Recruitment.JobShiftDate).ToList(),
            //    shiftCheckInTime = studentMonthShifts.Select(shift => shift.Shift.CheckInTime).ToList(),
            //    shiftCheckOutTime = studentMonthShifts.Select(shift => shift.Shift.CheckOutTime).ToList(),
            //    shiftDuration = shiftDuration.ToList(),
            //    shiftsWage = shiftWage.ToList(),
            //    jobLocation = studentMonthShifts.Select(recruitment => recruitment.Recruitment.JobLocation).ToList(),
            //    jobPic = studentMonthShifts.Select(recruitment => recruitment.Recruitment.StaffID).ToList(),
            //    jobVerification = studentMonthShifts.Select(shift => shift.Shift.IsAuthorized).ToList(),
        }





        //Get all shift details (Able to show job location, time, descripton and pic)
        [HttpGet("{studentId}/{month}/{year}/getShiftDetails")]
        public async Task<IActionResult> GetShiftDetails([FromRoute] int studentId, int month, int year)
        {
            var shifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.CheckInTime.Year == year && s.CheckInTime.Month == month).ToListAsync();
            List<Recruitment> recruitments = new List<Recruitment>();
            List<Staff> shiftPICs = new List<Staff>();

            foreach (var shift in shifts)
            {
                var recruitmentIDAsString = shift.RecruitmentID.ToString();
                // For each shift get the recruitment
                var recruitment = await _myDBContext.Recruitment.FindAsync(recruitmentIDAsString);

                if (recruitment != null)
                {
                    var shiftPIC = await _myDBContext.Staff.FindAsync(recruitment.StaffID);
                    recruitments.Add(recruitment);
                    shiftPICs.Add(shiftPIC);

                }

            }

            return Ok(new
            {
                shifts,
                recruitments,
                shiftPICs
            });
        }


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
