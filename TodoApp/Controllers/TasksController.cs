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
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await context.Tasks.ToListAsync();
            return Ok(tasks ?? []);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} does not exist." });
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] Task task)
        {
            var newTask = new Task
            {
                Title = task.Title,
                CreatedDate = DateTime.Now,
                isCompleted = task.isCompleted
            };
            newTask.UpdatedDate = newTask.CreatedDate;

            await context.Tasks.AddAsync(newTask);
            await context.SaveChangesAsync();

            return Ok(newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] Task task)
        {
            var updatedTask = await context.Tasks.FindAsync(id);
            if (updatedTask is null)
            {
                return NotFound(new { message = $"Task with ID {id} does not exist." });
            }

            updatedTask.Title = task.Title;
            updatedTask.UpdatedDate = DateTime.Now;
            updatedTask.isCompleted = task.isCompleted;

            await context.SaveChangesAsync();

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} does not exist." });
            }

            context.Tasks.Remove(task);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}