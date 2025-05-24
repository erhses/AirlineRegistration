using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTO;

namespace BusinessLogic.Services.Interface
{
    public interface IPassengerService
    {
        Task<PassengerDto> GetPassengerByPassportAsync(string passportNumber);
        Task<bool> CheckInPassengerAsync(int passengerId, int flightId, string seatNumber);
        Task<bool> HasReservationAsync(string passportNumber, int flightId);
    }
}
