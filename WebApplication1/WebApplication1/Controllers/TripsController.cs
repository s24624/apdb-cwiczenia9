using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips(string? query, int? page, int? pageSize)
    {
        var result = await _tripsService.GetTrips(query, page, pageSize);
        return Ok(result);
    }

    [HttpDelete("/{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var result = await _tripsService.DeleteClient(id);
        return Ok(result);
    }
}