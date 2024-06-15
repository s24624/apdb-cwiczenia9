namespace WebApplication1.DTOs;

public class PagesDto
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public List<TripDto> trips { get; set; } = new List<TripDto>();
}