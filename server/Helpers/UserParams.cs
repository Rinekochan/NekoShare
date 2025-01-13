namespace server.Helpers;

public class UserParams
{
    public int PageNumber { get; set; } = 1;
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 1800;
    public string OrderBy { get; set; } = "lastActive";
    
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}