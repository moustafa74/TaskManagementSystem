using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BLL;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentController : ControllerBase
    {
        private readonly AttachmentService _attachmentService;

        public AttachmentController(AttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetAttachmentsByTaskId(int taskId)
        {
            var attachments = await _attachmentService.GetAttachmentsByTaskIdAsync(taskId);
            return Ok(attachments);
        }

        [HttpPost]
        public async Task<IActionResult> AddAttachment([FromBody] Attachment attachment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _attachmentService.AddAttachmentAsync(attachment);
            return CreatedAtAction(nameof(GetAttachmentsByTaskId), new { taskId = attachment.TaskEntityId }, attachment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            await _attachmentService.DeleteAttachmentAsync(id);
            return NoContent();
        }
    }
}
