namespace TankiForum.DTOs.Users;

public class UserStatsDto
{
    public int TopicCount { get; set; }
    public int PostCount { get; set; }
    public int RespectCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
