using System;
using System.Collections.Generic;

namespace Snake.DAL.Models;

public partial class Leaderboard
{
    public int ScoreId { get; set; }

    public int UserId { get; set; }

    public int Score { get; set; }

    public DateTime? AchievedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
