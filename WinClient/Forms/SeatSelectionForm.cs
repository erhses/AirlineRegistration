using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinClient.Services;
using BusinessLogic.DTO;

namespace WinClient.Forms
{
    public partial class SeatSelectionForm : Form
    {
        private readonly ApiClient _apiClient;
        private readonly SocketClient _socketClient;
        private readonly FlightDto _flight;
        private readonly PassengerDto _passenger;
        private List<SeatDto> _seats;
        private Dictionary<string, Button> _seatButtons;

        private const int SeatWidth = 50;
        private const int SeatHeight = 50;
        private const int Spacing = 10;

        private void OnSeatAssignmentChanged(object sender, SeatAssignmentChangedEventArgs e)
        {
            // Make sure this event is for our flight
            if (e.FlightId != _flight.Id) return;

            // Update UI on the UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateSeatStatus(e.SeatNumber, e.IsAssigned)));
            }
            else
            {
                UpdateSeatStatus(e.SeatNumber, e.IsAssigned);
            }
        }

        private void UpdateSeatStatus(string seatNumber, bool isAssigned)
        {
            // Find the button for this seat
            if (_seatButtons.TryGetValue(seatNumber, out Button seatButton))
            {
                if (isAssigned)
                {
                    // Seat was just assigned, mark it as occupied
                    seatButton.BackColor = Color.Red;
                    seatButton.Enabled = false;
                    seatButton.Text = $"{seatNumber}\nX";
                }
                else
                {
                    // Seat was just freed up, mark it as available
                    seatButton.BackColor = Color.LightGreen;
                    seatButton.Enabled = true;
                    seatButton.Text = seatNumber;
                }
            }
        }

        public SeatSelectionForm(ApiClient apiClient, SocketClient socketClient, FlightDto flight, PassengerDto passenger)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _socketClient = socketClient;
            _flight = flight;
            _passenger = passenger;
            _seatButtons = new Dictionary<string, Button>();

            // Subscribe to socket events
            _socketClient.SeatAssignmentChanged += OnSeatAssignmentChanged;
        }

        private void SeatSelectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // unsubscribe from socket events
            _socketClient.SeatAssignmentChanged -= OnSeatAssignmentChanged;

            // unsubscribe from flight updates
            Task.Run(async () => await _socketClient.UnsubscribeFromFlightAsync(_flight.Id));
        }

        private async void SeatSelectionForm_Load(object sender, EventArgs e)
        {
            lblTitle.Text = $"Select Seat for {_passenger.FirstName} {_passenger.LastName}";
            lblFlight.Text = $"Flight: {_flight.FlightNumber}";

            try
            {
                // subscribe to flight updates w socket
                await _socketClient.SubscribeToFlightAsync(_flight.Id);

                // Get initial seat data
                _seats = (await _apiClient.GetSeatsForFlightAsync(_flight.Id)).ToList();
                CreateSeatMap();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading seats: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void CreateSeatMap()
        {
            panelSeats.Controls.Clear();
            _seatButtons.Clear();

            var seatGroups = _seats.GroupBy(s => s.SeatNumber[0])
                                  .OrderBy(g => g.Key);

            int rowIndex = 0;
            foreach (var group in seatGroups)
            {
                // Add row label
                var rowLabel = new Label
                {
                    Text = group.Key.ToString(),
                    Location = new Point(10, 60 + rowIndex * (SeatHeight + Spacing)),
                    AutoSize = true
                };
                panelSeats.Controls.Add(rowLabel);

                // Add seats in this row
                int colIndex = 0;
                foreach (var seat in group.OrderBy(s => s.SeatNumber))
                {
                    var btn = new Button
                    {
                        Width = SeatWidth,
                        Height = SeatHeight,
                        Location = new Point(
                            50 + colIndex * (SeatWidth + Spacing),
                            60 + rowIndex * (SeatHeight + Spacing)),
                        Text = seat.SeatNumber,
                        Tag = seat,
                        BackColor = seat.IsOccupied ? Color.Red : Color.Green,
                        Enabled = !seat.IsOccupied
                    };

                    btn.Click += SeatButton_Click;
                    panelSeats.Controls.Add(btn);
                    _seatButtons.Add(seat.SeatNumber, btn);
                    colIndex++;
                }
                rowIndex++;
            }

            // Add aisle indicator in the middle
            var aisleLabel = new Label
            {
                Text = "Aisle",
                Location = new Point(panelSeats.Width / 2, 30),
                AutoSize = true
            };
            panelSeats.Controls.Add(aisleLabel);
        }

        private async void SeatButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var seat = (SeatDto)button.Tag;

            try
            {
                // Assign seat via API
                var (success, msg) = await _apiClient.AssignSeatAsync(_flight.Id, _passenger.PassportNumber, seat.SeatNumber);

                if (success)
                {
                    // Notify all clients via socket that this seat is now assigned
                    await _socketClient.NotifySeatAssignmentAsync(_flight.Id, seat.SeatNumber, true);

                    MessageBox.Show($"Seat {seat.SeatNumber} assigned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var boardingPass = new BoardingPassDto
                    {
                        PassengerName = $"{_passenger.FirstName} {_passenger.LastName}",
                        PassportNumber = _passenger.PassportNumber,
                        FlightNumber = _flight.FlightNumber,
                        Origin = _flight.Origin,
                        Destination = _flight.Destination,
                        DepartureTime = _flight.DepartureTime,
                        BoardingTime = _flight.DepartureTime.AddMinutes(-30), 
                        Gate = _flight.Gate,
                        SeatNumber = seat.SeatNumber,
                        Barcode = $"{_flight.FlightNumber}-{_passenger.PassportNumber}-{seat.SeatNumber}",
                        IssuedAt = DateTime.Now
                    };
                    var printer = new BoardingPassPrinter(boardingPass);
                    printer.ShowPrintPreview();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to assign seat. Please try another seat.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Refresh seat map
                    _seats = (await _apiClient.GetSeatsForFlightAsync(_flight.Id)).ToList();
                    CreateSeatMap();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning seat: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}