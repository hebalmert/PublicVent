using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vent.AccessData.Data;
using Vent.Backend.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers.Entites;

[Route("api/softplans")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class SoftPlansController : ControllerBase
{
    private readonly DataContext _context;

    public SoftPlansController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("loadCombo")]
    public async Task<ActionResult<IEnumerable<SoftPlan>>> GetPeriodicidads()
    {
        var listResult = await _context.SoftPlans.OrderBy(x => x.Name).ToListAsync();
        return listResult;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SoftPlan>>> GetSoftPlans([FromQuery] PaginationDTO pagination)
    {
        var queryable = _context.SoftPlans.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SoftPlan>> GetSoftPlan(int id)
    {
        try
        {
            var modelo = await _context.SoftPlans
            .FindAsync(id);

            if (modelo == null)
            {
                return NotFound();
            }

            return Ok(modelo);
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

    [HttpPut]
    public async Task<IActionResult> PutSoftPlan(SoftPlan modelo)
    {
        try
        {
            _context.SoftPlans.Update(modelo);
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

    [HttpPost]
    public async Task<ActionResult<SoftPlan>> PostSoftPlan(SoftPlan modelo)
    {
        try
        {
            _context.SoftPlans.Add(modelo);
            await _context.SaveChangesAsync();

            return Ok(modelo);

            //return CreatedAtAction("GetSoftPlan", new { id = modelo.SoftPlanId }, modelo);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSoftPlan(int id)
    {
        try
        {
            var DataRemove = await _context.SoftPlans.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.SoftPlans.Remove(DataRemove);
            await _context.SaveChangesAsync();
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