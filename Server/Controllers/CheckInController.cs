using BusinessLogic.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enum;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly IBoardingPassService _boardingPassService;
        private readonly IPassengerService _passengerService;
        private readonly ILogger<CheckInController> _logger;

        public CheckInController(
            ISeatService seatService,
            IBoardingPassService boardingPassService,
            IPassengerService passengerService,
            ILogger<CheckInController> logger)
        {
            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
            _boardingPassService = boardingPassService ?? throw new ArgumentNullException(nameof(boardingPassService));
            _passengerService = passengerService ?? throw new ArgumentNullException(nameof(passengerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("seats/{flightId}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeats(int flightId)
        {
            var seats = await _seatService.GetSeatsByAircraftIdAsync(flightId);
            return Ok(seats);
        }

        [HttpGet("seats/{flightId}/available")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetAvailableSeats(int flightId)
        {
            var availableSeats = await _seatService.GetAvailableSeatsAsync(flightId);
            return Ok(availableSeats);
        }

        [HttpPost("seat/assign")]
        public async Task<IActionResult> AssignSeat([FromBody] SeatAssignmentRequest request)
        {
            _logger.LogInformation($"Processing seat assignment request: Flight {request.FlightId}, Passport {request.PassportNumber}, Seat {request.SeatNumber}");

            if (string.IsNullOrEmpty(request.SeatNumber))
                return BadRequest("Seat number is required");

            // Check if passenger exists
            var passenger = await _passengerService.GetPassengerByPassportAsync(request.PassportNumber);
            if (passenger == null)
                return NotFound("Passenger not found");

            // Check if passenger has a reservation
            var hasReservation = await _passengerService.HasReservationAsync(request.PassportNumber, request.FlightId);
            if (!hasReservation)
                return BadRequest("Passenger does not have a reservation for this flight");

            // Pre-check if seat is available before attempting to assign it
            var isSeatAvailable = await _seatService.IsSeatAvailableAsync(request.FlightId, request.SeatNumber);
            if (!isSeatAvailable)
            {
                _logger.LogWarning($"Seat {request.SeatNumber} on flight {request.FlightId} is already taken - rejected request from passport {request.PassportNumber}");
                return BadRequest("Seat is already taken by another passenger");
            }

            // Try to assign the seat
            var result = await _seatService.AssignSeatAsync(request.FlightId, request.SeatNumber, passenger.Id);

            if (!result.Success)
            {
                _logger.LogWarning($"Failed to assign seat: {result.Message}");
                return BadRequest(result.Message);
            }

            _logger.LogInformation($"Successfully assigned seat {request.SeatNumber} on flight {request.FlightId} to passenger {passenger.Id} (Passport: {request.PassportNumber})");
            return Ok(new { Message = result.Message });
        }


        [HttpPost("boardingpass")]
        public async Task<ActionResult<BoardingPass>> GenerateBoardingPass([FromBody] BoardingPassRequest request)
        {
            try
            {
                // Check if passenger exists
                var passenger = await _passengerService.GetPassengerByPassportAsync(request.PassportNumber);
                if (passenger == null)
                    return NotFound("Passenger not found");

                // Generate boarding pass
                var boardingPass = await _boardingPassService.GenerateBoardingPassAsync(
                    passenger.Id, request.FlightId, request.SeatNumber);

                return Ok(boardingPass);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to generate boarding pass: {ex.Message}");
            }
        }
    }

    // Request models
    public class FlightStatusUpdateRequest
    {
        public int FlightId { get; set; }
        public FlightStatus NewStatus { get; set; }
    }

    public class SeatAssignmentRequest
    {
        public int FlightId { get; set; }
        public string PassportNumber { get; set; }
        public string SeatNumber { get; set; }
    }

    public class BoardingPassRequest
    {
        public int FlightId { get; set; }
        public string PassportNumber { get; set; }
        public string SeatNumber { get; set; }
    }
}
