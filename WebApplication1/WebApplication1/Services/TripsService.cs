using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1;

public class TripsService : ITripsService
{
    private Apdb9Context _context;

    public TripsService(Apdb9Context context)
    {
        _context = context;
    }

    public async Task<List<Trip>> GetTrips()
    {
        var trips = await _context.Trips.ToListAsync();
        return  trips;
    }
}