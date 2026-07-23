using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Task = Infrastructure.Data.Entities.Task;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var query = context.Tasks.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()));
            }

            var tasks = await query.ToListAsync(cancellationToken);
            return Ok(tasks ?? []);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id, CancellationToken cancellationToken)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} does not exist." });
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] Task task, CancellationToken cancellationToken)
        {
            var newTask = new Task
            {
                Title = task.Title,
                CreatedDate = DateTime.Now,
                isCompleted = task.isCompleted
            };
            newTask.UpdatedDate = newTask.CreatedDate;

            await context.Tasks.AddAsync(newTask, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] Task task, CancellationToken cancellationToken)
        {
            var updatedTask = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            if (updatedTask is null)
            {
                return NotFound(new { message = $"Task with ID {id} does not exist." });
            }

            updatedTask.Title = task.Title;
            updatedTask.UpdatedDate = DateTime.Now;
            updatedTask.isCompleted = task.isCompleted;

            await context.SaveChangesAsync(cancellationToken);

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} does not exist." });
            }

            context.Tasks.Remove(task);
            await context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}