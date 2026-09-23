using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models;

public sealed record CreateUserRequest
{
    [Required, StringLength(50), NotWhiteSpace]
    public string FirstName { get; init; } = string.Empty;

    [Required, StringLength(50), NotWhiteSpace]
    public string LastName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(80), NotWhiteSpace]
    public string Department { get; init; } = string.Empty;
}
