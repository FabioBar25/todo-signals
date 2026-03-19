using Microsoft.AspNetCore.Mvc;
using TodoSignals.Host.Contracts;
using TodoSignals.Manager.Interfaces;
using TodoSignals.Manager.Models;

namespace TodoSignals.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TasksController(ITaskManager taskManager) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetTasks(CancellationToken cancellationToken)
    {
        var tasks = await taskManager.GetTasksAsync(cancellationToken);
        return Ok(tasks.Select(Map).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await taskManager.CreateTaskAsync(request.Title, cancellationToken);
            var response = Map(task);
            return CreatedAtAction(nameof(GetTasks), new { id = response.Id }, response);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(detail: exception.Message);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> UpdateTask(
        int id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await taskManager.UpdateTaskAsync(id, request.Title, cancellationToken);

            if (task is null)
            {
                return NotFound();
            }

            return Ok(Map(task));
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(detail: exception.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
    {
        var deleted = await taskManager.DeleteTaskAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static TaskResponse Map(TaskItem task) =>
        new()
        {
            Id = task.Id,
            Title = task.Title
        };
}
