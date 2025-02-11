using System.ComponentModel.DataAnnotations;
using Vent.Shared.Entities;
using Vent.Shared.Enum;

namespace Vent.Shared.EntitiesSoftSec;

public class UsuarioRole
{
    [Key]
    public int UsuarioRoleId { get; set; }

    [Required(ErrorMessage = "El largo maximo es de {0}")]
    [Display(Name = "Usuario")]
    public int UsuarioId { get; set; }

    [Display(Name = "Tipo Usuario")]
    public UserType UserType { get; set; }

    //Relaciones
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public Usuario? Usuario { get; set; }
}