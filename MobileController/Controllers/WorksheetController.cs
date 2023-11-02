using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileController.Models.Data;
using MobileController.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileController.Models.Data;
using System;
using System.Net;
using Newtonsoft.Json;
using System.Drawing;
using System.Collections.Generic;

namespace MobileController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorksheetController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public WorksheetController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
            //_shiftService = shiftService;
        }


        [HttpGet("StudentPerformance")]
        public async Task<Leaderboard> CalcStudentPerformance( int studentId, int month, int year)
        {
            // Calculating punctuality
            var studentMonthShifts = await _myDBContext.Shift.Join(
                                            _myDBContext.Recruitment,
                                            s => s.RecruitmentID,
                                            r => r.RecruitmentID,
                                            (s, r) => new
                                            {
                                                Shift = s,
                                                Recruitment = r
                                            })
                                            .Where(sr => sr.Shift.StudentID == studentId && sr.Recruitment.JobShiftDate.Year == year && sr.Recruitment.JobShiftDate.Month == month) // Add isCheck out //isApproved
                                            .ToListAsync();


            // Calculating total duration
            int totalDuration = 0;
            DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            TimeSpan duration, start, end;
            TimeSpan startTime;
            TimeSpan endTime;
            // Calculating commitment
            foreach (var shift in studentMonthShifts)
            {
                // Get the start and end time of shift
                startTime = shift.Recruitment.StartTime.TimeOfDay;
                endTime = shift.Recruitment.EndTime.TimeOfDay;

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
                if (shift.Shift.CheckOutTime.TimeOfDay >= endTime.Add(new TimeSpan(1, 0, 0)) && shift.Shift.IsOvertime == true)
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

                totalDuration += hours;

                System.Diagnostics.Debug.WriteLine(duration);


                //return Ok(new
                //{
                //    lateCount,
                //    totalDuration,
                //    rating, 
                //    job_cancellation,
                //    score
                //});


            }
            System.Diagnostics.Debug.WriteLine("a" + totalDuration);


            // Late count
            int lateCount = studentMonthShifts.Where(o => o.Shift.IsLate).Count();

            // Calculate total rating
            var rating = studentMonthShifts.Sum(o => o.Shift.Rating);

            // Cancellation
            var cancellation = studentMonthShifts.Where(o => o.Shift.IsCancelled).Count();
            // Didn't check in
            var bunk = studentMonthShifts.Where(o => o.Shift.CheckInTime == ori).Count();

            var job_cancellation = bunk + cancellation;

            var score = totalDuration * 1 + rating * 4 - job_cancellation * 3 - lateCount * 2;

            System.Diagnostics.Debug.WriteLine(lateCount + " " + totalDuration + " " + rating + " " + job_cancellation);
            // Calculate rating
            //return Ok(new
            //{
            //    lateCount,
            //    totalDuration,
            //    rating,
            //    job_cancellation,
            //    score
            //});
            return new Leaderboard(lateCount, totalDuration, rating, job_cancellation, score);
        }




        [HttpGet("Leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int month, int year)
        {
            var studentMonthShifts = await _myDBContext.Shift.Join(
                                    _myDBContext.Recruitment,
                                    s => s.RecruitmentID,
                                    r => r.RecruitmentID,
                                    (s, r) => new
                                    {
                                        Shift = s,
                                        Recruitment = r
                                    })
                                    .Where(sr => sr.Shift.CheckInTime.Year == year && sr.Shift.CheckInTime.Month == month) // Add isCheck out //isApproved
                                    .ToListAsync();

            var uniqueStudentIDs = studentMonthShifts.Select(sr => sr.Shift.StudentID).Distinct().ToList();

            var studentLeaderboard = new Dictionary<int, int>();
            // Create a new HttpClient object.
            HttpClient client = new HttpClient();

            foreach (var item in uniqueStudentIDs)
            {
                System.Diagnostics.Debug.WriteLine("ID" + item);
                var studentShiftList = studentMonthShifts.FindAll(sr => sr.Shift.StudentID == item).ToList();

                // Calculating total duration
                int totalDuration = 0;
                DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                TimeSpan duration, start, end;
                TimeSpan startTime;
                TimeSpan endTime;
                // Calculating commitment
                foreach (var shift in studentShiftList)
                {
                    System.Diagnostics.Debug.WriteLine("Date" + shift.Recruitment.JobShiftDate);
                    // Get the start and end time of shift
                    startTime = shift.Recruitment.StartTime.TimeOfDay;
                    endTime = shift.Recruitment.EndTime.TimeOfDay;

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
                    if (shift.Shift.CheckOutTime.TimeOfDay >= endTime.Add(new TimeSpan(1, 0, 0)) && shift.Shift.IsOvertime == true)
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

                    totalDuration += hours;

                    System.Diagnostics.Debug.WriteLine("Hrs Worked " + hours);

                }

                // Late count
                int lateCount = studentMonthShifts.Where(o => o.Shift.IsLate && o.Shift.StudentID == item).Count();
                System.Diagnostics.Debug.WriteLine("Late  " + lateCount);

                // Calculate total rating
                var rating = studentMonthShifts.Where(o => o.Shift.StudentID == item).Sum(o => o.Shift.Rating);

                // Cancellation
                var cancellation = studentMonthShifts.Where(o => o.Shift.IsCancelled && o.Shift.StudentID == item).Count();
                // Didn't check in
                var bunk = studentMonthShifts.Where(o => o.Shift.CheckInTime == ori && o.Shift.StudentID == item).Count();

                var job_cancellation = bunk + cancellation;

                var score = totalDuration * 1 + rating * 4 - job_cancellation * 3 - lateCount * 2;

                System.Diagnostics.Debug.WriteLine(lateCount + " " + totalDuration + " " + rating + " " + job_cancellation);

                //int scoreRounded = (int)Math.Round(score);
                System.Diagnostics.Debug.WriteLine("Score " + score);
                studentLeaderboard.Add(item, score);

                // Make a GET request to the endpoint that returns the JSON response body.
                // HttpResponseMessage response = await client.GetAsync($"http://localhost:5012/api/Worksheet/StudentPerformance?studentId={item}&month={month}&year={year}");

            }
            //// Sort the studentsLeaderboard list by descending order of performance.score.
            // studentsLeaderboard.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            // Order the dictionary in descending order of score
            var sortedStudentScores = studentLeaderboard.OrderByDescending(x => x.Value).ToList();
            // Student Name, Score, Total Hours Worked

            return Ok(sortedStudentScores);
        }
        //// When student press get pay worksheet
        //[HttpGet("{studentId}/{month}/{year}")]
        //public async Task<IActionResult> GetWorksheet([FromRoute] int studentId, int month, int year)
        //{
        //    var shifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.CheckInTime.Year == year && s.CheckInTime.Month == month).ToListAsync();
        //    List<double> shiftWage = new List<double>();
        //    //TimeSpan totalDuration = TimeSpan.Zero;
        //    int totalDuration = 0;

        //    DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);

        //    foreach (var shift in shifts)
        //    {
        //        if (shift.CheckOutTime == ori || shift.CheckInTime == ori)
        //            continue;

        //        TimeSpan duration = shift.CheckOutTime - shift.CheckInTime;
        //        //Rounding of to hours
        //        int hours = duration.Hours;

        //        shiftWage.Add(hours * 8.75);

        //        totalDuration += hours;

        //        //if (duration >= new TimeSpan(5, 0, 0))
        //        //{
        //        //    totalDuration += duration;
        //        //}



        //        System.Diagnostics.Debug.WriteLine(duration);

        //        System.Diagnostics.Debug.WriteLine("a" + totalDuration);
        //    }
        //    var totalPay = Math.Round(totalDuration * 8.75);


        //    //Check if the shift is authorized

        //    // Get the duration 

        //    // Return all the shifts.
        //    return Ok(new
        //    {
        //        shift = shifts,
        //        shiftsWage = shiftWage,
        //        totalPay,
        //        totalDuration
        //    });
        //}


        //// When student press get worksummary report
        //[HttpGet("{month}/{year}/GetWorkSummaryReport")]
        //public async Task<IActionResult> GetWorkSummaryReport([FromRoute] int month, [FromRoute] int year)
        //{
        //    Shift studentShifts = await _myDBContext.Shift.FindAsync(month, year);

        //    if (studentShifts != null)
        //    {
        //        foreach (var shift in studentShifts)
        //        {
        //            // ...
        //        }
        //    }

        //    return Ok();

        //}








    }
}
