namespace HoroscopeApi.Entities;

public class HoroscopeQuery
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Sign { get; set; } = null!;

    public DateOnly QueryDate { get; set; }

    public string ResultText { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
