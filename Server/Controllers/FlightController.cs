using BusinessLogic.DTO;
using BusinessLogic.Services;
using BusinessLogic.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models.Enum;
using Server.Hubs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IHubContext<FlightInfoHub> _hubContext;

        public FlightController(IFlightService flightService, IHubContext<FlightInfoHub> hubContext)
        {
            _flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        /// <summary>
        /// Get flight by ID
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>Flight details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FlightDto>> GetById(int id)
        {
            var flight = await _flightService.GetFlightByIdAsync(id);

            if (flight == null)
                return NotFound(new { message = $"Flight with ID {id} not found" });

            return Ok(flight);
        }

        /// <summary>
        /// Get flight by flight number
        /// </summary>
        /// <param name="flightNumber">Flight number</param>
        /// <returns>Flight details</returns>
        [HttpGet("number/{flightNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FlightDto>> GetByFlightNumber(string flightNumber)
        {
            var flight = await _flightService.GetFlightByNumberAsync(flightNumber);

            if (flight == null)
                return NotFound(new { message = $"Flight with number {flightNumber} not found" });

            return Ok(flight);
        }

        /// <summary>
        /// Get all flights
        /// </summary>
        /// <returns>List of flights</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetAll()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            return Ok(flights);
        }

        /// <summary>
        /// Get flights by status
        /// </summary>
        /// <param name="status">Flight status</param>
        /// <returns>List of flights with the specified status</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetByStatus(string status)
        {
            if (!Enum.TryParse<FlightStatus>(status, true, out var flightStatus))
                return BadRequest(new { message = "Invalid flight status" });

            var flights = await _flightService.GetFlightsByStatusAsync(flightStatus);
            return Ok(flights);
        }

        /// <summary>
        /// Update flight status
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <param name="status">New flight status</param>
        /// <returns>Result of the update operation</returns>
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateRequest request)
        {
            if (!Enum.TryParse<FlightStatus>(request.Status, true, out var newStatus))
                return BadRequest(new { message = "Invalid flight status" });

            // get current flight 
            var flight = await _flightService.GetFlightByIdAsync(id);
            if (flight == null)
                return NotFound(new { message = $"Flight with ID {id} not found" });

            // update status
            var success = await _flightService.UpdateFlightStatusAsync(id, newStatus);

            if (!success)
                return BadRequest(new { message = "Failed to update flight status" });

            // get updated flight for notification
            var updatedFlight = await _flightService.GetFlightByIdAsync(id);

            // sned to clients w signalr
            await _hubContext.SendFlightStatusUpdateAsync(new FlightDto
            {
                Id = updatedFlight.Id,

                Destination = updatedFlight.Destination,
                FlightNumber = updatedFlight.FlightNumber,
                DepartureTime = updatedFlight.DepartureTime,
                Gate = updatedFlight.Gate,
                Status = updatedFlight.Status
            });

            return Ok(new { message = $"Flight status updated to {newStatus}" });
        }
    }

    public class StatusUpdateRequest
    {
        public string Status { get; set; }
    }
}