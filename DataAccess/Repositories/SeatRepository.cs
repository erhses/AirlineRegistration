using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Interface;
using Models.Entities;
using Models;

namespace DataAccess.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly FlightCheckInContext _context;

        public SeatRepository(FlightCheckInContext context)
        {
            _context = context;
        }

        public async Task<Seat> GetByIdAsync(int id)
        {
            return await _context.Seats
                .Include(s => s.Aircraft)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Seat>> GetByAircraftIdAsync(int aircraftId)
        {
            return await _context.Seats
                .Where(s => s.AircraftId == aircraftId)
                .ToListAsync();
        }
        public async Task<bool> UpdateAsync(Seat seat)
        {
            if (seat == null)
                throw new ArgumentNullException(nameof(seat));

            try
            {
                _context.Entry(seat).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency conflicts
                return false;
            }
            catch (Exception)
            {
                // Log other exceptions
                return false;
            }
        }
        public async Task<bool> UpdateSeatOccupancyAsync(Seat seat)
        {
            if (seat == null)
                return false;
            return await UpdateAsync(seat);
        }
        public async Task<IEnumerable<Seat>> GetAvailableSeatsForFlightAsync(int flightId)
        {
            var flight = await _context.Flights
                .Include(f => f.Aircraft)
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (flight == null)
                return new List<Seat>();

            // Тухайн нислэгт захиалагдсан суудлууд
            var bookedSeatIds = await _context.Bookings
                .Where(b => b.FlightId == flightId && b.SeatId.HasValue)
                .Select(b => b.SeatId.Value)
                .ToListAsync();

            // Онгоцны бүх суудлуудаас захиалагдаагүйг нь олох
            var availableSeats = await _context.Seats
                .Where(s => s.AircraftId == flight.AircraftId && !bookedSeatIds.Contains(s.Id))
                .ToListAsync();

            return availableSeats;
        }
        public async Task<bool> IsSeatAvailableAsync(int flightId, int seatId)
        {
            // Нислэгийн мэдээлэл
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null)
                return false;

            // Суудал тухайн онгоцны суудал мөн эсэх
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.Id == seatId && s.AircraftId == flight.AircraftId);

            if (seat == null)
                return false;

            // Суудал сул эсэх
            var isSeatBooked = await _context.Bookings
                .AnyAsync(b => b.FlightId == flightId && b.SeatId == seatId);

            return !isSeatBooked;
        }
    }

}
