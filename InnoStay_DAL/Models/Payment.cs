using System;
using System.Collections.Generic;

namespace InnoStay_DAL.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal Amouont { get; set; }

    public string? PaymentStatus { get; set; }

    public int? TransactionId { get; set; }

    public virtual Booking? Booking { get; set; }
}
