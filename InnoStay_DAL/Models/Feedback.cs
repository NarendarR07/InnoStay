using System;
using System.Collections.Generic;

namespace InnoStay_DAL.Models;

public partial class Feedback
{
    public int FeedBackId { get; set; }

    public int? UserId { get; set; }

    public int? RoomId { get; set; }

    public int? Rating { get; set; }

    public string? Comments { get; set; }

    public virtual Room? Room { get; set; }

    public virtual User? User { get; set; }
}
