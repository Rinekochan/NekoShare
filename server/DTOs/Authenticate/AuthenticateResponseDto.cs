namespace server.DTOs;

public class AuthenticateResponseDto
{
    public required string Username { get; set; }

    public required string Token { get; set; }
}