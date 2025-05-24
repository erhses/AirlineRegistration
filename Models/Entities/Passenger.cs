using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Passenger
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassportNumber { get; set; }
        public string Nationality { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Navigation properties
        public List<Booking> Bookings { get; set; }
    }
}
