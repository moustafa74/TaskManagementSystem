using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.DAL.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {

        public TeamRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<int?> GetTeamIdByUserIdAsync(string userId)
        {
            var teamMember = await _context.TeamMembers
                .Where(tm => tm.UserId == userId)
                .Select(tm => tm.TeamId)
                .FirstOrDefaultAsync();

            return teamMember;
        }
    }
}
