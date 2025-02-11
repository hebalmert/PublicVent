using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoftSec;

public class Usuario
{
    [Key]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    [Display(Name = "Nombres")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    [Display(Name = "Apellidos")]
    public string LastName { get; set; } = null!;

    [MaxLength(100, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [MaxLength(15, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "RUC ó DNI")]
    public string Nro_Document { get; set; } = null!;

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [MaxLength(25, ErrorMessage = "El {0} no puede tener mas de {1} Caracteres.")]
    [Display(Name = "Telefono")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "La {0} es Obligatoria")]
    [MaxLength(256, ErrorMessage = "El campo no puede ser mayor a {0} de largo")]
    [Display(Name = "Direccion")]
    public string? Address { get; set; }

    //Correo y Coirporation
    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [MaxLength(256, ErrorMessage = "El campo no puede ser mayor a {0} de largo")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [MaxLength(50, ErrorMessage = "El {0} no puede tener mas de {1} Caracteres.")]
    [Display(Name = "Puesto Trabajo")]
    public string? Job { get; set; }

    [Display(Name = "Foto")]
    public string? Photo { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //Propiedades Virtuales
    //TODO: Pending to put the correct paths
    [Display(Name = "Foto")]
    public string ImageFullPath => Photo == string.Empty || Photo == null
    ? $"https://localhost:7148/Images/NoImage.png"
    : $"https://localhost:7148/Images/ImgUsuarios/{Photo}";

    //? $"https://spi.nexxtplanet.net/Images/NoImage.png"
    //: $"https://spi.nexxtplanet.net/Images/ImgUsuarios/{Photo}";

    [NotMapped]
    public string? ImgBase64 { get; set; }

    //Relaciones

    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public ICollection<UsuarioRole>? UsuarioRoles { get; set; }
}