using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MobileController.Models;
using MobileController.Models.Data;

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
            var query = _myDBContext.Shift.Join(
                            _myDBContext.Recruitment,
                            s => s.RecruitmentID,
                            r => r.RecruitmentID,
                            (s, r) => new
                            {
                                Shift = s,
                                Recruitment = r
                            })
                            .Where(sr => sr.Shift.StudentID == stuID) //Today check in is still visible       //&& sr.Recruitment.JobShiftDate.Date >= currentDate
                            .OrderBy(sr => sr.Recruitment.JobShiftDate);


            var shifts = await query.ToListAsync();

            return shifts.Select(s => s.Recruitment).ToList();
        }
    }
}
