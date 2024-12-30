using System.ComponentModel.DataAnnotations;

namespace NekoShare.DTOs;

public class AuthenticateRequestDto
{
    [Required]
    public required string Username { get; set; }
   
    [Required]
    public required string Password { get; set; }
}