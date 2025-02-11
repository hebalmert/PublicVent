using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vent.AccessData.Data;
using Vent.Backend.Helpers;
using Vent.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoftSec;
using Vent.Shared.Enum;
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers.EntitiesSoftSec
{
    [Route("api/usuarioRoles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [ApiController]
    public class UsuariosRoleController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public UsuariosRoleController(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        [HttpGet("loadCombo")]
        public async Task<ActionResult<IEnumerable<EnumItemModel>>> GetPeriodicidads()
        {
            List<EnumItemModel> list = Enum.GetValues(typeof(UserTypeDTO)).Cast<UserTypeDTO>().Select(c => new EnumItemModel()
            {
                Name = c.ToString(),
                Value = (int)c
            }).ToList();

            list.Insert(0, new EnumItemModel
            {
                Name = "[Seleccione Tipo Usuario...]",
                Value = 0
            });

            return list;
        }

        // GET: api/Planillas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioRole>>> GetUsuarioRole([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.UsuarioRoles.Where(x => x.UsuarioId == pagination.Id)
                .Include(x => x.Usuario).AsQueryable();

            await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
            return await queryable.Paginate(pagination).ToListAsync();
        }

        // GET: api/Planillas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioRole>> GetUsuarioRole(int id)
        {
            try
            {
                var modelo = await _context.UsuarioRoles.FirstOrDefaultAsync(x => x.UsuarioRoleId == id);

                if (modelo == null)
                {
                    return NotFound();
                }

                return modelo;
            }
            catch (DbUpdateException dbUpdateException)
            {
                return BadRequest(dbUpdateException.InnerException!.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        // POST: api/Planillas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UsuarioRole>> PostUsuarioRole(UsuarioRole modelo)
        {
            try
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
                User userAsp = await _userHelper.GetUserAsync(email);
                if (userAsp == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

                var CurrentUser = await _context.Usuarios.FindAsync(modelo.UsuarioId);
                var UserSystem = await _userHelper.GetUserAsync(CurrentUser!.UserName);
                var transaction = await _context.Database.BeginTransactionAsync();

                modelo.CorporationId = Convert.ToInt32(userAsp.CorporationId);
                _context.UsuarioRoles.Add(modelo);
                await _context.SaveChangesAsync();

                UserRoleDetails newUserRoleDetail = new()
                {
                    UserId = UserSystem.Id,
                    UserType = modelo.UserType
                };
                _context.UserRoleDetails.Add(newUserRoleDetail);
                await _userHelper.AddUserToRoleAsync(userAsp, modelo.UserType.ToString());
                await _userHelper.AddUserClaims(modelo.UserType, userAsp.UserName!);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return CreatedAtAction("GetUsuarioRole", new { id = modelo.UsuarioRoleId }, modelo);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un Registro con el mismo nombre.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        // DELETE: api/Planillas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarioRole(int id)
        {
            try
            {
                var transaction = await _context.Database.BeginTransactionAsync();

                var DataRemove = await _context.UsuarioRoles.FindAsync(id);
                if (DataRemove == null)
                {
                    return NotFound();
                }
                _context.UsuarioRoles.Remove(DataRemove);
                await _context.SaveChangesAsync();

                var usuario = await _context.Usuarios.FindAsync(DataRemove.UsuarioId);
                var userAsp = await _userHelper.GetUserAsync(usuario!.UserName);
                var registro = await _context.UserRoleDetails.Where(c => c.UserId == userAsp.Id && c.UserType == DataRemove.UserType).FirstOrDefaultAsync();

                _context.UserRoleDetails.Remove(registro!);
                await _context.SaveChangesAsync();

                await _userHelper.RemoveUserToRoleAsync(userAsp, DataRemove.UserType.ToString());
                await _userHelper.RemoveUserClaims(DataRemove.UserType, userAsp.UserName!);

                await transaction.CommitAsync();
                return NoContent();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("REFERENCE"))
                {
                    return BadRequest("Existen Registros Relacionados y no se puede Eliminar");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}