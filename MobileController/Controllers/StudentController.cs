using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileController.Data;
using MobileController.DTO;
using MobileController.Models;
using MobileController.Services;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MobileController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public StudentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        [HttpGet("StudentsMonthList/{month}/{year}")]
        public async Task<IActionResult> GetMonthStudents(int month, int year)
        {
            //Getting all the shifts of every student in a particular month
            var studentMonthShifts = await _myDBContext.Shift.Join(
                                    _myDBContext.Recruitment,
                                    s => s.RecruitmentID,
                                    r => r.RecruitmentID,
                                    (s, r) => new
                                    {
                                        Shift = s,
                                        Recruitment = r
                                    })                                                                         
                                    .Where(sr => sr.Shift.CheckInTime.Year == year && sr.Shift.CheckInTime.Month == month && sr.Shift.IsAuthorized) // Add isCheck out //isApproved
                                    .ToListAsync();

            var uniqueStudentIDs = studentMonthShifts.Select(sr => sr.Shift.StudentID).Distinct().ToList();


            List<Payroll> payrollList = new List<Payroll>();

            foreach (var item in uniqueStudentIDs)
            {
                var student = await _myDBContext.Student.FindAsync(item);

                // Get the list of shift for a student in the month 
                var monthShifts = await _myDBContext.Shift.Join(
                                _myDBContext.Recruitment,
                                s => s.RecruitmentID,
                                r => r.RecruitmentID,
                                (s, r) => new
                                {
                                    Shift = s,
                                    Recruitment = r
                                })
                                .Where(sr => sr.Shift.StudentID == item &&
                                sr.Shift.CheckInTime.Year == year &&
                                sr.Shift.CheckInTime.Month == month)
                                .ToListAsync();

                DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                int totalDuration = 0;
                double totalWage = 0;

                // Calculate the total duration
                foreach (var shift in monthShifts)
                {
                    if (shift.Shift.CheckOutTime == ori || shift.Shift.CheckInTime == ori)
                        continue;

                    int duration = getDuration(shift.Recruitment.StartTime, shift.Recruitment.EndTime, shift.Shift.CheckInTime, shift.Shift.CheckOutTime, shift.Shift.IsOvertime);

                    double wage = getWage(duration);

                    totalDuration += duration;
                    totalWage += wage;
                }

                Payroll studentPayroll = new Payroll(student, totalDuration, totalWage);

                payrollList.Add(studentPayroll);

            }



            
            return (Ok(payrollList));

        }

        private double getWage(int totalDurationMinutes)
        {
            // Per hour rate and quarter hour rate
            double hourlyRate = 8.75;
            double quarterHourRate = 2.0;

            int roundedTotalMinutes = totalDurationMinutes / 15 * 15;
            //Calculating Wage
            int hours = roundedTotalMinutes / 60;
            int minutes = roundedTotalMinutes % 60;
            double wage = hours * hourlyRate + (minutes / 15) * quarterHourRate;

            return wage;
        }





        [HttpGet("StudentsList")]
        public async Task<IActionResult> GetStudents()
        {
            var results = await _myDBContext.Student.ToListAsync();

            List<StudentPerformanceItem> studentsExperience = new List<StudentPerformanceItem>();
            System.Diagnostics.Debug.WriteLine("Hello");

            foreach (var student in results)
            {
                System.Diagnostics.Debug.WriteLine(student.StudentID);
                var overallShift = await _myDBContext.Shift.Where(s => s.StudentID == student.StudentID && s.IsAuthorized).ToListAsync();


                //TimeOnly startTime, TimeOnly endTime, DateTime checkIn, DateTime checkOut, bool isOvertime)

                Dictionary<string, int> departmentDurations = new Dictionary<string, int>();

                var studentShifts = await _myDBContext.Shift
                    .Join(_myDBContext.Recruitment,
                        (s) => s.RecruitmentID,
                        (r) => r.RecruitmentID,
                        (s, r) => new { Shift = s, Recruitment = r })
                    .Join(_myDBContext.Staff,
                        (sr) => sr.Recruitment.StaffID,
                        (st) => st.StaffID,
                        (sr, st) => new { ShiftRecruitment = sr, Staff = st })
                    .Join(_myDBContext.Department,
                        (sst) => sst.Staff.DepartmentID,
                        (d) => d.DepartmentID,
                        (sst, d) => new
                        {
                            Shift = sst.ShiftRecruitment.Shift,
                            Recruitment = sst.ShiftRecruitment.Recruitment,
                            Staff = sst.Staff,
                            Department = d
                        })
                    .Where(sr => sr.Shift.StudentID == student.StudentID && sr.Shift.IsAuthorized)
                    .ToListAsync();

                int tRating = 0;
                foreach (var item in studentShifts)
                {
                    int duration = getDuration(item.Recruitment.StartTime, item.Recruitment.EndTime, item.Shift.CheckInTime, item.Shift.CheckOutTime, item.Shift.IsOvertime);
                    tRating += item.Shift.Rating;
                    string department = item.Department.DepartmentName;

                    if (departmentDurations.ContainsKey(department))
                    {
                        departmentDurations[department] += duration;
                    }
                    else
                    {
                        departmentDurations[department] = duration;
                    }
                }
                double ratingPercent = (double)tRating / (studentShifts.Count * 5);
                double aveRating = ratingPercent * 5;
                double roundedRating = Math.Round(aveRating, 2);

                foreach (var kvp in departmentDurations)
                {
                    System.Diagnostics.Debug.WriteLine($"Department: {kvp.Key}, Total Duration: {kvp.Value}"); // Or store this information as needed
                }

                StudentPerformanceItem studentPerformance = new StudentPerformanceItem(student, departmentDurations, roundedRating);
                studentsExperience.Add(studentPerformance);

            }


            return Ok(studentsExperience);

        }

        private int getDuration(TimeOnly startTime, TimeOnly endTime, DateTime checkIn, DateTime checkOut, bool isOvertime)
        {

            DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            TimeSpan duration, start, end;

            // Check if student check in and check out
            if (checkIn == ori || checkOut == ori)
            {
                return 0;
            }

            TimeOnly startTimeplus10Mins = startTime.Add(new TimeSpan(0, 10, 0));
            TimeSpan startSpanplus10Mins = new TimeSpan(startTimeplus10Mins.Hour, startTimeplus10Mins.Minute, startTimeplus10Mins.Second);

            // If student check in more than 1 hour or later 
            if (checkIn.TimeOfDay >= startSpanplus10Mins)
            {
                start = checkIn.TimeOfDay;
            }
            // If student check in early then reset to starttime or late not exceeding 1 hr
            else
            {
                start = new TimeSpan(startTime.Hour, startTime.Minute, startTime.Second);
            }

            TimeOnly endTimeplus1Hr = endTime.Add(new TimeSpan(1, 0, 0));
            TimeSpan endSpanplus1Hr = new TimeSpan(endTimeplus1Hr.Hour, endTimeplus1Hr.Minute, endTimeplus1Hr.Second);

            // If student works overtime (1 hour and above) // Even if student forget to checkout, the admin can reset IsOvertime to prevent overcalculation
            if (checkOut.TimeOfDay >= endSpanplus1Hr && isOvertime == true)
            {
                end = checkOut.TimeOfDay;
            }
            else
            {
                end = new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second);
            }

            // end will be default unless exceed 1 hour
            // start will be dafault unless late 1 hr
            duration = end - start;
            int totalDurationMinutes = (int)duration.TotalMinutes;
            int roundedTotalMinutes = totalDurationMinutes / 15 * 15;

            // Minus 1 hr for full shift due to break time
            if (endTime == new TimeOnly(17, 30, 0))
            {
                roundedTotalMinutes = roundedTotalMinutes - 60;
            }

            return roundedTotalMinutes;
        }

        [HttpGet("StudentProfile")]
        public async Task<IActionResult> GetStudentProfile(int stuID)
        {
            var results = await _myDBContext.Student.FindAsync(stuID);
            return Ok(results);

        }

        [HttpPost("RegisterStudent")]
        public async Task<IActionResult> AddStudent(Student student)
        {
            var results = await _myDBContext.Student.AddAsync(student);
            return Ok(results);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(int stuID, String password)
        {
            var user = await _myDBContext.Student.FirstOrDefaultAsync(u => u.StudentID == stuID);

            if (user != null && user.StudentPassword == password)
            {
                return Ok("Login successful!");
            }

            return BadRequest("Invalid ID or password");
        }

        //[HttpPut("StudentParent")]
        //public async Task<IActionResult> UpdateParent(int stuID, int parentID)
        //{
        //    var results = await _myDBContext.Student.FindAsync(stuID);
        //    results.ParentID = 
        //    return Ok(results);

        //}



    }
}
