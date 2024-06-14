using WebApplication1.Models;

namespace WebApplication1;

public interface ITripsService
{
    public Task<List<Trip>> GetTrips();
}
