using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.Enum;

namespace DataAccess.Interface
{
    public interface IFlightRepository
    {
        Task<Flight> GetByIdAsync(int id);
        Task<Flight> GetByFlightNumberAsync(string flightNumber);
        Task<List<Flight>> GetAllAsync();
        Task<List<Flight>> GetFlightsByStatusAsync(FlightStatus status);
        Task UpdateStatusAsync(int id, FlightStatus status);
        Task UpdateAsync(Flight flight);
    }
}
