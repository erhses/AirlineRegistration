using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enum;
using Models;
using DataAccess.Interface;

namespace DataAccess.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly FlightCheckInContext _context;

        public FlightRepository(FlightCheckInContext context)
        {
            _context = context;
        }

        public async Task<Flight> GetByIdAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Flight> GetByFlightNumberAsync(string flightNumber)
        {
            return await _context.Flights
                .Include(f => f.Aircraft)
                .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);
        }

        public async Task<List<Flight>> GetAllAsync()
        {
            return await _context.Flights
                .Include(f => f.Aircraft)
                .ToListAsync();
        }

        public async Task<List<Flight>> GetFlightsByStatusAsync(FlightStatus status)
        {
            return await _context.Flights
                .Where(f => f.Status == status)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int id, FlightStatus status)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                flight.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Flight flight)
        {
            _context.Entry(flight).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}