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
    public async Task<IActionResult> GetTrips()
    {
        var trips = await _tripsService.GetTrips();
        return Ok(trips);
    }
}