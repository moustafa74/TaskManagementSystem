using TaskManagementSystem.Models;

namespace TaskManagementSystem.DAL.Repositories
{
    public interface ITeamRepository:IGenericRepository<Team>
    {
        Task<int?> GetTeamIdByUserIdAsync(string userId);
    }
}
