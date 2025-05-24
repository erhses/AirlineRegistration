using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AirlineRegistration.BusinessLogic.DTOs;

namespace WinClient.Services
{
    public class SeatAssignmentChangedEventArgs : EventArgs
    {
        public int FlightId { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class SocketClient : IDisposable
    {
        private readonly string _serverAddress;
        private readonly int _serverPort;
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _receiveTask;
        private bool _isConnected;
        private bool _disposed;
        private readonly object _sendLock = new object();

        public event EventHandler<string> MessageReceived;
        public event EventHandler<string> ConnectionError;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        //event seat assignment changed
        public event EventHandler<SeatAssignmentChangedEventArgs> SeatAssignmentChanged;

        public SocketClient(string serverAddress, int serverPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _isConnected = false;
        }
        public bool IsConnected => _isConnected;
        public async Task ConnectAsync()
        {
            if (_isConnected) return;

            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_serverAddress, _serverPort);
                _stream = _client.GetStream();
                _isConnected = true;

                // Start receiving messages
                _cancellationTokenSource = new CancellationTokenSource();
                _receiveTask = Task.Run(() => ReceiveMessagesAsync(_cancellationTokenSource.Token));

                Console.WriteLine($"Connected to socket server at {_serverAddress}:{_serverPort}");
            }
            catch (Exception ex)
            {
                _isConnected = false;
                throw new Exception($"Failed to connect to socket server: {ex.Message}", ex);
            }
        }

        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            try
            {
                while (!cancellationToken.IsCancellationRequested && _client.Connected)
                {
                    var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0) break;

                    var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    await ProcessMessageAsync(json);
                }
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving messages: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
            }
        }

        private async Task ProcessMessageAsync(string json)
        {
            try
            {
                var baseMessage = JsonSerializer.Deserialize<SocketMessageBase>(json);

                if (baseMessage?.MessageType == "SeatAssignment")
                {
                    var message = JsonSerializer.Deserialize<SeatAssignmentMessage>(json);

                    // trigger event
                    SeatAssignmentChanged?.Invoke(this, new SeatAssignmentChangedEventArgs
                    {
                        FlightId = message.FlightId,
                        SeatNumber = message.SeatNumber,
                        IsAssigned = message.IsAssigned
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        public async Task SubscribeToFlightAsync(int flightId)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Client is not connected");

            var message = new SubscribeToFlightMessage
            {
                FlightId = flightId
            };

            await SendMessageAsync(message);
        }

        public async Task UnsubscribeFromFlightAsync(int flightId)
        {
            if (!_isConnected)
                return; // silent return if fail

            var message = new UnsubscribeFromFlightMessage
            {
                FlightId = flightId
            };

            await SendMessageAsync(message);
        }

        public async Task NotifySeatAssignmentAsync(int flightId, string seatNumber, bool isAssigned)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Client is not connected");

            var message = new SeatAssignmentMessage
            {
                FlightId = flightId,
                SeatNumber = seatNumber,
                IsAssigned = isAssigned
            };

            await SendMessageAsync(message);
        }

        private async Task SendMessageAsync<T>(T message) where T : SocketMessageBase
        {
            if (!_isConnected)
                throw new InvalidOperationException("Client is not connected");

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            lock (_sendLock)
            {
                _stream.Write(bytes, 0, bytes.Length);
            }

            await Task.CompletedTask; 
        }

        public async Task DisconnectAsync()
        {
            if (!_isConnected)
                return;

            _cancellationTokenSource?.Cancel();

            try
            {
                _stream?.Close();
                _client?.Close();
            }
            catch (Exception ex)
            {
                ConnectionError?.Invoke(this, $"Disconnect error: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                DisconnectAsync();
                _cancellationTokenSource?.Dispose();
                _client?.Dispose();
            }

            _disposed = true;
        }
    }
}