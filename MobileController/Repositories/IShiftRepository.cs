using MobileController.Models;

namespace MobileController.Repositories
{
    public interface IShiftRepository
    {
        Task<List<Recruitment>> GetShiftByStuID(int stuID);
    }
}
