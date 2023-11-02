using Microsoft.AspNetCore.Mvc;
using MobileController.Models;

namespace MobileController.Services
{
    public interface IShiftService
    {
        Task<IActionResult> GetFutureShiftByStuID(int stuID);
    }
}
