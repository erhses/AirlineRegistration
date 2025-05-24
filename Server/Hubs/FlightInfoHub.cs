using Microsoft.AspNetCore.SignalR;
using Models.Entities;
using BusinessLogic.Services.Interface;
using BusinessLogic.DTO;

namespace Server.Hubs
{
    public class FlightInfoHub : Hub
    {
        private readonly IFlightService _flightService;

        public FlightInfoHub(IFlightService flightService)
        {
            _flightService = flightService;
        }

        /// <summary>
        /// clients connect to this hub to receive real-time flight updates
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// subscribe to updates for a specific flight
        /// </summary>
        /// <param name="flightId">Flight ID to subscribe to</param>
        public async Task SubscribeToFlight(int flightId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"flight_{flightId}");

            try
            {
                var flightDto = await _flightService.GetFlightByIdAsync(flightId);
                if (flightDto != null)
                {
                    await Clients.Caller.SendAsync("FlightStatusChanged", flightDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending initial flight data: {ex.Message}");
            }
        }

        /// <summary>
        /// unsubscribe from updates for a specific flight
        /// </summary>
        /// <param name="flightId">Flight ID to unsubscribe from</param>
        public async Task UnsubscribeFromFlight(int flightId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"flight_{flightId}");
        }

        /// <summary>
        /// subscribe to all flight updates (for display screens)
        /// </summary>
        public async Task SubscribeToAllFlights()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "all_flights");

            try
            {
                var flightDtos = await _flightService.GetAllFlightsAsync();
                var flightList = flightDtos.ToList();

                await Clients.Caller.SendAsync("FlightListUpdated", flightList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending initial flight data: {ex.Message}");
            }
        }

        /// <summary>
        /// request current flight data
        /// </summary>
        public async Task GetCurrentFlights()
        {
            try
            {
                var flightDtos = await _flightService.GetAllFlightsAsync();
                var flightList = flightDtos.ToList();
                await Clients.Caller.SendAsync("FlightListUpdated", flightList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting current flights: {ex.Message}");
            }
        }
    }

    public static class FlightInfoHubExtensions
    {
        public static async Task SendFlightStatusUpdateAsync(this IHubContext<FlightInfoHub> hubContext, FlightDto flightDto)
        {
            if (flightDto == null)
                throw new ArgumentNullException(nameof(flightDto));

            await hubContext.Clients.Group($"flight_{flightDto.Id}").SendAsync("FlightStatusChanged", flightDto);
            await hubContext.Clients.Group("all_flights").SendAsync("FlightStatusChanged", flightDto);
        }

        public static async Task SendSeatAssignmentUpdateAsync(this IHubContext<FlightInfoHub> hubContext, int flightId, string seatNumber, bool isAssigned)
        {
            var seatInfo = new { FlightId = flightId, SeatNumber = seatNumber, IsAssigned = isAssigned };
            await hubContext.Clients.Group($"flight_{flightId}").SendAsync("SeatAssignmentChanged", seatInfo);
            await hubContext.Clients.Group("all_flights").SendAsync("SeatAssignmentChanged", seatInfo);
        }

        public static async Task SendBoardingUpdateAsync(this IHubContext<FlightInfoHub> hubContext, int flightId, int totalPassengers, int boardedPassengers)
        {
            var boardingInfo = new
            {
                FlightId = flightId,
                TotalPassengers = totalPassengers,
                BoardedPassengers = boardedPassengers,
                BoardingPercentage = totalPassengers > 0 ? (int)((double)boardedPassengers / totalPassengers * 100) : 0
            };

            await hubContext.Clients.Group($"flight_{flightId}").SendAsync("BoardingStatusChanged", boardingInfo);
            await hubContext.Clients.Group("all_flights").SendAsync("BoardingStatusChanged", boardingInfo);
        }
    }
}