namespace LunaEdgeTestTask.DTOs.Requests;

public class RefreshTokenRequest
{
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}