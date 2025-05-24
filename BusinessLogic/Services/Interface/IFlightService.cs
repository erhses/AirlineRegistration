using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTO;
using Models.Enum;

namespace BusinessLogic.Services.Interface
{
    public interface IFlightService
    {
        Task<FlightDto> GetFlightByIdAsync(int id);
        Task<FlightDto> GetFlightByNumberAsync(string flightNumber);
        Task<IEnumerable<FlightDto>> GetAllFlightsAsync();
        Task<IEnumerable<FlightDto>> GetFlightsByStatusAsync(FlightStatus status);
        Task<bool> UpdateFlightStatusAsync(int flightId, FlightStatus status);
    }
}
