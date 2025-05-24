using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTO;
using BusinessLogic.Services.Interface;
using DataAccess.Interface;

namespace BusinessLogic.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IPassengerRepository _passengerRepository;
        private readonly ISeatService _seatService;
        private readonly IBoardingPassService _boardingPassService;

        public PassengerService(
            IPassengerRepository passengerRepository,
            ISeatService seatService,
            IBoardingPassService boardingPassService)
        {
            _passengerRepository = passengerRepository;
            _seatService = seatService;
            _boardingPassService = boardingPassService;
        }
        public async Task<bool> HasReservationAsync(string passportNumber, int flightId)
        {
            return await _passengerRepository.HasReservationAsync(passportNumber, flightId);
        }
        public async Task<PassengerDto> GetPassengerByPassportAsync(string passportNumber)
        {
            var passenger = await _passengerRepository.GetByPassportNumberAsync(passportNumber);
            if (passenger == null) return null;

            return new PassengerDto
            {
                Id = passenger.Id,
                FirstName = passenger.FirstName,
                LastName = passenger.LastName,
                PassportNumber = passenger.PassportNumber,
                Nationality = passenger.Nationality
            };
        }

        public async Task<bool> CheckInPassengerAsync(int passengerId, int flightId, string seatNumber)
        {
            // check if seat is available
            var isSeatAvailable = await _seatService.IsSeatAvailableAsync(flightId, seatNumber);
            if (!isSeatAvailable) return false;

            // assign seat
            var seatAssigned = await _seatService.AssignSeatAsync(flightId, seatNumber, passengerId);
            if (!seatAssigned.Success) return false;

            // generate boarding pass
            await _boardingPassService.GenerateBoardingPassAsync(passengerId, flightId, seatNumber);

            return true;
        }
    }
}
