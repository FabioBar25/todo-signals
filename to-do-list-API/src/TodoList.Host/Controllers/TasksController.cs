using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Manager.Contracts;
using TodoList.Manager.Managers;

namespace TodoList.Host.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class TasksController(ITaskManager taskManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
    {
        var tasks = await taskManager.GetTasksAsync(GetUserId(), cancellationToken);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] SaveTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await taskManager.CreateTaskAsync(GetUserId(), request, cancellationToken);
        return Created($"/api/tasks/{task.Id}", task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] SaveTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await taskManager.UpdateTaskAsync(GetUserId(), id, request, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
    {
        var wasDeleted = await taskManager.DeleteTaskAsync(GetUserId(), id, cancellationToken);
        return wasDeleted ? NoContent() : NotFound();
    }

    private int GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("The current user session is invalid.");
    }
}
