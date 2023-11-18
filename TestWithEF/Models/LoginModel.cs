#nullable enable
using System.ComponentModel.DataAnnotations;

namespace TestWithEF.Models;

public class LoginModel
{
    [Required(ErrorMessage = "User Name is required")]
    public string Username { get; init; }=default!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; init; }=default!;
}
