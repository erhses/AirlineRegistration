using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;  


namespace DataAccess.Interface
{
    public interface IBoardingPassRepository
    {
        /// <summary>
        /// Generates a boarding pass for a passenger on a specific flight with assigned seat
        /// </summary>
        /// <param name="passengerId">ID of the passenger</param>
        /// <param name="flightId">ID of the flight</param>
        /// <param name="seatNumber">Seat number assigned to the passenger</param>
        /// <returns>A boarding pass DTO with all required information</returns>
        Task<BoardingPass> GenerateBoardingPassAsync(int passengerId, int flightId, string seatNumber);

        /// <summary>
        /// Gets boarding pass information for a specific booking
        /// </summary>
        /// <param name="bookingId">ID of the booking</param>
        /// <returns>Boarding pass DTO if available, null otherwise</returns>
        Task<BoardingPass> GetBoardingPassAsync(int bookingId);

        /// <summary>
        /// Verifies if a boarding pass exists and is valid
        /// </summary>
        /// <param name="boardingPassNumber">Boarding pass number to verify</param>
        /// <returns>True if valid, false otherwise</returns>
        Task<bool> VerifyBoardingPassAsync(string boardingPassNumber);
    }
}
