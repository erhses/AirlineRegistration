using System.Threading.Tasks;
using BusinessLogic.DTO;
using Models.Entities;

namespace BusinessLogic.Services.Interface
{
    /// <summary>
    /// Interface for notification services that broadcast flight-related updates
    /// </summary>
    public interface IFlightInfoNotificationService
    {
        /// <summary>
        /// Notify clients about seat assignment changes
        /// </summary>
        /// <param name="flightId">The ID of the flight</param>
        /// <param name="seatNumber">The seat number that was assigned or unassigned</param>
        /// <param name="isAssigned">Whether the seat was assigned (true) or unassigned (false)</param>
        Task NotifySeatAssignmentChangeAsync(int flightId, string seatNumber, bool isAssigned);

        /// <summary>
        /// Notify clients about flight status changes
        /// </summary>
        /// <param name="flight">The flight with updated status</param>
        Task NotifyFlightStatusChangeAsync(FlightDto flight);

        /// <summary>
        /// Notify clients about boarding status changes
        /// </summary>
        /// <param name="flightId">The ID of the flight</param>
        /// <param name="totalPassengers">Total number of passengers</param>
        /// <param name="boardedPassengers">Number of passengers who have boarded</param>
        Task NotifyBoardingStatusChangeAsync(int flightId, int totalPassengers, int boardedPassengers);
    }
}