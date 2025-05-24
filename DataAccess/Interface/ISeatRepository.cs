using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace DataAccess.Interface
{
    public interface ISeatRepository
    {
        Task<Seat> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetByAircraftIdAsync(int aircraftId);
        Task<IEnumerable<Seat>> GetAvailableSeatsForFlightAsync(int flightId);
        Task<bool> IsSeatAvailableAsync(int flightId, int seatId);
        Task<bool> UpdateAsync(Seat seat);
        Task<bool> UpdateSeatOccupancyAsync(Seat seat);
    }

}
