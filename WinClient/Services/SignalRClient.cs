using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace AirlineRegistration.WinClient.Services
{
    public class SignalRClient : IDisposable
    {
        private readonly HubConnection _hubConnection;
        private bool _disposed = false;

        //subscribable events
        public event EventHandler<FlightStatusChangedEventArgs> FlightStatusChanged;
        public event EventHandler<BoardingStatusChangedEventArgs> BoardingStatusChanged;

        public SignalRClient(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            // handle flight status update
            _hubConnection.On<JsonElement>("FlightStatusChanged", flight =>
            {
                try
                {
                    FlightStatusChanged?.Invoke(this, new FlightStatusChangedEventArgs
                    {
                        FlightId = GetIntValue(flight, "id"),
                        Destination = GetStringValue(flight, "destination"),
                        Status = GetStringValue(flight, "status"),
                        FlightNumber = GetStringValue(flight, "flightNumber")
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing FlightStatusChanged: {ex.Message}");
                }
            });

            _hubConnection.On<JsonElement>("BoardingStatusChanged", boardingInfo =>
            {
                try
                {
                    BoardingStatusChanged?.Invoke(this, new BoardingStatusChangedEventArgs
                    {
                        FlightId = GetIntValue(boardingInfo, "flightId"),
                        TotalPassengers = GetIntValue(boardingInfo, "totalPassengers"),
                        BoardedPassengers = GetIntValue(boardingInfo, "boardedPassengers"),
                        BoardingPercentage = GetIntValue(boardingInfo, "boardingPercentage")
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing BoardingStatusChanged: {ex.Message}");
                }
            });
        }

        private string GetStringValue(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
                return null;

            switch (property.ValueKind)
            {
                case JsonValueKind.String:
                    return property.GetString();
                case JsonValueKind.Number:
                    return property.GetInt32().ToString();
                case JsonValueKind.Null:
                    return null;
                default:
                    return property.ToString();
            }
        }

        private int GetIntValue(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
                return 0;

            switch (property.ValueKind)
            {
                case JsonValueKind.Number:
                    return property.GetInt32();
                case JsonValueKind.String:
                    if (int.TryParse(property.GetString(), out var intValue))
                        return intValue;
                    return 0;
                default:
                    return 0;
            }
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR hub: {ex.Message}");
            }
        }

        public async Task SubscribeToFlightAsync(int flightId)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("SubscribeToFlight", flightId);
            }
        }

        public async Task UnsubscribeFromFlightAsync(int flightId)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("UnsubscribeFromFlight", flightId);
            }
        }

        public async Task SubscribeToAllFlightsAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("SubscribeToAllFlights");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _hubConnection.DisposeAsync().AsTask().Wait();
                }

                _disposed = true;
            }
        }
    }

    public class FlightStatusChangedEventArgs : EventArgs
    {
        public int FlightId { get; set; }
        public string Destination {  get; set; }
        public string Status { get; set; }
        public string FlightNumber { get; set; }
    }

    public class BoardingStatusChangedEventArgs : EventArgs
    {
        public int FlightId { get; set; }
        public int TotalPassengers { get; set; }
        public int BoardedPassengers { get; set; }
        public int BoardingPercentage { get; set; }
    }
}