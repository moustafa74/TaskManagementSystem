using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.BLL
{
    public class CommentService
    {
        private readonly IGenericRepository<Comment> _commentRepository;

        public CommentService(IGenericRepository<Comment> commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(int taskId)
        {
            return await _commentRepository.GetAllAsync(
                comment => comment.TaskEntityId == taskId,
                query => query.Include(comment => comment.User) 
            );
        }
        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            return await _commentRepository.GetByIdAsync(commentId);
        }
        public async Task AddCommentAsync(Comment comment)
        {
            //UserManager<ApplicationUser> user = _commentRepository.GetByIdAsync(comment.UserId);
            //if (IsUserCommentOwnerAsync(user.id, comment.Id)) ;
            //{
            //    return "Access Denied";
            //}
            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveAsync();
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment != null)
            {
                await _commentRepository.DeleteAsync(comment);
                await _commentRepository.SaveAsync();
            }
        }
        public async Task<bool> IsUserCommentOwnerAsync(string userId, int commentId)
        {
            var comment = await GetCommentByIdAsync(commentId);
            return comment != null && comment.UserId == userId;
        }
    }
}
