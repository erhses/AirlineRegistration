using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using BusinessLogic.DTO;
using Models.Entities;
using Models.Enum;

namespace InfoDis.Services
{
    public class SignalRClient : IAsyncDisposable
    {
        private readonly ILogger<SignalRClient> _logger;
        private HubConnection _hubConnection;
        private readonly string _hubUrl;
        private bool _started = false;

        //subscribable
        public event Action<Flight> OnFlightStatusChanged;
        public event Action<SeatAssignmentInfo> OnSeatAssignmentChanged;
        public event Action<BoardingInfo> OnBoardingStatusChanged;

        public SignalRClient(ILogger<SignalRClient> logger, string hubUrl)
        {
            _logger = logger;
            _hubUrl = hubUrl;
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async Task StartAsync()
        {
            if (_started)
                return;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();

            try
            {
                await _hubConnection.StartAsync();
                _started = true;
                _logger.LogInformation("SignalR connection started");

                await _hubConnection.InvokeAsync("SubscribeToAllFlights");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting SignalR connection");
                throw;
            }
        }

        public async Task StopAsync()
        {
            if (!_started)
                return;

            try
            {
                await _hubConnection.StopAsync();
                _started = false;
                _logger.LogInformation("SignalR connection stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping SignalR connection");
                throw;
            }
        }

        private void RegisterHandlers()
        {
            _hubConnection.On<Flight>("FlightStatusChanged", (flight) =>
            {
                _logger.LogInformation($"Flight status changed: {flight.FlightNumber} - {flight.Status}");
                OnFlightStatusChanged?.Invoke(flight);
            });

            _hubConnection.On<SeatAssignmentInfo>("SeatAssignmentChanged", (seatInfo) =>
            {
                _logger.LogInformation($"Seat assignment changed: Flight {seatInfo.FlightId}, Seat {seatInfo.SeatNumber}, IsAssigned: {seatInfo.IsAssigned}");
                OnSeatAssignmentChanged?.Invoke(seatInfo);
            });

            _hubConnection.On<BoardingInfo>("BoardingStatusChanged", (boardingInfo) =>
            {
                _logger.LogInformation($"Boarding status changed: Flight {boardingInfo.FlightId}, {boardingInfo.BoardedPassengers}/{boardingInfo.TotalPassengers} passengers boarded ({boardingInfo.BoardingPercentage}%)");
                OnBoardingStatusChanged?.Invoke(boardingInfo);
            });
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }

    public class SeatAssignmentInfo
    {
        public int FlightId { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class BoardingInfo
    {
        public int FlightId { get; set; }
        public int TotalPassengers { get; set; }
        public int BoardedPassengers { get; set; }
        public int BoardingPercentage { get; set; }
    }
}
