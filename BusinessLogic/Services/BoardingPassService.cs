using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTO;
using DataAccess.Interface;
using BusinessLogic.Services.Interface;
using Models.Entities;

namespace BusinessLogic.Services
{
    public class BoardingPassService : IBoardingPassService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly ISeatRepository _seatRepository;

        public BoardingPassService(
            IBookingRepository bookingRepository,
            IPassengerRepository passengerRepository,
            IFlightRepository flightRepository,
            ISeatRepository seatRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _passengerRepository = passengerRepository ?? throw new ArgumentNullException(nameof(passengerRepository));
            _flightRepository = flightRepository ?? throw new ArgumentNullException(nameof(flightRepository));
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
        }

        public async Task<BoardingPass> GenerateBoardingPassAsync(int passengerId, int flightId, string seatNumber)
        {
            // check if passenger exists  
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                throw new ArgumentException($"Passenger with ID {passengerId} not found");

            // check if flight exists  
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new ArgumentException($"Flight with ID {flightId} not found");

            // find seat  
            var seats = await _seatRepository.GetByAircraftIdAsync(flight.AircraftId);
            var seat = seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
            if (seat == null)
                throw new ArgumentException($"Seat {seatNumber} not found for the aircraft");

            // find existing booking or create a new one  
            var bookings = await _bookingRepository.GetByFlightIdAsync(flightId);
            var booking = bookings.FirstOrDefault(b => b.PassengerId == passengerId);

            if (booking == null)
            {
                booking = new Booking
                {
                    BookingReference = GenerateBookingReference(passenger.PassportNumber, flight.FlightNumber),
                    BookingDate = DateTime.UtcNow,
                    PassengerId = passengerId,
                    Passenger = passenger,
                    FlightId = flightId,
                    Flight = flight,
                    SeatId = seat.Id,
                    Seat = seat
                };

                await _bookingRepository.AddAsync(booking);
            }
            else
            {
                // update existing booking with seat information  
                booking.SeatId = seat.Id;
                booking.Seat = seat;
                await _bookingRepository.UpdateAsync(booking);
            }

            // boarding pass  
            booking.GenerateBoardingPass();
            await _bookingRepository.UpdateAsync(booking);

            // Create and return boarding pass  
            return new BoardingPass
            {
                Id = booking.Id,
                BoardingPassNumber = booking.BoardingPassNumber,
                PassengerName = $"{passenger.FirstName} {passenger.LastName}",
                PassportNumber = passenger.PassportNumber,
                FlightNumber = flight.FlightNumber,
                DepartureTime = flight.DepartureTime,
                Origin = flight.DepartureCity,
                Destination = flight.ArrivalCity,
                SeatNumber = seat.SeatNumber,
                Gate = booking.Gate,
                BoardingTime = booking.BoardingTime,
                Barcode = booking.Barcode,
                IssuedAt = booking.BoardingPassIssuedAt ?? DateTime.UtcNow
            };
        }

        public async Task<BoardingPass> GetBoardingPassAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || !booking.IsCheckedIn || string.IsNullOrEmpty(booking.BoardingPassNumber))
                return null;

            return new BoardingPass
            {
                Id = booking.Id,
                BoardingPassNumber = booking.BoardingPassNumber,
                PassengerName = $"{booking.Passenger.FirstName} {booking.Passenger.LastName}",
                PassportNumber = booking.Passenger.PassportNumber,
                FlightNumber = booking.Flight.FlightNumber,
                DepartureTime = booking.Flight.DepartureTime,
                Origin = booking.Flight.DepartureCity,
                Destination = booking.Flight.ArrivalCity,
                SeatNumber = booking.Seat?.SeatNumber,
                Gate = booking.Gate,
                BoardingTime = booking.BoardingTime,
                Barcode = booking.Barcode,
                IssuedAt = booking.BoardingPassIssuedAt ?? DateTime.UtcNow
            };
        }

        public async Task<bool> VerifyBoardingPassAsync(string boardingPassNumber)
        {
            if (string.IsNullOrEmpty(boardingPassNumber))
                return false;

            var bookings = await _bookingRepository.GetByFlightIdAsync(0); 
            var booking = bookings.FirstOrDefault(b => b.BoardingPassNumber == boardingPassNumber);

            if (booking == null)
                return false;

            return booking.IsBoardingPassValid();
        }

        private string GenerateBookingReference(string passportNumber, string flightNumber)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
