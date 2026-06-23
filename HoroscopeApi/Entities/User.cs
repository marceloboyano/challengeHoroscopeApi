namespace HoroscopeApi.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<HoroscopeQuery> Queries { get; set; } = new List<HoroscopeQuery>();
}
