using System;
using System.Text.Json.Serialization;

namespace AirlineRegistration.BusinessLogic.DTOs
{
    // Base message class for all socket messages
    public class SocketMessageBase
    {
        [JsonPropertyName("messageType")]
        public string MessageType { get; set; }
    }

    // Message for subscribing to flight updates
    public class SubscribeToFlightMessage : SocketMessageBase
    {
        public SubscribeToFlightMessage()
        {
            MessageType = "SubscribeToFlight";
        }

        [JsonPropertyName("flightId")]
        public int FlightId { get; set; }
    }

    // Message for unsubscribing from flight updates
    public class UnsubscribeFromFlightMessage : SocketMessageBase
    {
        public UnsubscribeFromFlightMessage()
        {
            MessageType = "UnsubscribeFromFlight";
        }

        [JsonPropertyName("flightId")]
        public int FlightId { get; set; }
    }

    // Message for seat assignment updates
    public class SeatAssignmentMessage : SocketMessageBase
    {
        public SeatAssignmentMessage()
        {
            MessageType = "SeatAssignment";
        }

        [JsonPropertyName("flightId")]
        public int FlightId { get; set; }

        [JsonPropertyName("seatNumber")]
        public string SeatNumber { get; set; }

        [JsonPropertyName("isAssigned")]
        public bool IsAssigned { get; set; }
    }

    // Generic response message
    public class ResponseMessage : SocketMessageBase
    {
        public ResponseMessage()
        {
            MessageType = "Response";
        }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}