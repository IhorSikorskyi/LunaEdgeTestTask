using LunaEdgeTestTask.DTOs.Requests;
using LunaEdgeTestTask.DTOs.Responses;

namespace LunaEdgeTestTask.Services.Interfaces;

public interface ITasksService
{
    Task<CreateTaskResponse> CreateTaskAsync(CreateTaskRequest request, Guid userId);
    Task<List<FilteredTasksResponse>> GetTasksAsync(GetTasksFilterRequest request);
    Task<GetTaskInfoResponse> GetTaskInfoAsync(GetTaskInfoOrDeleteRequest request, Guid userId);
    Task<UpdateTaskResponse> UpdateTaskAsync(UpdateTaskRequest updateRequest, Guid userId);
    Task<bool> DeleteTaskAsync(GetTaskInfoOrDeleteRequest request, Guid userId);
}