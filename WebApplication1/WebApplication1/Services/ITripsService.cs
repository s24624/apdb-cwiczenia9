using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1;

public interface ITripsService
{
    public Task<PagesDto> GetTrips(string? query,int? page,int? pageSize);
}
