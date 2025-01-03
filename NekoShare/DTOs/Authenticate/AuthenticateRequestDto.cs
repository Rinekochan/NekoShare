using System.ComponentModel.DataAnnotations;

namespace NekoShare.DTOs;

public class AuthenticateRequestDto
{
    [Required] public string Username { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;
}