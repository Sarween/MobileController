using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileController.Data;
using MobileController.Models;
using MobileController.Repositories;
using NuGet.Protocol.Core.Types;
using System.Numerics;

namespace MobileController.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _repository;
        private readonly MyDBContext _myDBContext;

        public ShiftService(IShiftRepository repository, MyDBContext myDBContext)
        {
            _repository = repository;
            _myDBContext = myDBContext;
        }

        public async Task<IActionResult> GetFutureShiftByStuID(int stuID)
        {
            var eachStudentShiftsRecord =  await _repository.GetShiftByStuID(stuID);

            List<Recruitment> futureShifts = new List<Recruitment>();

            var dateTime = DateTime.UtcNow.AddHours(8).Date;
            var currentDate = DateOnly.FromDateTime(dateTime);

            foreach (var shift in eachStudentShiftsRecord)
            {
                if (shift.JobShiftDate >= currentDate)
                {
                    futureShifts.Add(shift);
                }
            }

            if (futureShifts.Count > 0)
            {

                return new OkObjectResult(futureShifts);
            }
            else
            {
                return new NoContentResult();
            }

        }

    }

}
