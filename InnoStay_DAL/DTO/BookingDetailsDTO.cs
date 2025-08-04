using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoStay_DAL.DTO
{
    public class BookingDetailsDTO
    {
        public int BookingId { get; set; }
        public int? RoomNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal? Price { get; set; }
        public string CustomerName { get; set; }
        public string BookingStatus { get; set; }
    }
}
