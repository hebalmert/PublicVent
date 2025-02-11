using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vent.AccessData.Data;
using Vent.Shared.Entities;

namespace Vent.Backend.Controllers.Entites;

[Route("api/countries")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly DataContext _context;

    public CountriesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("loadCombo")]
    public async Task<ActionResult<IEnumerable<Country>>> GetPeriodicidads()
    {
        var listResult = await _context.Countries.OrderBy(x => x.Name).ToListAsync();
        return listResult;
    }

    [HttpGet]
    public async Task<ActionResult> GetCountries()
    {
        var listResult = await _context.Countries.Include(x => x.States)!.ThenInclude(x => x.Cities).ToListAsync();
        return Ok(listResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCorporation(int id)
    {
        try
        {
            var modelo = await _context.Countries
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
}