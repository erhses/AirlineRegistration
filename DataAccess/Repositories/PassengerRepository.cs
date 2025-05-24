using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Interface;
using Models;
using Models.Entities;

namespace DataAccess.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly FlightCheckInContext _context;

        public PassengerRepository(FlightCheckInContext context)
        {
            _context = context;
        }

        public async Task<Passenger> GetByIdAsync(int id)
        {
            return await _context.Passengers
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Passenger> GetByPassportNumberAsync(string passportNumber)
        {
            return await _context.Passengers
                .FirstOrDefaultAsync(p => p.PassportNumber == passportNumber);
        }

        public async Task<IEnumerable<Passenger>> GetAllAsync()
        {
            return await _context.Passengers.ToListAsync();
        }

        public async Task AddAsync(Passenger passenger)
        {
            await _context.Passengers.AddAsync(passenger);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Passenger>> GetByFlightAsync(int flightId)
        {
            return await _context.Passengers
                .Include(p => p.Bookings) // Include bookings if you need them
                .Where(p => p.Bookings.Any(b => b.FlightId == flightId))
                .ToListAsync();
        }
        public async Task<bool> HasReservationAsync(string passportNumber, int flightId)
        {
            return await _context.Passengers
                .Include(p => p.Bookings)
                .AnyAsync(p => p.PassportNumber == passportNumber && p.Bookings.Any(b => b.FlightId == flightId));
        }

        public async Task UpdateAsync(Passenger passenger)
        {
            _context.Entry(passenger).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
