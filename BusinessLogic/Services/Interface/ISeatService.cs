using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTO;

namespace BusinessLogic.Services.Interface
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDto>> GetSeatsByAircraftIdAsync(int aircraftId);
        Task<IEnumerable<SeatDto>> GetAvailableSeatsAsync(int flightId);
        Task<SeatAssignmentResult> AssignSeatAsync(int bookingId, string seatNumber);
        Task<SeatAssignmentResult> AssignSeatAsync(int flightId, string seatNumber, int passengerId);
        Task<bool> IsSeatAvailableAsync(int flightId, string seatNumber);
    }
}
