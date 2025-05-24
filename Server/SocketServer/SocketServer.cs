using BusinessLogic.Services.Interface;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Server.SocketServerImp
{
    public class SocketServer : IDisposable
    {
        private readonly TcpListener _server;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ConcurrentDictionary<string, ClientHandler> _clients;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _port;
        private bool _isRunning;
        private bool _disposed;

        public SocketServer(int port, IServiceProvider serviceProvider)
        {
            _port = port;
            _serviceProvider = serviceProvider;
            _server = new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();
            _clients = new ConcurrentDictionary<string, ClientHandler>();

            ClientHandler.SeatAssignmentChanged += OnSeatAssignmentChanged;
        }

        public void Start()
        {
            if (_isRunning) return;

            _server.Start();
            _isRunning = true;
            Console.WriteLine($"Socket server started on port {_port}");

            // start accepting clients in background
            _ = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await _server.AcceptTcpClientAsync(cancellationToken);
                    _ = HandleClientAsync(client, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AcceptClients error: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            var clientId = Guid.NewGuid().ToString();
            Console.WriteLine($"Client connected: {clientId}");

            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var handler = new ClientHandler(
                client,
                clientId,
                services.GetRequiredService<IFlightService>(),
                services.GetRequiredService<ISeatService>(),
                services.GetRequiredService<IPassengerService>(),
                services.GetRequiredService<IBoardingPassService>(),
                () => HandleClientDisconnected(clientId));

            _clients.TryAdd(clientId, handler);

            try
            {
                await handler.RunAsync(cancellationToken);
            }
            finally
            {
                _clients.TryRemove(clientId, out _);
                client.Dispose();
                Console.WriteLine($"Client disconnected: {clientId}");
            }
        }

        private void HandleClientDisconnected(string clientId)
        {
            _clients.TryRemove(clientId, out _);
        }

        // handler seat assignment changes
        private async void OnSeatAssignmentChanged(int flightId, string seatNumber, bool isAssigned)
        {
            // broadcast to all clients
            foreach (var client in _clients.Values)
            {
                try
                {
                    await client.NotifySeatAssignmentAsync(flightId, seatNumber, isAssigned);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error notifying client: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            ClientHandler.SeatAssignmentChanged -= OnSeatAssignmentChanged;
            _cancellationTokenSource.Cancel();
            foreach (var client in _clients.Values)
            {
                client.Disconnect();
            }
            _clients.Clear();
            _server.Stop();
            _isRunning = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) Stop();
            _disposed = true;
        }
    }
}