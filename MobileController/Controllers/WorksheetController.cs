using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using Newtonsoft.Json;
using System.Drawing;
using System.Collections.Generic;
using MobileController.Data;
using MobileController.DTO;
using MobileController.Models;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.Net.Mail;
using System.Net.Mime;
using System.Globalization;
using Microsoft.Identity.Client.Kerberos;
using SixLabors.Fonts;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;

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


        //[HttpGet("StudentPerformance")]
        //public async Task<Performance> CalcStudentPerformance(int studentId, int month, int year)
        //{

        //    //// Calculating punctuality
        //    //var studentMonthShifts = await _myDBContext.Shift.Join(
        //    //                                _myDBContext.Recruitment,
        //    //                                s => s.RecruitmentID,
        //    //                                r => r.RecruitmentID,
        //    //                                (s, r) => new
        //    //                                {
        //    //                                    Shift = s,
        //    //                                    Recruitment = r
        //    //                                })
        //    //                                .Where(sr =>
        //    //                                sr.Shift.StudentID == studentId &&
        //    //                                sr.Recruitment.JobShiftDate.HasValue
        //    //                                )
        //    //                                .AsEnumerable()
        //    //                                .Where(sr =>
        //    //                                sr.Recruitment.JobShiftDate.Value.Year == year &&
        //    //                                sr.Recruitment.JobShiftDate.Value.Month == month) // Add isCheck out //isApproved
        //    //                                .ToListAsync();


        //    // Fetches all shifts for the student from the database
        //    var query = _myDBContext.Shift
        //        .Where(s => s.StudentID == studentId)
        //        .ToList(); 

        //    // Join shift and recruitment
        //    // Get all the recruitments of the months which has already passed. No future ones
        //    var studentMonthShifts = query
        //        .Join(
        //            _myDBContext.Recruitment,
        //            s => s.RecruitmentID,
        //            r => r.RecruitmentID,
        //            (s, r) => new { Shift = s, Recruitment = r }
        //        )
        //        .Where(sr =>
        //            sr.Recruitment.JobShiftDate.HasValue && // Ensure JobShiftDate has a value
        //            sr.Recruitment.JobShiftDate.Value.Year == year &&
        //            sr.Recruitment.JobShiftDate.Value.Month == month &&
        //            sr.Recruitment.JobShiftDate.Value.Day < DateTime.Today.Day
        //            //sr.Shift.IsAuthorized == true
        //        )
        //        .ToList(); // Materialize the filtered shifts to a list

        //    // Calculating total duration
        //    int totalDuration = 0;
        //    DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
        //    TimeSpan duration, start, end;
        //    TimeOnly startTime;
        //    TimeOnly endTime;

        //    // Calculating commitment
        //    foreach (var shift in studentMonthShifts)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Date " + shift.Recruitment.JobShiftDate);
        //        //// Get the start and end time of shift
        //        //startTime = shift.Recruitment.StartTime;
        //        //endTime = shift.Recruitment.EndTime;

        //        //// Check if student check in and check out
        //        //if (shift.Shift.CheckInTime == ori || shift.Shift.CheckOutTime == ori)
        //        //{
        //        //    continue;
        //        //}

        //        //TimeOnly startTimeplus1Hr = startTime.Add(new TimeSpan(1, 0, 0));
        //        //TimeSpan startSpanplus1Hr = new TimeSpan(startTimeplus1Hr.Hour, startTimeplus1Hr.Minute, startTimeplus1Hr.Minute);

        //        //// If student check in more than 1 hour or later 
        //        //if (shift.Shift.CheckInTime.TimeOfDay >= startSpanplus1Hr)
        //        //{
        //        //    start = shift.Shift.CheckInTime.TimeOfDay;
        //        //}
        //        //// If student check in early then reset to starttime or late not exceeding 1 hr
        //        //else
        //        //{
        //        //    start = new TimeSpan(startTime.Hour, startTime.Minute, startTime.Minute);
        //        //}

        //        //TimeOnly endTimeplus1Hr = endTime.Add(new TimeSpan(1, 0, 0));
        //        //TimeSpan endSpanplus1Hr = new TimeSpan(endTimeplus1Hr.Hour, endTimeplus1Hr.Minute, endTimeplus1Hr.Minute);

        //        //// If student works overtime (1 hour and above) // Even if student forget to checkout, the admin can reset IsOvertime to prevent overcalculation
        //        //if (shift.Shift.CheckOutTime.TimeOfDay >= endSpanplus1Hr && shift.Shift.IsOvertime == true)
        //        //{
        //        //    end = shift.Shift.CheckOutTime.TimeOfDay;
        //        //}
        //        //else
        //        //{
        //        //    end = new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second);
        //        //}

        //        //// end will be default unless exceed 1 hour
        //        //// start will be dafault unless late 1 hr
        //        //duration = end - start;
        //        ////Rounding of to hours
        //        //int hours = duration.Hours;
        //        //// Minus 1 hr for full shift due to break time
        //        //if (shift.Recruitment.EndTime == new TimeOnly(17, 30, 0))
        //        //{
        //        //    hours = hours - 1;
        //        //}

        //        if (shift.Shift.IsCancelled || !shift.Shift.IsAuthorized) {
        //            continue; 
        //        }  
        //        else
        //        {
        //            int totalTime = getDuration(shift.Recruitment.StartTime, shift.Recruitment.EndTime, shift.Shift.CheckInTime, shift.Shift.CheckOutTime, shift.Shift.IsOvertime);
        //            System.Diagnostics.Debug.WriteLine(totalTime);

        //            totalDuration += totalTime;

        //        }



        //        //return Ok(new
        //        //{
        //        //    lateCount,
        //        //    totalDuration,
        //        //    rating, 
        //        //    job_cancellation,
        //        //    score
        //        //});


        //    }

        //    int totalMonthShiftApproved = studentMonthShifts.Where(o => o.Shift.IsAuthorized).Count();

        //    System.Diagnostics.Debug.WriteLine("Shifts" + totalMonthShiftApproved);
        //    // Late count
        //    int lateCount = studentMonthShifts.Where(o => o.Shift.IsLate && o.Shift.IsAuthorized).Count();

        //    // Calculating punctuality score
        //    var punctualityScore = (double)(totalMonthShiftApproved - lateCount)/ totalMonthShiftApproved;
        //    System.Diagnostics.Debug.WriteLine("Late "+ lateCount + " " + punctualityScore);

        //    // Calculate total rating
        //    var rating = studentMonthShifts.Where(o => o.Shift.IsAuthorized).Sum(o => o.Shift.Rating);

        //    double ratingScore = (double)rating /(totalMonthShiftApproved * 5);
        //    System.Diagnostics.Debug.WriteLine("Rating " + rating + " " + ratingScore);

        //    // Cancellation
        //    var cancellation = studentMonthShifts.Where(o => o.Shift.IsCancelled).Count();
        //    // Didn't check in
        //    var bunk = studentMonthShifts.Where(o => o.Shift.CheckInTime == ori && !o.Shift.IsCancelled).Count();

        //    var job_cancellation = bunk + cancellation;
        //    System.Diagnostics.Debug.WriteLine("Cancel " + cancellation + " " + bunk);


        //    // Include 
        //    int totalMonthShift = studentMonthShifts.Count();
        //    // Reliability score
        //    var reliabilityScore = (double)(totalMonthShift - job_cancellation) / totalMonthShift;
        //    System.Diagnostics.Debug.WriteLine("cancel " + job_cancellation + " " + reliabilityScore);

        //    // A point for every 15 minutes
        //    var commitmentScore = totalDuration / 15;
        //    System.Diagnostics.Debug.WriteLine("Commitment " + totalDuration + " " + commitmentScore);

        //    // Assign weightage
        //    //var commitmentMarks = commitmentScore * 0.1;
        //    //var ratingMarks = ratingScore * 0.4;
        //    //var reliabilityMarks = reliabilityScore * 0.3;
        //    //var punctualityMarks = punctualityScore * 0.2;


        //    // Aggregate based on assigned weightage
        //    //var score = commitmentMarks + ratingMarks + reliabilityMarks + punctualityMarks;

        //    //System.Diagnostics.Debug.WriteLine("Performance: " + commitmentMarks + " " + ratingMarks + " " + reliabilityMarks + " " + punctualityMarks);
        //    // Calculate rating
        //    //return Ok(new
        //    //{
        //    //    lateCount,
        //    //    totalDuration,
        //    //    rating,
        //    //    job_cancellation,
        //    //    score
        //    //});

        //    //var student = _myDBContext.Student.FindAsync(studentId);

        //    //return new Performance(lateCount, totalDuration, rating, job_cancellation, score);

        //    return new Performance(punctualityScore, ratingScore, commitmentScore, reliabilityScore);
        //}

        private Performance GetStudentPerformance(int studentId, int month, int year)
        {
            // Fetches all shifts for the student from the database
            var query = _myDBContext.Shift
                .Where(s => s.StudentID == studentId)
                .ToList();

            // Join shift and recruitment
            // Get all the recruitments of the months which has already passed. No future ones // Added isAuthorized to ensure
            var studentMonthShifts = query
                .Join(
                    _myDBContext.Recruitment,
                    s => s.RecruitmentID,
                    r => r.RecruitmentID,
                    (s, r) => new { Shift = s, Recruitment = r }
                )
                .Where(sr =>
                    sr.Recruitment.JobShiftDate.HasValue && // Ensure JobShiftDate has a value
                    sr.Recruitment.JobShiftDate.Value.Year == year &&
                    sr.Recruitment.JobShiftDate.Value.Month == month &&
                    sr.Shift.IsAuthorized
                /* sr.Recruitment.JobShiftDate.Value.Day < DateTime.Today.Day*/ // sr.Shift.IsAuthorized
                )
                .ToList(); // Materialize the filtered shifts to a list

            // Get student name
            string studentName = _myDBContext.Student.Where(s => s.StudentID == studentId)
                             .Select(s => s.StudentName)
                             .FirstOrDefault();

            // Calculating total duration
            int totalDuration = 0;
            DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            TimeSpan duration, start, end;
            TimeOnly startTime;
            TimeOnly endTime;

            // Calculating commitment
            foreach (var shift in studentMonthShifts)
            {
                System.Diagnostics.Debug.WriteLine("Date " + shift.Recruitment.JobShiftDate);

                // Checking if Authorized
                if (shift.Shift.IsCancelled || !shift.Shift.IsAuthorized)
                {
                    continue;
                }
                else
                {
                    int totalTime = getDuration(shift.Recruitment.StartTime, shift.Recruitment.EndTime, shift.Shift.CheckInTime, shift.Shift.CheckOutTime, shift.Shift.IsOvertime);
                    System.Diagnostics.Debug.WriteLine(totalTime);

                    totalDuration += totalTime;

                }

            }

            int totalMonthShiftApproved = studentMonthShifts.Where(o => o.Shift.IsAuthorized).Count();

            // Exit if no shifts are approved
            if (totalMonthShiftApproved == 0) {
                System.Diagnostics.Debug.WriteLine("FAILLLLLLLLLLL");
                return new Performance(studentName, 0, 0, 0, 0, 0, 0, 0);
            }

            System.Diagnostics.Debug.WriteLine("Shifts" + totalMonthShiftApproved);
            // Late count
            int lateCount = studentMonthShifts.Where(o => o.Shift.IsLate && o.Shift.IsAuthorized).Count();

            // Calculating punctuality score
            var punctualityScore = (double)(totalMonthShiftApproved - lateCount) / totalMonthShiftApproved;
            System.Diagnostics.Debug.WriteLine("Late " + lateCount + " " + punctualityScore);

            // Calculate total rating
            var rating = studentMonthShifts.Where(o => o.Shift.IsAuthorized).Sum(o => o.Shift.Rating);

            double ratingScore = (double)rating / (totalMonthShiftApproved * 5);
            System.Diagnostics.Debug.WriteLine("Rating " + rating + " " + ratingScore);

            var totalMonthShift = _myDBContext.Shift.Join(
                        _myDBContext.Recruitment,
                        s => s.RecruitmentID,
                        r => r.RecruitmentID,
                        (s, r) => new
                        {
                            Shift = s,
                            Recruitment = r
                        }).ToList();

            //Getting all the shifts of every student in a particular month
            var studentCancelMonthShifts = totalMonthShift                                                                        // WARNING-Test this, recently you added IsAuthorized
                                    .Where(sr => sr.Recruitment.JobShiftDate.HasValue && // Ensure JobShiftDate has a value
                                            sr.Recruitment.JobShiftDate.Value.Year == year &&
                                            sr.Recruitment.JobShiftDate.Value.Month == month &&
                                            sr.Shift.StudentID == studentId
                                            && sr.Shift.IsCancelled)
                                    .Count();
                                 //   == year && sr.Shift.CheckInTime.Month == month && sr.Shift.StudentID == studentId && sr.Shift.IsCancelled) // Add isCheck out //isApproved
            System.Diagnostics.Debug.WriteLine("CHECKOING  " + studentId + " " + studentCancelMonthShifts);
            // Get the current date as DateOnly
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            //Getting all the shifts of every student in a particular month
            var studentBunkMonthShifts = totalMonthShift
                                    .Where(sr => sr.Recruitment.JobShiftDate.HasValue && // Ensure JobShiftDate has a value
                                            sr.Recruitment.JobShiftDate.Value.Year == year &&
                                            sr.Recruitment.JobShiftDate.Value.Month == month &&
                                            sr.Shift.StudentID == studentId &&
                                            sr.Shift.CheckInTime == ori &&
                                            !sr.Shift.IsCancelled &&
                                            sr.Recruitment.JobShiftDate.Value < currentDate)
                                    .Count();
            System.Diagnostics.Debug.WriteLine("BUNK  " + studentId + " " + studentBunkMonthShifts);
            //studentMonthShifts.Where(o => o.Shift.IsAuthorized).Count();
            // Cancellation

            //var cancellation = studentMonthShifts.Where(o => o.Shift.IsCancelled).Count();
            // Didn't check in
            //var bunk = studentMonthShifts.Where(o => o.Shift.CheckInTime == ori && !o.Shift.IsCancelled).Count();

            var job_cancellation = studentCancelMonthShifts + studentBunkMonthShifts;
            System.Diagnostics.Debug.WriteLine("Cancel " + studentCancelMonthShifts + " " + studentBunkMonthShifts);


            //// Include 
            //int totalMonthShift = _myDBContext.Shift.Join(
            //                        _myDBContext.Recruitment,
            //                        s => s.RecruitmentID,
            //                        r => r.RecruitmentID,
            //                        (s, r) => new
            //                        {
            //                            Shift = s,
            //                            Recruitment = r
            //                        })                                                                          
            //                        .Where(sr => sr.Shift.CheckInTime.Year == year && sr.Shift.CheckInTime.Month == month && sr.Shift.StudentID == studentId) 
            //                        .Count();
            int totalMonthShiftCount = totalMonthShift.Count();

            // Reliability score
            var reliabilityScore = (double)(totalMonthShiftCount - job_cancellation) / totalMonthShiftCount;
            System.Diagnostics.Debug.WriteLine("cancel " + job_cancellation + " " + reliabilityScore);

            // A point for every 15 minutes
            var commitmentScore = totalDuration / 15;
            System.Diagnostics.Debug.WriteLine("Commitment " + totalDuration + " " + commitmentScore);

            var overtimeCount = studentMonthShifts.Where(o => o.Shift.IsOvertime).Count();
            var score = punctualityScore + ratingScore + commitmentScore + reliabilityScore;

            return new Performance(studentName, punctualityScore, ratingScore, commitmentScore, reliabilityScore, totalDuration, overtimeCount, score);
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

            // If student check in more than 10 minutes or later 
            if (checkIn.TimeOfDay >= startSpanplus10Mins)
            {
                start = checkIn.TimeOfDay;
            }
            // If student check in early then reset to starttime or late not exceeding 10 mins
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

        [HttpGet("Duration")]
        public async Task<IActionResult> getDuration2(TimeOnly startTime, TimeOnly endTime, DateTime checkIn, DateTime checkOut, bool isOvertime)
        {

            DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            TimeSpan duration, start, end;

            // Check if student check in and check out
            if (checkIn == ori || checkOut == ori)
            {
                return Ok(0);
            }

            TimeOnly startTimeplus1Hr = startTime.Add(new TimeSpan(1, 0, 0));
            TimeSpan startSpanplus1Hr = new TimeSpan(startTimeplus1Hr.Hour, startTimeplus1Hr.Minute, startTimeplus1Hr.Second);

            // If student check in more than 1 hour or later 
            if (checkIn.TimeOfDay >= startSpanplus1Hr)
            {
                start = checkIn.TimeOfDay;
            }
            // If student check in early then reset to starttime or late not exceeding 1 hr
            else
            {
                start = new TimeSpan(startTime.Hour, startTime.Minute, startTime.Minute);
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

            return Ok(roundedTotalMinutes);
        }



        [HttpGet("Leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int month, int year)
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
                                    .Where(sr => sr.Shift.CheckInTime.Year == year && sr.Shift.CheckInTime.Month == month) // Add isCheck out //isApproved
                                    .ToListAsync();

            var uniqueStudentIDs = studentMonthShifts.Select(sr => sr.Shift.StudentID).Distinct().ToList();

            if (!uniqueStudentIDs.Any())
            {
                return Ok("No student participated");
            }

            List<LeaderBoard> leaderBoard = new List<LeaderBoard>();
            // Create a new HttpClient object.
            HttpClient client = new HttpClient();

            List<Performance> performanceList = new List<Performance>();



            foreach (var item in uniqueStudentIDs)
            {
                var student = await _myDBContext.Student.FindAsync(item);

                System.Diagnostics.Debug.WriteLine("ID" + item);
                var studentShiftList = studentMonthShifts.FindAll(sr => sr.Shift.StudentID == item).ToList();

                Performance performance = GetStudentPerformance(student.StudentID, month, year);

                performanceList.Add(performance);
                System.Diagnostics.Debug.WriteLine(performance.rating);

                

                //Start

                //// Calculating total duration
                //int totalDuration = 0;
                //DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);
                //TimeSpan duration, start, end;
                //TimeOnly startTime;
                //TimeOnly endTime;

                //// Late count
                //int lateCount = studentMonthShifts.Where(o => o.Shift.IsLate && o.Shift.StudentID == item).Count();
                //System.Diagnostics.Debug.WriteLine("Late  " + lateCount);

                //// Calculate total rating
                //var rating = studentMonthShifts.Where(o => o.Shift.StudentID == item).Sum(o => o.Shift.Rating);

                //// Cancellation
                //var cancellation = studentMonthShifts.Where(o => o.Shift.IsCancelled && o.Shift.StudentID == item).Count();
                //// Didn't check in
                //var bunk = studentMonthShifts.Where(o => o.Shift.CheckInTime == ori && o.Shift.StudentID == item).Count();

                //var job_cancellation = bunk + cancellation;

                //var score = totalDuration * 1 + rating * 4 - job_cancellation * 3 - lateCount * 2; //Normalize

                //System.Diagnostics.Debug.WriteLine(lateCount + " " + totalDuration + " " + rating + " " + job_cancellation);

                ////int scoreRounded = (int)Math.Round(score);
                //System.Diagnostics.Debug.WriteLine("Score " + score);
                //var candidate = new LeaderBoard(student, score);
                //leaderBoard.Add(candidate);

                //End

                // Make a GET request to the endpoint that returns the JSON response body.
                // HttpResponseMessage response = await client.GetAsync($"http://localhost:5012/api/Worksheet/StudentPerformance?studentId={item}&month={month}&year={year}");

            }

            // Finding maximum values for each attribute
            double maxPunctuality;
            double maxRating;
            double maxCommitment;
            double maxReliability;

            // Can only rank if there is atleast 2 students
            if (uniqueStudentIDs.Count != 1)
            {
                // Finding maximum values for each attribute
                maxPunctuality = performanceList.Max(p => p.punctuality);
                maxRating = performanceList.Max(p => p.rating);
                maxCommitment = performanceList.Max(p => p.commitment);
                maxReliability = performanceList.Max(p => p.reliability);
            }
            else
            {
                // Finding maximum values for each attribute
                maxPunctuality = 1;
                maxRating = 1;
                maxCommitment = performanceList[0].commitment; // Initially 1
                maxReliability = 1;
            }

            //// Finding maximum values for each attribute
            //double maxPunctuality = performanceList.Max(p => p.punctuality);
            //double maxRating = performanceList.Max(p => p.rating);
            //double maxCommitment = performanceList.Max(p => p.commitment);
            //double maxReliability = performanceList.Max(p => p.reliability);

            System.Diagnostics.Debug.WriteLine(maxPunctuality);

            List<Performance> normalizedPerfomance = new List<Performance>();

            foreach (var item in performanceList) {
                // Linear Normalization
                double nPunctuality = item.punctuality / maxPunctuality;
                double nRating = item.rating / maxRating;
                double nCommitment = item.commitment / maxCommitment;
                double nReliability = item.reliability / maxReliability;

                // Assign weightage
                double wPunctuality = nPunctuality * 0.2;
                double wRating = nRating * 0.4;
                double wCommitment = nCommitment * 0.1;
                double wReliability = nReliability * 0.3;

                double score = wPunctuality + wRating + wCommitment + wReliability;

                Performance p = new Performance(item.name, nPunctuality,nRating,nCommitment,nReliability, item.duration, item.overtimeCount, score);

                normalizedPerfomance.Add(p);

            }

            List<Performance> sortedPerformances = normalizedPerfomance.OrderByDescending(p => p.score).ToList();

            return Ok(sortedPerformances);



            //// Sort the studentsLeaderboard list by descending order of performance.score.
            // studentsLeaderboard.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            // Order the dictionary in descending order of score
            //leaderBoard.Sort((x, y) => y.score.CompareTo(x.score));
            // Student Name, Score, Total Hours Worked

            //return Ok(leaderBoard);
        }



        // When student press get pay worksheet
        [HttpGet("GetWorksheet")]
        public async Task<IActionResult> GetWorksheet(int studentId, int month, int year)
        {
            // Not checking for isAuthorized because getDuration will already check for it.
            var studentMonthShifts = await _myDBContext.Shift.Join(
                        _myDBContext.Recruitment,
                        s => s.RecruitmentID,
                        r => r.RecruitmentID,
                        (s, r) => new
                        {
                            Shift = s,
                            Recruitment = r
                        })
                        .Where(sr => sr.Shift.StudentID == studentId && 
                        sr.Shift.CheckInTime.Year == year && 
                        sr.Shift.CheckInTime.Month == month)
                        .ToListAsync();

            var shifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.CheckInTime.Year == year && s.CheckInTime.Month == month).ToListAsync();
            List<double> shiftWage = new List<double>();
            List<JBSBWorksheetShift> jBSBWorksheets = new List<JBSBWorksheetShift>();
            //TimeSpan totalDuration = TimeSpan.Zero;


            DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);

            foreach (var shift in studentMonthShifts)
            {
                if (shift.Shift.CheckOutTime == ori || shift.Shift.CheckInTime == ori)
                    continue;

                //TimeSpan duration = shift.Shift.CheckOutTime - shift.Shift.CheckInTime;
                //// Calculatin Duration in minutes
                //int totalDurationMinutes = (int)duration.TotalMinutes;
                int totalTime = getDuration(shift.Recruitment.StartTime, shift.Recruitment.EndTime, shift.Shift.CheckInTime, shift.Shift.CheckOutTime, shift.Shift.IsOvertime);
                //int roundedTotalMinutes = totalDurationMinutes / 15 * 15;
                //Calculating Wage
                //int hours = totalTime / 60;
                //int minutes = totalTime % 60;
                //double wage = hours * 8.75 + (minutes/15) * 2; //Add wage for Every 15 mins - RM 2.00\\
                double wage = getWage(totalTime);

                string supervisor = await _myDBContext.Staff.Where(s => s.StaffID == shift.Recruitment.StaffID).Select(s => s.StaffName).FirstOrDefaultAsync();

                // Supervisor will be allowed to 
                JBSBWorksheetShift obj = new JBSBWorksheetShift(shift.Recruitment.JobShiftDate, shift.Shift.CheckInTime, shift.Shift.CheckOutTime, totalTime, wage, shift.Recruitment.JobLocation, supervisor, shift.Shift.IsAuthorized, shift.Shift.IsLate, shift.Shift.IsOvertime, shift.Shift.StaffReview, shift.Shift.Rating, shift.Shift.StudentComment);
                jBSBWorksheets.Add(obj);
                //shiftWage.Add(hours * 8.75);

                //totalDuration += hours;

                //if (duration >= new TimeSpan(5, 0, 0))
                //{
                //    totalDuration += duration;
                //}

            }

            //var totalPay = Math.Round(totalDuration * 8.75);

            // Checking if the shift is authorized
            double totalPay = 0;
            int totalDuration = 0;
            foreach (var shift in jBSBWorksheets)
            {
                totalPay = shift.shiftsWage + totalPay;
                totalDuration = shift.shiftDuration + totalDuration;
                //if (shift.jobVerification == true)
                //{
                //    totalPay = shift.shiftsWage + totalPay;
                //    totalDuration = shift.shiftDuration + totalDuration;

                //}
                //else
                //    continue;
            }

            jBSBWorksheets = jBSBWorksheets.OrderByDescending(obj => obj.shiftdates).ToList();


            //Check if the shift is authorized

            // Get the duration 

            // Return all the shifts.
            //return Ok(new
            //{
            //    shift = shifts,
            //    shiftsWage = shiftWage,
            //    totalPay,
            //    totalDuration
            //});
            WorksheetReport worksheetReport = new WorksheetReport(jBSBWorksheets, totalDuration,totalPay);



            return Ok(worksheetReport);
        }

        // Get PDF/{studentId}/{month}/{year}
        [HttpGet("generatepdf")] 
        public async Task<IActionResult> GeneratePDF(int studentId, int month, int year)
        {
            string studentname = await _myDBContext.Student.Where(s => s.StudentID == studentId).Select(s => s.StudentName).FirstOrDefaultAsync();

            var studentMonthShifts = await _myDBContext.Shift.Join(
            _myDBContext.Recruitment,
            s => s.RecruitmentID,
            r => r.RecruitmentID,
            (s, r) => new
            {
                Shift = s,
                Recruitment = r
            })
            .Where(sr => sr.Shift.StudentID == studentId && sr.Shift.CheckInTime.Year == year && sr.Shift.CheckInTime.Month == month && sr.Shift.IsAuthorized) // Add isCheck out //isApproved
            .ToListAsync();



            var shifts = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.CheckInTime.Year == year && s.CheckInTime.Month == month).ToListAsync();
            List<double> shiftWage = new List<double>();
            List<JBSBWorksheetShift> jBSBWorksheets = new List<JBSBWorksheetShift>();

            DateTime ori = new DateTime(2000, 1, 1, 0, 0, 0, 0);

            foreach (var shift in studentMonthShifts)
            {
                if (shift.Shift.CheckOutTime == ori || shift.Shift.CheckInTime == ori)
                    continue;

                //TimeSpan duration = shift.Shift.CheckOutTime - shift.Shift.CheckInTime;
                int totalTime = getDuration(shift.Recruitment.StartTime, shift.Recruitment.EndTime, shift.Shift.CheckInTime, shift.Shift.CheckOutTime, shift.Shift.IsOvertime);
                double wage = getWage(totalTime);

                //// Calculatin Duration in minutes
                //int totalDurationMinutes = (int)duration.TotalMinutes;
                //int roundedTotalMinutes = totalDurationMinutes / 15 * 15;
                ////Calculating Wage
                //int hours = roundedTotalMinutes / 60;
                //int minutes = roundedTotalMinutes % 60;
                ////int hours = duration.Hours;
                ////int minutes = duration.Minutes;
                ////int roundedMinutes = minutes / 15 * 15;
                //double wage = hours * 8.75 + (minutes / 15) * 2; //Add wage for Every 15 mins - RM 2.00\\

                string supervisor = await _myDBContext.Staff.Where(s => s.StaffID == shift.Recruitment.StaffID).Select(s => s.StaffName).FirstOrDefaultAsync();

                // Supervisor will be allowed to 
                JBSBWorksheetShift obj = new JBSBWorksheetShift(shift.Recruitment.JobShiftDate, shift.Shift.CheckInTime, shift.Shift.CheckOutTime, totalTime, wage, shift.Recruitment.JobLocation, supervisor, shift.Shift.IsAuthorized, shift.Shift.IsLate, shift.Shift.IsOvertime, shift.Shift.StaffReview, shift.Shift.Rating, shift.Shift.StudentComment);
                jBSBWorksheets.Add(obj);

            }
            if (jBSBWorksheets.Count == 0)
            {
                return BadRequest();
            }

            double totalPay = 0;
            int totalDuration = 0;
            foreach (var shift in jBSBWorksheets)
            {
                totalPay = shift.shiftsWage + totalPay;
                totalDuration = shift.shiftDuration + totalDuration;
            }


            var document = new PdfDocument();
            //string HtmlContent = "<h3>JBSB WORKSHEET - WAREHOUSE</h3>";
            //HtmlContent += "<h4>Name</h4>";
            //HtmlContent += "<p></p>";

            string HtmlContent = @"
                <style>
                    /* General Styles */
                    body {
                        font-family: Arial, sans-serif;
                        margin: 20px;
                    }
        
                    h3 {
                        color: #333;
                        text-align: center;
                        margin-bottom: 20px;
                    }
        
                    h4 {
                        color: #333;
                        margin-bottom: 5px;
                    }
        
                    p {
                        color: #333;
                        margin-bottom: 15px;
                    }
        
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin-bottom: 20px;
                    }
        
                    th, td {
                        border: 1px solid #ddd;
                        padding: 8px;
                        text-align: left;
                    }
        
                    th {
                        background-color: #f5f5f5;
                    }
                </style>

                <div style='text-align: center;'>
                    <h3>JBSB WORKSHEET - WAREHOUSE</h3>
                    <table>
                        <tr>";
                            HtmlContent += "<td>Name: "+ studentname + "</td>";
                            HtmlContent += "<td>Student ID: " + studentId + "</td>";
                            HtmlContent += "<td>Month: " + month + "/" + year +  "</td>";
            HtmlContent += @"
                        </tr>
                    </table>
                </div>

                <br>

                <table>
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Check In</th>
                            <th>Check Out</th>
                            <th>Wages (RM)</th>
                            <th>Job Location</th>
                            <th>PIC Name</th>
                        </tr>
                    </thead>
                    <tbody>";

            if (jBSBWorksheets != null)
            {
                foreach (JBSBWorksheetShift obj in jBSBWorksheets)
                {
                    HtmlContent += "<tr>";
                    HtmlContent += "<td>" + obj.shiftdates + "</td>";
                    HtmlContent += "<td>" + obj.shiftCheckInTime.TimeOfDay + "</td>";
                    HtmlContent += "<td>" + obj.shiftCheckOutTime.TimeOfDay + "</td>";
                    HtmlContent += "<td>" + obj.shiftsWage + "</td>";
                    HtmlContent += "<td>" + obj.jobLocation + "</td>";
                    HtmlContent += "<td>" + obj.jobPIC + "</td>";
                    HtmlContent += "</tr>";
                }
                //HtmlContent += @" 
                //        <tr>
                //            <td></td>
                //            <td>Data 2</td>
                //            <td>Data 3</td>
                //            <td>Data 1</td>
                //            <td>Data 2</td>
                //            <td>Data 3</td>

                //        </tr>";
            }
            HtmlContent += @"
                    </tbody>
                </table>

                <div style='text-align: right; margin-top: 20px;'>
                    <h4>Total Salary</h4>";
            HtmlContent += "<p> RM "+ totalPay +"</p>";
            HtmlContent += @"
                </div>

            ";

            PdfGenerator.AddPdfPages(document, HtmlContent, PageSize.A4);
            byte[]? response = null;
            using(MemoryStream memoryStream = new MemoryStream())
            {
                document.Save(memoryStream);
                response = memoryStream.ToArray(); 
            }

            DateTimeFormatInfo monthInfo = new DateTimeFormatInfo();
            string monthName = monthInfo.GetMonthName(month);

            String Filename = "Worksheet_" + studentId + "_" + month + "_" + year + ".pdf";


            var file = File(response, "application/pdf", Filename);

            // Get parent email
            var student = await _myDBContext.Student.FindAsync(studentId);

            //string toAddress = "sarweenalages@gmail.com";
            string toAddress = student.Email;
            string fromAddress = "srw2799@gmail.com";

            var currentTime = DateTime.Now.ToString("h:mm tt");
            var subject = "JBSB Worksheet " + monthName + " " + year;
            var body = $@"
        <p>Dear Student,</p>
        <p>Attached is your JBSB Worksheet.</p>
        <p>Thank you!</p>
    ";
            using (var mailMessage = new MailMessage(fromAddress, toAddress))
            {
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                using (MemoryStream ms = new MemoryStream(response))
                {
                    mailMessage.Attachments.Add(new Attachment(ms, new ContentType("application/pdf"))
                    {
                        Name = Filename // Set the file name
                    });


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
                            return BadRequest(ex);
                        }
                    }

                }



            }



            return File(response, "application/pdf", Filename);
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

        //private double getWage(TimeOnly startTime, TimeOnly endTime, DateTime checkIn, DateTime checkOut, bool isOvertime)
        //{
        //    TimeOnly checkInTime = TimeOnly.FromDateTime(checkIn);
        //    TimeOnly checkOutTime = TimeOnly.FromDateTime(checkOut);
        //    //if (isOvertime)
        //    //{
        //    //    TimeOnly checkOutTime = TimeOnly.FromDateTime(checkOut);
        //    //}
        //    //else
        //    //{
        //    //    TimeOnly checkOutTime = endTime;
        //    //}
        //    System.Diagnostics.Debug.WriteLine($" checkInTime: {checkInTime} hours");
        //    System.Diagnostics.Debug.WriteLine($" checkInTime: {checkInTime} hours");
        //    System.Diagnostics.Debug.WriteLine($" checkInTime: {checkInTime} hours");
        //    System.Diagnostics.Debug.WriteLine($" checkOUTTime: {checkOutTime} hours");

        //    // Per hour rate and overtime rate
        //    double hourlyRate = 8.75;
        //    double overtimeRate = 2.0;
        //    double latePenaltyRate = hourlyRate;

        //    // Calculate total worked hours
        //    double totalWorkedHours = CalculateTotalWorkedHours(startTime, endTime, checkInTime, checkOutTime);
        //    System.Diagnostics.Debug.WriteLine($" totalWorkedHours: {totalWorkedHours} hours");

        //    // Calculate regular hours and overtime hours
        //    double regularHours = 0;
        //    int overtimeMinutesCount = 0;

        //    if (checkInTime < endTime && checkOutTime > startTime)
        //    {
        //        double workedHrs = (endTime - checkInTime).TotalHours;
        //        System.Diagnostics.Debug.WriteLine($" workedHrs: {workedHrs} hours");
        //        // Worker worked during the shift
        //        regularHours = Math.Max(Math.Min(totalWorkedHours, (endTime - checkInTime).TotalHours), 0);
        //        if (isOvertime)
        //        {
        //            overtimeMinutesCount = CalculateLateCheckOutTime(endTime, checkOutTime) / 15;
        //        }

        //    }

        //    // Calculate regular wage and overtime wage
        //    double regularWage = regularHours * hourlyRate;
        //    double overtimeWage = overtimeMinutesCount * overtimeRate;
        //    //double overtimeWage = Math.Ceiling(overtimeHours / 0.25) * overtimeRate; // Round up to the nearest 15 minutes


        //    // Calculate total wage
        //    double totalWage = regularWage + overtimeWage;

        //    System.Diagnostics.Debug.WriteLine($"Regular Hours: {regularHours} hours");
        //    System.Diagnostics.Debug.WriteLine($"Overtime Hours: {overtimeMinutesCount} count");
        //    System.Diagnostics.Debug.WriteLine($"Regular Wage: RM {regularWage:F2}");
        //    System.Diagnostics.Debug.WriteLine($"Overtime Wage: RM {overtimeWage:F2}");
        //    System.Diagnostics.Debug.WriteLine($"Total Wage: RM {totalWage:F2}");

        //    return totalWage;
        //    // Back up
        //    //// Per hour rate and overtime rate
        //    //double hourlyRate = 8.75;
        //    //double overtimeRate = 2.0;

        //    //int roundedTotalMinutes = totalDurationMinutes / 15 * 15;
        //    ////Calculating Wage
        //    //int hours = roundedTotalMinutes / 60;
        //    //int minutes = roundedTotalMinutes % 60;
        //    //double wage = hours * 8.75 + (minutes / 15) * 2; //Add wage for Every 15 mins - RM 2.00\\

        //    //return wage;
        //}
        static int CalculateLateCheckOutTime(TimeOnly shiftEndTime, TimeOnly checkOutTime)
        {
            TimeSpan shiftEnd = new TimeSpan(shiftEndTime.Hour, shiftEndTime.Minute, 0);
            TimeSpan checkOut = new TimeSpan(checkOutTime.Hour, checkOutTime.Minute, 0);

            int lateCheckOutMinutes = (int)Math.Ceiling((checkOut - shiftEnd).TotalMinutes);

            // Round up to the nearest 15 minutes
            return (lateCheckOutMinutes / 15) * 15;
        }

        static double CalculateTotalWorkedHours(TimeOnly shiftStartTime, TimeOnly shiftEndTime, TimeOnly checkInTime, TimeOnly checkOutTime)
        {
            TimeSpan shiftStart = new TimeSpan(shiftStartTime.Hour, shiftStartTime.Minute, 0);
            TimeSpan shiftEnd = new TimeSpan(shiftEndTime.Hour, shiftEndTime.Minute, 0);
            TimeSpan checkIn = new TimeSpan(checkInTime.Hour, checkInTime.Minute, 0);
            TimeSpan checkOut = new TimeSpan(checkOutTime.Hour, checkOutTime.Minute, 0);

            System.Diagnostics.Debug.WriteLine($" shiftStart: {shiftStart} hours");
            System.Diagnostics.Debug.WriteLine($" shiftEnd: {shiftEnd} hours");
            System.Diagnostics.Debug.WriteLine($" checkIn: {checkIn} hours");
            System.Diagnostics.Debug.WriteLine($" checkOut: {checkOut} hours");

            TimeSpan totalWorkedTime = checkOut - checkIn;
            System.Diagnostics.Debug.WriteLine($" totalWorkedTime: {totalWorkedTime} hours");
            TimeSpan shiftDuration = shiftEnd - shiftStart;
            System.Diagnostics.Debug.WriteLine($" shiftDuration: {shiftDuration} hours");

            return Math.Max(Math.Max(totalWorkedTime.TotalHours, shiftDuration.TotalHours), 0);
        }

        private void test()
        {
            // Get parent email
            string toAddress = "sarweenalages@gmail.com";
            string fromAddress = "sarweenalages@gmail.com";

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

        // Get students performance in the Ejbsb program
        [HttpGet("OverallHours")]
        public async Task<IActionResult> getOverallHours(int studentId)
        {
            var overallShift = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.IsAuthorized).ToListAsync();

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
                .Where(sr => sr.Shift.StudentID == studentId && sr.Shift.IsAuthorized)
                .ToListAsync();

            foreach (var item in studentShifts)
            {
                int duration = getDuration(item.Recruitment.StartTime, item.Recruitment.EndTime, item.Shift.CheckInTime, item.Shift.CheckOutTime, item.Shift.IsOvertime);
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

            foreach (var kvp in departmentDurations)
            {
                System.Diagnostics.Debug.WriteLine($"Department: {kvp.Key}, Total Duration: {kvp.Value}"); // Or store this information as needed
            }

            return Ok(departmentDurations);

        }

        // Get the students hours worked for the front page
        [HttpGet("HoursWorked")]
        public async Task<IActionResult> getHoursWorked(int studentId)
        {
            var overallShift = await _myDBContext.Shift.Where(s => s.StudentID == studentId && s.IsAuthorized).ToListAsync();

            //TimeOnly startTime, TimeOnly endTime, DateTime checkIn, DateTime checkOut, bool isOvertime)

            var studentShifts = await _myDBContext.Shift.Join(
                _myDBContext.Recruitment,
                s => s.RecruitmentID,
                r => r.RecruitmentID,
                (s, r) => new
                {
                    Shift = s,
                    Recruitment = r
                })
                .Where(sr => sr.Shift.StudentID == studentId & sr.Shift.IsAuthorized) // Add isCheck out //isApproved
                .ToListAsync();

            int tHours = 0;
            int tRating = 0;
            double tWage = 0;
            foreach (var item in studentShifts)
            {
                int duration = getDuration(item.Recruitment.StartTime, item.Recruitment.EndTime, item.Shift.CheckInTime, item.Shift.CheckOutTime, item.Shift.IsOvertime);
                tRating += item.Shift.Rating;
                tHours += duration;
                double wage = getWage(duration);
                tWage += wage;
            }
            double ratingPercent = (double)tRating / (studentShifts.Count * 5);
            double aveRating = ratingPercent * 5;
            double roundedRating = Math.Round(aveRating, 2);
            List<double> data = new List<double>();

            data.Add(tHours);
            data.Add(roundedRating);
            data.Add(tWage);
            return Ok(data);

        }





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
