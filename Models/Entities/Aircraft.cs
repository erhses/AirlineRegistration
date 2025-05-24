using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Aircraft
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
        public int TotalSeats { get; set; }

        // Navigation properties
        public List<Seat> Seats { get; set; }
        public List<Flight> Flights { get; set; }
    }
}
