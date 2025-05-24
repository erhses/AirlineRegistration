using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enum;

namespace BusinessLogic.DTO
{
    public class SeatDto
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public int AircraftId { get; set; }
        public bool IsOccupied { get; set; }
        public SeatClass SeatClass { get; set; }
    }
}
