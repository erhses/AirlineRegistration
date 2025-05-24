using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enum;

namespace Models.Entities
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Gate { get; set; }

        // Нислэгийн төлвүүд
        public FlightStatus Status { get; set; }

        // Navigation properties
        public Aircraft Aircraft { get; set; }
        public int AircraftId { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
