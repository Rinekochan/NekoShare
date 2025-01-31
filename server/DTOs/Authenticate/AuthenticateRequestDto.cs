﻿using System.ComponentModel.DataAnnotations;

namespace server.DTOs.Authenticate;

public class AuthenticateRequestDto
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}