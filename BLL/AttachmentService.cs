using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.BLL
{
    public class AttachmentService
    {
        private readonly IGenericRepository<Attachment> _attachmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttachmentService(IGenericRepository<Attachment> attachmentRepository, UserManager<ApplicationUser> userManager)
        {
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;

        }
        //public async Task<bool> IsTeamLeadAuthorizedForTask(string teamLeadId, int taskId)
        //{
        //    var task = await _attachmentRepository.(taskId);
        //    if (task == null)
        //    {
        //        return false;
        //    }

        //    // تحقق من أن المستخدم هو TeamLead لفريق مرتبط بالمهمة
        //    var isTeamLead = task.TaskAssignments.Any(ta => ta.Team.TeamLeadId == teamLeadId);
        //    return isTeamLead;
        //}
        public async Task<IEnumerable<Attachment>> GetAttachmentsByTaskIdAsync(int taskId)
        {
            return await _attachmentRepository.GetAllAsync(attachment => attachment.TaskEntityId == taskId);
        }

        public async Task AddAttachmentAsync(Attachment attachment)
        {
            await _attachmentRepository.AddAsync(attachment);
            await _attachmentRepository.SaveAsync();
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment != null)
            {
                _attachmentRepository.Delete(attachment);
                await _attachmentRepository.SaveAsync();
            }
        }
    }
}
