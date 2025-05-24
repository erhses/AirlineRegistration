using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enum;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Models
{
    public class FlightCheckInContext : DbContext
    {
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=flightcheckin.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Passenger)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PassengerId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Flight)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FlightId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Seat)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.SeatId);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Aircraft)
                .WithMany(a => a.Flights)
                .HasForeignKey(f => f.AircraftId);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Aircraft)
                .WithMany(a => a.Seats)
                .HasForeignKey(s => s.AircraftId);

            modelBuilder.Entity<Passenger>()
                .HasIndex(p => p.PassportNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingReference)
                .IsUnique();

            // Seed test data
            SeedData(modelBuilder);
        }

        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Aircraft
            var aircrafts = new List<Aircraft> {
                new Aircraft
            {
                Id = 1,
                Model = "Boeing 737",
                RegistrationNumber = "MN-737",
                TotalSeats = 4
            },
                new Aircraft
                {
                    Id = 2,
                    Model = "Airbus A320",
                    RegistrationNumber = "MN-320",
                    TotalSeats = 16
                }
            };

            modelBuilder.Entity<Aircraft>().HasData(aircrafts);

            // Seats
            var seats = new List<Seat>
        {
            new Seat { Id = 1, SeatNumber = "1A", SeatClass = SeatClass.Economy, AircraftId = 1, IsOccupied = false },
            new Seat { Id = 2, SeatNumber = "1B", SeatClass = SeatClass.Economy, AircraftId = 1, IsOccupied = false},
            new Seat { Id = 3, SeatNumber = "2A", SeatClass = SeatClass.Business, AircraftId = 1, IsOccupied = false },
            new Seat { Id = 4, SeatNumber = "2B", SeatClass = SeatClass.Business, AircraftId = 1 , IsOccupied = false},
            new Seat { Id = 5, SeatNumber = "3A", SeatClass = SeatClass.First, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 6, SeatNumber = "3B", SeatClass = SeatClass.First, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 7, SeatNumber = "4A", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 8, SeatNumber = "4B", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 9, SeatNumber = "5A", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 10, SeatNumber = "5B", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 11, SeatNumber = "6A", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 12, SeatNumber = "6B", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 13, SeatNumber = "7A", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 14, SeatNumber = "7B", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 15, SeatNumber = "8A", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 16, SeatNumber = "8B", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
            new Seat { Id = 17, SeatNumber = "9A", SeatClass = SeatClass.Economy, AircraftId = 2, IsOccupied = false },
        };

            modelBuilder.Entity<Seat>().HasData(seats);

            // Flight
            var flights = new List<Flight>
            {
                new Flight
                {
                    Id = 2,
                    FlightNumber = "OM401",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Tokyo",
                    DepartureTime = DateTime.UtcNow.AddDays(2),
                    ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(3),
                    Gate = "B1",
                    Status = FlightStatus.Registering,
                    AircraftId = 2
                },
                new Flight
            {
                Id = 1,
                FlightNumber = "OM402",
                DepartureCity = "Ulaanbaatar",
                ArrivalCity = "Seoul",
                DepartureTime = DateTime.UtcNow.AddDays(1),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(3),
                Gate = "A1",
                Status = FlightStatus.Boarding,
                AircraftId = 1
            }
            };

            modelBuilder.Entity<Flight>().HasData(flights);

            // Passenger
            var passengers = new List<Passenger>
            {
                new Passenger
            {
                Id = 1,
                FirstName = "Bat",
                LastName = "Erdene",
                PassportNumber = "A1234567",
                Nationality = "Mongolian",
                DateOfBirth = new DateTime(1995, 5, 10)
            },new Passenger
            {
                Id = 2,
                FirstName = "Bruce",
                LastName = "Lee",
                PassportNumber = "A1122334",
                Nationality = "Mongolian",
                DateOfBirth = new DateTime(1995, 6, 23)
            }
            };

            modelBuilder.Entity<Passenger>().HasData(passengers);

            // Booking
            var bookings = new List<Booking>{
                new Booking
                {
                    Id = 1,
                    BookingReference = "BK001",
                    BookingDate = DateTime.UtcNow,
                    IsCheckedIn = false,
                    CheckInTime = DateTime.UtcNow.AddMinutes(-60),
                    BoardingPassNumber = "MN101-A12-05161045",
                    Barcode = "BP-ABCDE12345",
                    Gate = "A1",
                    BoardingTime = DateTime.UtcNow.AddDays(1).AddMinutes(-30),
                    PassengerId = 1,
                    FlightId = 1,
                    SeatId = 1,
                    BoardingPassIssuedAt = DateTime.UtcNow
                },
                new Booking
                {
                    Id = 2,
                    BookingReference = "BK002",
                    BookingDate = DateTime.UtcNow,
                    IsCheckedIn = false,
                    CheckInTime = DateTime.UtcNow.AddMinutes(-60),
                    BoardingPassNumber = "MN101-A12-05161045",
                    Barcode = "BP-ABCDE12345",
                    Gate = "A1",
                    BoardingTime = DateTime.UtcNow.AddDays(1).AddMinutes(-30),
                    PassengerId = 2,
                    FlightId = 2,
                    SeatId = 7,
                    BoardingPassIssuedAt = DateTime.UtcNow
                } 
            };

            modelBuilder.Entity<Booking>().HasData(bookings);

            // Optional: Seed a BoardingPass object (you may skip this if not tracked by EF)
            var boardingPass = new BoardingPass
            {
                Id = 1,
                BoardingPassNumber = "MN101-A12-05161045",
                PassengerName = "Bat Erdene",
                PassportNumber = "A1234567",
                FlightNumber = "MN101",
                DepartureTime = flights[0].DepartureTime,
                Origin = "Ulaanbaatar",
                Destination = "Seoul",
                SeatNumber = "1A",
                Gate = "A1",
                BoardingTime = flights[0].DepartureTime.AddMinutes(-30),
                Barcode = "BP-ABCDE12345",
                IssuedAt = DateTime.UtcNow
            };

            modelBuilder.Entity<BoardingPass>().HasData(boardingPass);
        }
    }
}