using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vent.AccessData.Data;
using Vent.Backend.Helpers;
using Vent.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers.Entites;

[Route("api/corporations")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class CorporationsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly string ImgRoute;

    public CorporationsController(DataContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
        ImgRoute = "wwwroot\\Images\\ImgCorporation";
    }

    [HttpGet("loadCombo")]
    public async Task<ActionResult<IEnumerable<Corporation>>> GetCorporations()
    {
        var listResult = await _context.Corporations.Where(x => x.Active).ToListAsync();
        return listResult;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Corporation>>> GetCorporations([FromQuery] PaginationDTO pagination)
    {
        var queryable = _context.Corporations.Include(x => x.SoftPlan).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Corporation>> GetCorporation(int id)
    {
        try
        {
            var modelo = await _context.Corporations
            .FindAsync(id);

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

    // PUT: api/Corporations/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut()]
    public async Task<IActionResult> PutCorporation(Corporation modelo)
    {
        try
        {
            if (!string.IsNullOrEmpty(modelo.ImgBase64))
            {
                string guid;
                if (modelo.ImagenId == null)
                {
                    guid = Guid.NewGuid().ToString() + ".jpg";
                }
                else
                {
                    guid = modelo.ImagenId;
                }
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.ImagenId = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Corporations.Update(modelo);
            await _context.SaveChangesAsync();
            return Ok();
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

    // POST: api/Corporations
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Corporation>> PostCorporation(Corporation modelo)
    {
        try
        {
            if (modelo.ImgBase64 is not null)
            {
                string guid = Guid.NewGuid().ToString() + ".jpg";
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.ImagenId = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Corporations.Add(modelo);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCorporation", new { id = modelo.CorporationId }, modelo);
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

    // DELETE: api/Corporations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCorporation(int id)
    {
        try
        {
            var DataRemove = await _context.Corporations.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.Corporations.Remove(DataRemove);
            await _context.SaveChangesAsync();

            if (DataRemove.ImagenId is not null)
            {
                var response = _fileStorage.DeleteImage(ImgRoute, DataRemove.ImagenId);
                if (!response)
                {
                    return BadRequest("Se Elimino el Registro pero sin la Imagen");
                }
            }
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