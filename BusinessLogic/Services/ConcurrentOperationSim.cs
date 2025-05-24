//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BusinessLogic.Services.Interface;

//namespace BusinessLogic.Services
//{
//    public class ConcurrentOperationSim
//    {
//        //private readonly ISeatService _seatService;
//        //private static readonly Random _random = new Random();

//        //public ConcurrentOperationSim(ISeatService seatService)
//        //{
//        //    _seatService = seatService;
//        //}

//        //public async Task SimulateConcurrentCheckIns(int bookingId, string seatNumber, int simulationCount)
//        //{
//        //    var tasks = new Task<SeatAssignmentResult>[simulationCount];

//        //    for (int i = 0; i < simulationCount; i++)
//        //    {
//        //        tasks[i] = Task.Run(async () =>
//        //        {
//        //            // Simulate slight delay between requests
//        //            await Task.Delay(_random.Next(50, 200));
//        //            return await _seatService.AssignSeatAsync(bookingId, seatNumber);
//        //        });
//        //    }

//        //    var results = await Task.WhenAll(tasks);
//        //    var successfulAssignments = results.Count(r => r.Success);

//        //    Console.WriteLine($"Simulation complete. Successful assignments: {successfulAssignments}/{simulationCount}");
//        //    foreach (var result in results.Where(r => !r.Success))
//        //    {
//        //        Console.WriteLine($"Failed: {result.Message}");
//        //    }
//        //}
//        private readonly ISeatService _seatService;

//        public ConcurrentOperationSim(ISeatService seatService)
//        {
//            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
//        }

//        /// <summary>
//        /// Simulates multiple users attempting to assign the same seat simultaneously
//        /// </summary>
//        /// <param name="flightId">Flight ID</param>
//        /// <param name="seatNumber">Seat number that all users will try to book</param>
//        /// <param name="passengerIds">List of passenger IDs who will try to book the seat</param>
//        /// <returns>The ID of the passenger who successfully booked the seat</returns>
//        public async Task<int?> SimulateMultipleBookingAttemptsAsync(
//            int flightId,
//            string seatNumber,
//            IEnumerable<int> passengerIds)
//        {
//            if (passengerIds == null || !passengerIds.Any())
//                throw new ArgumentException("Passenger IDs cannot be null or empty", nameof(passengerIds));

//            // Check if the seat is available before starting simulation
//            bool isSeatAvailable = await _seatService.IsSeatAvailableAsync(flightId, seatNumber);
//            if (!isSeatAvailable)
//                return null; // Seat is already taken

//            var tasks = new List<Task<(int PassengerId, bool Success)>>();
//            var random = new Random();

//            // Create booking tasks for all passengers
//            foreach (var passengerId in passengerIds)
//            {
//                tasks.Add(Task.Run(async () =>
//                {
//                    // Add random delay to simulate realistic conditions (0-100ms)
//                    await Task.Delay(random.Next(0, 100));

//                    // Attempt to assign the seat
//                    bool success = await _seatService.AssignSeatAsync(flightId, seatNumber, passengerId);
//                    return (passengerId, success);
//                }));
//            }

//            // Wait for all tasks to complete
//            var results = await Task.WhenAll(tasks);

//            // Find the successful booking
//            var successfulBooking = results.FirstOrDefault(r => r.Success);
//            return successfulBooking.Success ? successfulBooking.PassengerId : null;
//        }

//        /// <summary>
//        /// Simulates multiple users trying to assign different seats at the same time
//        /// </summary>
//        /// <param name="flightId">Flight ID</param>
//        /// <param name="bookingRequests">Dictionary of passenger ID to seat number</param>
//        /// <returns>Dictionary of passenger ID to success status</returns>
//        public async Task<Dictionary<int, bool>> SimulateMultipleSeatsBookingAsync(
//            int flightId,
//            Dictionary<int, string> bookingRequests)
//        {
//            if (bookingRequests == null || !bookingRequests.Any())
//                throw new ArgumentException("Booking requests cannot be null or empty", nameof(bookingRequests));

//            var tasks = new List<Task<(int PassengerId, bool Success)>>();
//            var random = new Random();

//            // Create booking tasks for all passengers with their requested seats
//            foreach (var request in bookingRequests)
//            {
//                int passengerId = request.Key;
//                string seatNumber = request.Value;

//                tasks.Add(Task.Run(async () =>
//                {
//                    // Add random delay to simulate realistic conditions (0-100ms)
//                    await Task.Delay(random.Next(0, 100));

//                    // Attempt to assign the seat
//                    bool success = await _seatService.AssignSeatAsync(flightId, seatNumber, passengerId);
//                    return (passengerId, success);
//                }));
//            }

//            // Wait for all tasks to complete
//            var results = await Task.WhenAll(tasks);

//            // Convert results to dictionary
//            return results.ToDictionary(r => r.PassengerId, r => r.Success);
//        }
//    }
//}
