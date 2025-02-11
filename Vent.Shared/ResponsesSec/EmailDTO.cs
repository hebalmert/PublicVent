using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.ResponsesSec;

public class EmailDTO
{
    [Required(ErrorMessage = "El Campo de Email es Obligatorio")]
    [EmailAddress(ErrorMessage = "Debe ser un formato de correo valido")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
}