using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace DataAccess.Interface
{
    public interface IPassengerRepository
    {
        Task<Passenger> GetByIdAsync(int id);
        Task<Passenger> GetByPassportNumberAsync(string passportNumber);
        Task<IEnumerable<Passenger>> GetByFlightAsync(int flightId);
        Task<IEnumerable<Passenger>> GetAllAsync();
        Task<bool> HasReservationAsync(string passportNumber, int flightId);
        Task AddAsync(Passenger passenger);
        Task UpdateAsync(Passenger passenger);
    }
}
