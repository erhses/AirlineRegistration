using BusinessLogic.DTO;
using BusinessLogic.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengerController : ControllerBase
    {
        private readonly IPassengerService _passengerService;

        public PassengerController(IPassengerService passengerService)
        {
            _passengerService = passengerService ?? throw new ArgumentNullException(nameof(passengerService));
        }

        /// <summary>
        /// Find a passenger by passport number
        /// </summary>
        /// <param name="passportNumber">Passport number</param>
        /// <returns>Passenger details</returns>
        [HttpGet("passport/{passportNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PassengerDto>> GetByPassport(string passportNumber)
        {
            if (string.IsNullOrWhiteSpace(passportNumber))
                return BadRequest(new { message = "Passport number is required" });

            var passenger = await _passengerService.GetPassengerByPassportAsync(passportNumber);

            if (passenger == null)
                return NotFound(new { message = $"Passenger with passport number {passportNumber} not found" });

            return Ok(passenger);
        }

        /// <summary>
        /// Check-in a passenger
        /// </summary>
        /// <param name="request">Check-in request details</param>
        /// <returns>Result of the check-in operation</returns>
        [HttpPost("checkin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request" });

            if (request.PassengerId <= 0)
                return BadRequest(new { message = "Invalid passenger ID" });

            if (request.FlightId <= 0)
                return BadRequest(new { message = "Invalid flight ID" });

            if (string.IsNullOrWhiteSpace(request.SeatNumber))
                return BadRequest(new { message = "Seat number is required" });

            // Try to check in the passenger
            var success = await _passengerService.CheckInPassengerAsync(
                request.PassengerId,
                request.FlightId,
                request.SeatNumber);

            if (!success)
                return BadRequest(new { message = "Failed to check in passenger. The seat may already be taken." });

            return Ok(new { message = "Passenger checked in successfully" });
        }
    }

    public class CheckInRequest
    {
        public int PassengerId { get; set; }
        public int FlightId { get; set; }
        public string SeatNumber { get; set; }
    }
}
