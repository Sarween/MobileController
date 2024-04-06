using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileController.Data;
using MobileController.Services;

namespace MobileController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmentController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public RecruitmentController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        // When student press check in
        [HttpGet("GetRecruitments/{staffid}")]
        public async Task<IActionResult> GetRecruitments(int staffid)
        {

            var recruitmentsForStaff = await _myDBContext.Recruitment
                                        .Where(r => r.StaffID == staffid)
                                        .ToListAsync();

            //var modifiedRecruitments = recruitmentsForStaff.Select(r => new
            //{
            //    Recruitment = r,
            //    //actionSelected = "original", // Default value for actionSelected
            //    selectedDuration = 0 // Default value for selectedDuration
            //});

            return Ok(recruitmentsForStaff);
        }

        [HttpGet("{RecruitmentID}")]
        public async Task<IActionResult> GetAssignmentsByRecruitmentID(int RecruitmentID)
        {
            var shifts = await _myDBContext.Shift
                .Where(a => a.RecruitmentID == RecruitmentID && a.IsCancelled == false)
                .Join(
                _myDBContext.Student,
                shift => shift.StudentID,
                student => student.StudentID,
                (shift, student) => new
                {
                    RecruitmentID = shift.RecruitmentID,
                    StudentID = shift.StudentID,
                    StudentName = student.StudentName,
                    CheckInTime = shift.CheckInTime,
                    CheckOutTime = shift.CheckOutTime,
                    Rating = shift.Rating,
                    StaffReview = shift.StaffReview,
                    IsOvertime = shift.IsOvertime,
                    IsAuthorized = shift.IsAuthorized,
                    actionSelected = "original",
                    selectedDuration = 0
                 
                })
                .ToListAsync();

            //var modifiedShifts = shifts.Select(s => new
            //{
            //    shifts = s,
            //    //actionSelected = "original", // Default value for actionSelected
            //    selectedDuration = 0 // Default value for selectedDuration
            //});

            return Ok(shifts);
        }

    }
}
