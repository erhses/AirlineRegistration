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
    public class BookingRepository : IBookingRepository
    {
        private readonly FlightCheckInContext _context;

        public BookingRepository(FlightCheckInContext context)
        {
            _context = context;
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Flight)
                .Include(b => b.Seat)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> GetByReferenceAsync(string reference)
        {
            return await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Flight)
                .Include(b => b.Seat)
                .FirstOrDefaultAsync(b => b.BookingReference == reference);
        }

        public async Task<IEnumerable<Booking>> GetByPassengerIdAsync(int passengerId)
        {
            return await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Seat)
                .Where(b => b.PassengerId == passengerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByFlightIdAsync(int flightId)
        {
            return await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Seat)
                .Where(b => b.FlightId == flightId)
                .ToListAsync();
        }

        public async Task<bool> AssignSeatAsync(int bookingId, int seatId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //check if booking exists
                    var booking = await _context.Bookings.FindAsync(bookingId);
                    if (booking == null)
                        return false;

                    //check if occupied
                    var seatIsAlreadyTaken = await _context.Bookings
                        .AnyAsync(b => b.FlightId == booking.FlightId &&
                                     b.SeatId == seatId &&
                                     b.Id != bookingId);

                    if (seatIsAlreadyTaken)
                        return false;

                    //assign seat
                    booking.SeatId = seatId;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task CheckInAsync(int bookingId, int? seatId = null)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.IsCheckedIn = true;
                booking.CheckInTime = DateTime.Now;

                if (seatId.HasValue)
                {
                    booking.SeatId = seatId.Value;
                }

                //boardingpass id
                booking.BoardingPassNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

                await _context.SaveChangesAsync();
            }
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }

}
