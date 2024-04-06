using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MobileController.Data;
using MobileController.Models;

namespace MobileController.Repositories
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly MyDBContext _myDBContext;

        public ShiftRepository(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        public async Task<List<Recruitment>> GetShiftByStuID(int stuID)
        {
            var dateTime = DateTime.UtcNow.AddHours(8).Date;
            var currentDate = DateOnly.FromDateTime(dateTime);

            DateTime checkOut = new DateTime(2000, 1, 1, 0, 0, 0);

            var query = _myDBContext.Shift.Join(
                            _myDBContext.Recruitment,
                            s => s.RecruitmentID,
                            r => r.RecruitmentID,
                            (s, r) => new
                            {
                                Shift = s,
                                Recruitment = r
                            })                                  
                            .Where(sr => sr.Shift.StudentID == stuID && // Check if The student has not checked out
                            sr.Recruitment.JobShiftDate >= currentDate) //Today check in is still visible       //sr.Shift.CheckOutTime == checkOut
                            .Select(sr => sr.Recruitment)
                            .OrderBy(sr => sr.JobShiftDate);


            var shifts = await query.ToListAsync();

            //return shifts.Select(s => s.Recruitment).ToList();
            return shifts;
        }
    }
}
