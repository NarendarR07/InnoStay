using System;
using System.Collections.Generic;

namespace InnoStay_DAL.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? SendAt { get; set; }

    public virtual User? User { get; set; }
}
