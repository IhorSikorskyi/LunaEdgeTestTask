namespace LunaEdgeTestTask.DTOs.Requests;

public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Email{ get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}

public class LoginRequest
{
    public required string UsernameOrEmail { get; set; }
    public required string Password { get; set; }
}