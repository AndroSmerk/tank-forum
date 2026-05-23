using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Clans;

/// <summary>
/// Request payload for joining or leaving a clan.
/// </summary>
public class JoinClanRequest
{
    /// <summary>
    /// Identifier of the clan to join or leave.
    /// </summary>
    [Required]
    public int ClanId { get; set; }
}
