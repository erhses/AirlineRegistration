using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DTO;
using BusinessLogic.Services.Interface;
using DataAccess.Interface;
using Models.Entities;
using Models.Enum;

namespace BusinessLogic.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IFlightInfoNotificationService _notificationService;
        private static readonly object _concurrencyLock = new object();

        public SeatService(
            ISeatRepository seatRepository,
            IFlightRepository flightRepository,
            IBookingRepository bookingRepository,
            IFlightInfoNotificationService notificationService)
        {
            _seatRepository = seatRepository;
            _flightRepository = flightRepository;
            _bookingRepository = bookingRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsByAircraftIdAsync(int aircraftId)
        {
            var seats = await _seatRepository.GetByAircraftIdAsync(aircraftId);
            return seats.Select(s => new SeatDto
            {
                Id = s.Id,
                SeatNumber = s.SeatNumber,
                AircraftId = s.AircraftId,
                IsOccupied = s.IsOccupied,
                SeatClass = s.SeatClass
            });
        }

        public async Task<IEnumerable<SeatDto>> GetAvailableSeatsAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null) return Enumerable.Empty<SeatDto>();

            var seats = await _seatRepository.GetByAircraftIdAsync(flight.AircraftId);
            var bookedSeats = flight.Bookings
                .Where(b => b.SeatId.HasValue)
                .Select(b => b.Seat.SeatNumber);

            return seats
                .Where(s => !bookedSeats.Contains(s.SeatNumber))
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    SeatNumber = s.SeatNumber,
                    AircraftId = s.AircraftId,
                    IsOccupied = false,
                    SeatClass = s.SeatClass
                });
        }

        public async Task<SeatAssignmentResult> AssignSeatAsync(int bookingId, string seatNumber)
        {
            // lock to ensure only one thread execute this section at a time
            lock (_concurrencyLock)
            {
                try
                {
                    var booking = _bookingRepository.GetByIdAsync(bookingId).GetAwaiter().GetResult();
                    if (booking == null)
                        return new SeatAssignmentResult(false, "Booking not found");

                    if (booking.IsCheckedIn)
                        return new SeatAssignmentResult(false, "Passenger already checked in");

                    var flight = _flightRepository.GetByIdAsync(booking.FlightId).GetAwaiter().GetResult();
                    if (flight == null)
                        return new SeatAssignmentResult(false, "Flight not found");

                    if (flight.Status == FlightStatus.NotStarted)
                        return new SeatAssignmentResult(false, "Flight not in checking-in status");

                    var seat = _seatRepository.GetByAircraftIdAsync(flight.AircraftId)
                        .GetAwaiter().GetResult()
                        .FirstOrDefault(s => s.SeatNumber == seatNumber);

                    if (seat == null)
                        return new SeatAssignmentResult(false, "Seat not found");

                    // check if the seat is occupied
                    if (flight.Bookings.Any(b => b.Seat != null && b.Seat.SeatNumber == seatNumber))
                        return new SeatAssignmentResult(false, "Seat already occupied");

                    booking.Seat = seat;
                    booking.SeatId = seat.Id;
                    booking.IsCheckedIn = true; // checked in
                    seat.IsOccupied = true; // occupied
                    booking.GenerateBoardingPass(); // boarding pass logic

                    _bookingRepository.UpdateAsync(booking).GetAwaiter().GetResult();

                    // notify all clients about the seat assignment
                    Task.Run(async () =>
                    {
                        await _notificationService.NotifySeatAssignmentChangeAsync(flight.Id, seatNumber, true);

                        // update boarding status
                        int totalPassengers = flight.Bookings.Count;
                        int boardedPassengers = flight.Bookings.Count(b => b.IsCheckedIn);
                        await _notificationService.NotifyBoardingStatusChangeAsync(flight.Id, totalPassengers, boardedPassengers);
                    });

                    return new SeatAssignmentResult(true, "Seat assigned and boarding pass generated");
                }
                catch (Exception ex)
                {
                    return new SeatAssignmentResult(false, $"Error assigning seat: {ex.Message}");
                }
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seatNumber)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null) return false;

            return !flight.Bookings.Any(b => b.Seat != null && b.Seat.SeatNumber == seatNumber);
        }

        public async Task<SeatAssignmentResult> AssignSeatAsync(int flightId, string seatNumber, int passengerId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                return new SeatAssignmentResult(false, "Flight not found");

            var booking = flight.Bookings.FirstOrDefault(b => b.PassengerId == passengerId);
            if (booking == null)
                return new SeatAssignmentResult(false, "No booking found for this passenger on this flight");

            return await AssignSeatAsync(booking.Id, seatNumber);
        }
    }


    public class SeatAssignmentResult
    {
        public bool Success { get; }
        public string Message { get; }

        public SeatAssignmentResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}