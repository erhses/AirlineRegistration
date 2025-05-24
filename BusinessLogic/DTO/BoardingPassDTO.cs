using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTO
{
    public class BoardingPassDto
    {
        public int Id { get; set; }
        public string BoardingPassNumber { get; set; }
        public string PassengerName { get; set; }
        public string PassportNumber { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string SeatNumber { get; set; }
        public string Gate { get; set; }
        public DateTime? BoardingTime { get; set; }
        public string Barcode { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}
