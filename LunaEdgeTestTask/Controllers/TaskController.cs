using LunaEdgeTestTask.DTOs.Requests;
using LunaEdgeTestTask.DTOs.Responses;
using LunaEdgeTestTask.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunaEdgeTestTask.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskController(ITasksService tasksService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CreateTaskResponse>> CreateTask
            ([FromBody] CreateTaskRequest request)
        {
            try
            {
                // Get user ID from JWT claims
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }

                var result = await tasksService.CreateTaskAsync(request, Guid.Parse(userId));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<FilteredTasksResponse>>> GetTasks
            ([FromQuery] GetTasksFilterRequest request)
        {
            try
            {
                var result = await tasksService.GetTasksAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetTaskInfoResponse>> GetTaskById
            ([FromRoute] Guid id, [FromQuery] GetTaskInfoOrDeleteRequest request)
        {
            try
            {
                // Get user ID from JWT claims
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User not authorized.");
                }
                // Pass the route id to the request object if needed
                request.Id = id;
                var result = await tasksService.GetTaskInfoAsync(request, Guid.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UpdateTaskResponse>> UpdateTask
            ([FromRoute] Guid id, [FromBody] UpdateTaskRequest updateRequest)
        {
            try
            {
                // Get user ID from JWT claims
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User not authorized.");
                }
                // Pass the route id to the request object if needed
                updateRequest.TaskId = id;
                var result = await tasksService.UpdateTaskAsync(updateRequest, Guid.Parse(userId));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteTask
            ([FromRoute] Guid id, [FromBody] GetTaskInfoOrDeleteRequest request)
        {
            try
            {
                // Get user ID from JWT claims
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User not authorized.");
                }
                // Pass the route id to the request object if needed
                request.Id = id;
                var result = await tasksService.DeleteTaskAsync(request, Guid.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
