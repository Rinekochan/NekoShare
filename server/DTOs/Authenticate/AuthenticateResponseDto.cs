namespace server.DTOs.Authenticate;

public class AuthenticateResponseDto
{
    public required string Username { get; set; }
    public required string Token { get; set; }
    public required string KnownAs { get; set; }
    public string? PhotoUrl { get; set; }
}