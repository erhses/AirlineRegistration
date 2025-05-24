using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enum;

namespace Models.Entities
{
    public class Seat
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public SeatClass SeatClass { get; set; }
        public bool IsOccupied { get; set; }

        // Navigation properties
        public Aircraft Aircraft { get; set; }
        public int AircraftId { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
