using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enum;

namespace BusinessLogic.DTO
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public FlightStatus Status { get; set; }
        public int AircraftId { get; set; }
        public string Gate { get; set; }
    }
}
