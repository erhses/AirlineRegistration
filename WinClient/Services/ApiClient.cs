using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLogic.DTO;

namespace WinClient.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient(string baseUrl = "https://localhost:7109/api")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        // get all flights
        public async Task<List<FlightDto>> GetAllFlightsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Flight");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<FlightDto>>();
        }

        // get flight by ID
        public async Task<FlightDto> GetFlightByIdAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Flight/{flightId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FlightDto>();
        }

        // get flight by flight number
        public async Task<FlightDto> GetFlightByNumberAsync(string flightNumber)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Flight/number/{flightNumber}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<FlightDto>();
        }

        // update flight status
        public async Task<bool> UpdateFlightStatusAsync(int flightId, string status)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { Status = status }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/Flight/{flightId}/status", content);
            return response.IsSuccessStatusCode;
        }

        // get all seats 
        public async Task<List<SeatDto>> GetSeatsForFlightAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/CheckIn/seats/{flightId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<SeatDto>>();
        }

        // get available seats 
        public async Task<List<SeatDto>> GetAvailableSeatsAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/CheckIn/seats/{flightId}/available");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<SeatDto>>();
        }

        // search passenger 
        public async Task<PassengerDto> GetPassengerByPassportAsync(string passportNumber)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Passenger/passport/{passportNumber}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PassengerDto>();
        }

        // assign seat 
        public async Task<(bool Success, string Message)> AssignSeatAsync(int flightId, string passportNumber, string seatNumber)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    FlightId = flightId,
                    PassportNumber = passportNumber,
                    SeatNumber = seatNumber
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/CheckIn/seat/assign", content);

            string message = await response.Content.ReadAsStringAsync();

            try
            {
                var result = JsonSerializer.Deserialize<AssignSeatResponse>(message);
                message = result?.Message ?? message;
            }
            catch
            {
            }

            return (response.IsSuccessStatusCode, message);
        }


        // generate boardingpass
        public async Task<BoardingPassDto> GenerateBoardingPassAsync(int flightId, string passportNumber, string seatNumber)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    FlightId = flightId,
                    PassportNumber = passportNumber,
                    SeatNumber = seatNumber
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/CheckIn/boardingpass", content);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<BoardingPassDto>();
        }
    }

    class AssignSeatResponse
    {
        public string Message { get; set; }
    }
}