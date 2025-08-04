using System;
using System.Collections.Generic;

namespace InnoStay_DAL.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int? RoomNumber { get; set; }

    public string? RoomType { get; set; }

    public decimal? PricePerNight { get; set; }

    public int Capacity { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
