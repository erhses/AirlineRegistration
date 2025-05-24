using BusinessLogic.DTO;
using BusinessLogic.Services.Interface;
using DataAccess.Interface;
using Models.Entities;
using Models.Enum;

namespace BusinessLogic.Services
{
    
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;

        public FlightService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        public async Task<FlightDto> GetFlightByIdAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            return MapToDto(flight);
        }

        public async Task<FlightDto> GetFlightByNumberAsync(string flightNumber)
        {
            var flight = await _flightRepository.GetByFlightNumberAsync(flightNumber);
            return MapToDto(flight);
        }

        public async Task<IEnumerable<FlightDto>> GetAllFlightsAsync()
        {
            var flights = await _flightRepository.GetAllAsync();
            return flights.Select(MapToDto).Where(f => f != null);
        }

        public async Task<IEnumerable<FlightDto>> GetFlightsByStatusAsync(FlightStatus status)
        {
            var flights = await _flightRepository.GetFlightsByStatusAsync(status);
            return flights.Select(MapToDto).Where(f => f != null);
        }

        public async Task<bool> UpdateFlightStatusAsync(int flightId, FlightStatus status)
        {
            try
            {
                await _flightRepository.UpdateStatusAsync(flightId, status);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private FlightDto MapToDto(Flight flight)
        {
            if (flight == null) return null;

            return new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                DepartureTime = flight.DepartureTime,
                Origin = flight.DepartureCity,
                Destination = flight.ArrivalCity,
                Status = flight.Status,
                AircraftId = flight.AircraftId,
                Gate = flight.Gate ?? "TBD"
            };
        }
    }
}