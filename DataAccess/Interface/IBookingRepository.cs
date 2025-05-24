using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace DataAccess.Interface
{
    public interface IBookingRepository
    {
        Task<Booking> GetByIdAsync(int id);
        Task<Booking> GetByReferenceAsync(string reference);
        Task<IEnumerable<Booking>> GetByPassengerIdAsync(int passengerId);
        Task<IEnumerable<Booking>> GetByFlightIdAsync(int flightId);
        Task<bool> AssignSeatAsync(int bookingId, int seatId);
        Task CheckInAsync(int bookingId, int? seatId = null);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
    }
}
