using System.ComponentModel.DataAnnotations;
using Vent.Shared.Enum;

namespace Vent.Shared.Entities;

public class UserRoleDetails
{
    [Key]
    public int UserRoleDetailsId { get; set; }

    [Display(Name = "Rol Usuario")]
    public UserType? UserType { get; set; }

    [Display(Name = "User Id")]
    public string? UserId { get; set; }

    //Relaciones
    public User? User { get; set; }
}