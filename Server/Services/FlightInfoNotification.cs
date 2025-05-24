using System.Threading.Tasks;
using BusinessLogic.DTO;
using BusinessLogic.Services.Interface;
using Microsoft.AspNetCore.SignalR;
using Models.Entities;
using Server.Hubs;

namespace Server.Services
{
    /// <summary>
    /// Implementation of IFlightInfoNotificationService that uses SignalR to broadcast updates
    /// </summary>
    public class FlightInfoNotificationService : IFlightInfoNotificationService
    {
        private readonly IHubContext<FlightInfoHub> _hubContext;

        public FlightInfoNotificationService(IHubContext<FlightInfoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySeatAssignmentChangeAsync(int flightId, string seatNumber, bool isAssigned)
        {
            await _hubContext.SendSeatAssignmentUpdateAsync(flightId, seatNumber, isAssigned);
        }

        public async Task NotifyFlightStatusChangeAsync(FlightDto flight)
        {
            await _hubContext.SendFlightStatusUpdateAsync(flight);
        }

        public async Task NotifyBoardingStatusChangeAsync(int flightId, int totalPassengers, int boardedPassengers)
        {
            await _hubContext.SendBoardingUpdateAsync(flightId, totalPassengers, boardedPassengers);
        }
    }
}