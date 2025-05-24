using BusinessLogic.Services.Interface;
using BusinessLogic.Services;
using DataAccess.Interface;
using DataAccess.Repositories;
using Microsoft.OpenApi.Models;
using Models;
using Server.Hubs;
using Server.SocketServerImp;
using Server.Services;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // dbcontext registration
            services.AddDbContext<FlightCheckInContext>();

            // repo registration
            services.AddScoped<IFlightRepository, FlightRepository>();
            services.AddScoped<IPassengerRepository, PassengerRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            // services registration
            services.AddScoped<IFlightInfoNotificationService, FlightInfoNotificationService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IPassengerService, PassengerService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IBoardingPassService, BoardingPassService>();

            // socketserver registration
            services.AddSingleton<SocketServer>(provider =>
            {
                return new SocketServer(8888, provider);
            });

            // signalr
            services.AddSignalR();

            // controllers
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Airline Registration API", Version = "v1" });
            });

            // cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorWasm",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5114", "https://localhost:7109")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials(); // Important for SignalR
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SocketServer socketServer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airline Registration API v1"));
                app.UseWebAssemblyDebugging();
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<FlightInfoHub>("/flightinfo");
                endpoints.MapFallbackToFile("index.html");
            });

            // start socket server
            socketServer.Start();
            Console.WriteLine("Server started successfully! Booom");
        }
    }
}