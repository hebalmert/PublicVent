using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.ResponsesSec;

public class LoginDTO
{
    [Display(Name = "Usuario")]
    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [EmailAddress(ErrorMessage = "El Usuario es un Formato de Correo Electronico")]
    public string Email { get; set; } = null!;

    [Display(Name = "Clave")]
    [Required(ErrorMessage = "La {0} es Obligatori1")]
    [MinLength(6, ErrorMessage = "La Clave debe ser Mayor de 6 Caracteres")]
    public string Password { get; set; } = null!;
}