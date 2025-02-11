using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.ResponsesSec;

public class ChangePasswordDTO
{
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Este Campo es obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave debe tener un minimo de {2} y un maximo de {1}")]
    [Display(Name = "Clave Actual")]
    public string CurrentPassword { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Este Campo es obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave debe tener un minimo de {2} y un maximo de {1}")]
    [Display(Name = "Nueva Clave")]
    public string NewPassword { get; set; } = null!;

    [Compare("NewPassword", ErrorMessage = "La Nueva Clave y la Confirmacion no coinciden")]
    [Required(ErrorMessage = "Este Campo es obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave debe tener un minimo de {2} y un maximo de {1}")]
    [Display(Name = "Confirmar Clave")]
    public string Confirm { get; set; } = null!;
}