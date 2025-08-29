using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using LunaEdgeTestTask.DTOs.Requests;
using LunaEdgeTestTask.DTOs.Responses;
using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using LunaEdgeTestTask.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace LunaEdgeTestTask.Services.Implementations;

public class AuthService(IUserRepository userRepository, IConfiguration configuration)
    : IAuthService
{
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if user with the same username or email already exists
        var existingUser = await userRepository.GetByUsernameOrEmailAsync(request.Username)
                           ?? await userRepository.GetByUsernameOrEmailAsync(request.Email);
        if (existingUser != null)
            throw new ArgumentException("User with the same username or email already exists");

        // Check password complexity and confirmation
        if (request.Password != request.ConfirmPassword)
            throw new ArgumentException("Passwords do not match");

        // if password is not complex enough ( at least 8 characters long, contain at least one uppercase letter,
        if (!IsPasswordComplex(request.Password))
            throw new ArgumentException(
                "Password must be at least 8 characters long, " +
                "contain at least one uppercase letter, one lowercase letter, " +
                "one number and one special character.");

        // Create new user instance
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = String.Empty,
            RefreshToken = String.Empty
        };

        // Hash password and generate refresh token
        user.PasswordHash = HashPassword(user, request.Password);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        // Save user to the database
        await userRepository.AddAsync(user);

        // Return JWT token
        return CreateTokenResponse(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        // Find user by username or email
        var user = await userRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail);
        if (user == null)
            throw new ArgumentException("Invalid User or Password");

        // Check password verification
        var passwordVerificationResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);

        // If password is incorrect, throw an exception
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new ArgumentException("Invalid User or Password");

        return CreateTokenResponse(user);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Validate the refresh token
        var user = await ValidateRefreshTokenAsync(request.UserId, request.AccessToken);
        if (user == null)
            throw new ArgumentException("Invalid Refresh Token");

        // Generate a new refresh token and update the user
        return CreateTokenResponse(user);
    }

    private AuthResponse CreateTokenResponse(User user)
    {
        // Generate new refresh token and update user's refresh token and expiry time
        return new AuthResponse
        {
            AccessToken = CreateToken(user)
        };
    }

    private string CreateToken(User user)
    {
        // Define claims for the JWT token
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.Username),
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Email, user.Email)
        };

        // Create signing credentials using the secret key from configuration
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:AccessToken")!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the JWT token with issuer, audience, claims, expiry, and signing credentials
        var accessToken = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        // Return the serialized JWT token
        return new JwtSecurityTokenHandler().WriteToken(accessToken);
    }

    private static string GenerateRefreshToken()
    {
        // Generate a secure random refresh token
        var randNumber = new byte[32];

        // Create a new instance of RandomNumberGenerator
        using var rng = RandomNumberGenerator.Create();

        // Fill the array with cryptographically strong random bytes
        rng.GetBytes(randNumber);

        // Convert the byte array to a Base64 string and return it
        return Convert.ToBase64String(randNumber);
    }

    private static string HashPassword(User user, string password)
    {
        // Hash the password using PasswordHasher
        return new PasswordHasher<User>().HashPassword(user, password);
    }

    private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        // Retrieve the user by ID and validate the refresh token
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;

        return user;
    }

    private static bool IsPasswordComplex(string password)
    {
        // Check if the password meets complexity requirements
        if (password.Length < 8) return false;

        // At least one uppercase letter, one lowercase letter, one digit, and one special character
        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

        // Iterate through each character in the password to check for required character types
        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
        }

        // Return true if all complexity requirements are met
        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
