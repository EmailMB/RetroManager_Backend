using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class UserCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "A password tem de ter pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
