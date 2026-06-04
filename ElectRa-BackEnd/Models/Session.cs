namespace ElectRa_BackEnd.Models;

public class Session
{
	public int Id { get; set; }

	public string SessionId { get; set; } = null!;

	public long UserId { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime ExpiresAt { get; set; }

	public DateTime? LastAccessedAt { get; set; }
}