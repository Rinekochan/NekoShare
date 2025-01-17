namespace server.DTOs.Photo;

public record PhotoAprrovalRequestDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public bool IsApproved { get; set; }
};