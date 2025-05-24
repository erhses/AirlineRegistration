using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string BookingReference { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public bool IsCheckedIn { get; set; } = false;
        public DateTime? CheckInTime { get; set; }

        [StringLength(20)]
        public string BoardingPassNumber { get; set; }

        public DateTime? BoardingPassIssuedAt { get; set; }

        [StringLength(50)]
        public string Barcode { get; set; }

        [StringLength(10)]
        public string Gate { get; set; }

        public DateTime? BoardingTime { get; set; }

        // Navigation properties
        [Required]
        public Passenger Passenger { get; set; }
        public int PassengerId { get; set; }

        [Required]
        public Flight Flight { get; set; }
        public int FlightId { get; set; }

        public Seat Seat { get; set; }
        public int? SeatId { get; set; }

        // Method to generate boarding pass information
        public void GenerateBoardingPass()
        {
            if (Seat == null)
                throw new InvalidOperationException("Cannot generate boarding pass without assigned seat");

            if (Flight == null)
                throw new InvalidOperationException("Flight information missing");

            BoardingPassNumber = $"{Flight.FlightNumber}-{Passenger.PassportNumber.Substring(0, 3)}-{DateTime.UtcNow:MMddHHmm}";
            Barcode = $"BP-{Guid.NewGuid().ToString("N").Substring(0, 10)}";
            BoardingPassIssuedAt = DateTime.UtcNow;
            IsCheckedIn = true;
            CheckInTime = DateTime.UtcNow;

            // Set boarding time 30 minutes before departure
            BoardingTime = Flight.DepartureTime.AddMinutes(-30);

            // Assign gate if available
            Gate = Flight.Gate ?? "TBD";
        }

        // Method to validate boarding pass
        public bool IsBoardingPassValid()
        {
            return !string.IsNullOrEmpty(BoardingPassNumber) &&
                   !string.IsNullOrEmpty(Barcode) &&
                   BoardingPassIssuedAt.HasValue &&
                   Seat != null;
        }
    }
}