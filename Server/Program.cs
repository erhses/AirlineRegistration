using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Server.SocketServerImp;
using Server;

namespace AirlineRegistration.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            InitializeDatabase(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void InitializeDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<Models.FlightCheckInContext>();

                    dbContext.Database.EnsureCreated();

                    Console.WriteLine("Database initialized successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
                }
            }
        }

        private static void ConfigureShutdown(IHost host)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                // Gracefully shut down the socket server
                var socketServer = host.Services.GetRequiredService<SocketServer>();
                socketServer.Stop();
                Console.WriteLine("Socket server stopped.");
            };
        }
    }
}