using BusinessLogic.Services.Interface;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using AirlineRegistration.BusinessLogic.DTOs;

namespace Server.SocketServerImp
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly string _clientId;
        private readonly IFlightService _flightService;
        private readonly ISeatService _seatService;
        private readonly IPassengerService _passengerService;
        private readonly IBoardingPassService _boardingPassService;
        private readonly Action _onDisconnected;
        private readonly NetworkStream _stream;

        private readonly HashSet<int> _subscribedFlightIds = new HashSet<int>();

        public delegate void SeatAssignmentEventHandler(int flightId, string seatNumber, bool isAssigned);

        public static event SeatAssignmentEventHandler SeatAssignmentChanged;

        public ClientHandler(
            TcpClient client,
            string clientId,
            IFlightService flightService,
            ISeatService seatService,
            IPassengerService passengerService,
            IBoardingPassService boardingPassService,
            Action onDisconnected)
        {
            _client = client;
            _clientId = clientId;
            _flightService = flightService;
            _seatService = seatService;
            _passengerService = passengerService;
            _boardingPassService = boardingPassService;
            _onDisconnected = onDisconnected;
            _stream = client.GetStream();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                var buffer = new byte[4096]; // Larger buffer for JSON messages
                while (!cancellationToken.IsCancellationRequested && _client.Connected)
                {
                    var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0) break;
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from {_clientId}: {message}");

                    // Process message and send response
                    var response = await ProcessMessageAsync(message);
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await _stream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client {_clientId} error: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        private async Task<string> ProcessMessageAsync(string messageJson)
        {
            try
            {
                var baseMessage = JsonSerializer.Deserialize<SocketMessageBase>(messageJson);

                switch (baseMessage?.MessageType)
                {
                    case "SubscribeToFlight":
                        return await HandleSubscribeToFlightAsync(messageJson);

                    case "UnsubscribeFromFlight":
                        return await HandleUnsubscribeFromFlightAsync(messageJson);

                    case "SeatAssignment":
                        return await HandleSeatAssignmentAsync(messageJson);

                    default:
                        return JsonSerializer.Serialize(new ResponseMessage
                        {
                            Success = false,
                            Message = "Unknown message type"
                        });
                }
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"JSON parsing error: {jex.Message}");
                return JsonSerializer.Serialize(new ResponseMessage
                {
                    Success = false,
                    Message = "Invalid message format"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                return JsonSerializer.Serialize(new ResponseMessage
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        private async Task<string> HandleSubscribeToFlightAsync(string messageJson)
        {
            var message = JsonSerializer.Deserialize<SubscribeToFlightMessage>(messageJson);

            if (message?.FlightId == null)
            {
                return JsonSerializer.Serialize(new ResponseMessage
                {
                    Success = false,
                    Message = "FlightId is required"
                });
            }

            // check if flight exists
            var flight = await _flightService.GetFlightByIdAsync(message.FlightId);
            if (flight == null)
            {
                return JsonSerializer.Serialize(new ResponseMessage
                {
                    Success = false,
                    Message = "Flight not found"
                });
            }

            _subscribedFlightIds.Add(message.FlightId);

            Console.WriteLine($"Client {_clientId} subscribed to flight {message.FlightId}");

            return JsonSerializer.Serialize(new ResponseMessage
            {
                Success = true,
                Message = $"Subscribed to flight {message.FlightId}"
            });
        }

        private async Task<string> HandleUnsubscribeFromFlightAsync(string messageJson)
        {
            var message = JsonSerializer.Deserialize<UnsubscribeFromFlightMessage>(messageJson);

            if (message?.FlightId == null)
            {
                return JsonSerializer.Serialize(new ResponseMessage
                {
                    Success = false,
                    Message = "FlightId is required"
                });
            }

            // remove from subscribed flights  
            _subscribedFlightIds.Remove(message.FlightId);

            Console.WriteLine($"Client {_clientId} unsubscribed from flight {message.FlightId}");

            return JsonSerializer.Serialize(new ResponseMessage
            {
                Success = true,
                Message = $"Unsubscribed from flight {message.FlightId}"
            });
        }

        private async Task<string> HandleSeatAssignmentAsync(string messageJson)
        {
            var message = JsonSerializer.Deserialize<SeatAssignmentMessage>(messageJson);

            if (message?.FlightId == null || string.IsNullOrEmpty(message?.SeatNumber))
            {
                return JsonSerializer.Serialize(new ResponseMessage
                {
                    Success = false,
                    Message = "FlightId and SeatNumber are required"
                });
            }

            //trigger static event for all clients
            SeatAssignmentChanged?.Invoke(message.FlightId, message.SeatNumber, message.IsAssigned);

            Console.WriteLine($"Seat assignment changed - Flight: {message.FlightId}, Seat: {message.SeatNumber}, Assigned: {message.IsAssigned}");

            return JsonSerializer.Serialize(new ResponseMessage
            {
                Success = true,
                Message = $"Seat assignment broadcast successful"
            });
        }

        public async Task NotifySeatAssignmentAsync(int flightId, string seatNumber, bool isAssigned)
        {
            if (!_subscribedFlightIds.Contains(flightId))
                return;

            try
            {
                var notification = new SeatAssignmentMessage
                {
                    FlightId = flightId,
                    SeatNumber = seatNumber,
                    IsAssigned = isAssigned
                };

                var json = JsonSerializer.Serialize(notification);
                var bytes = Encoding.UTF8.GetBytes(json);

                await _stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification to client {_clientId}: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
                _onDisconnected?.Invoke();
            }
            catch {  }
        }
    }
}