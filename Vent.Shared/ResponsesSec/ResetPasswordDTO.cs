using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.ResponsesSec;

public class ResetPasswordDTO
{
    [Required()]
    [EmailAddress(ErrorMessage = "El {0} no es un Correo Valido")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave estar entre {1} y {2} caracteres")]
    [Display(Name = "Nueva Clave")]
    public string NewPassword { get; set; } = null!;

    [Compare("NewPassword", ErrorMessage = "No coinciden las Claves, verifique")]
    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave estar entre {1} y {2} caracteres")]
    [Display(Name = "Confirmar Clave")]
    public string ConfirmPassword { get; set; } = null!;

    public string Token { get; set; } = null!;
}