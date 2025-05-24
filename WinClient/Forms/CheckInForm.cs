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
    public partial class CheckInForm : Form
    {
        private readonly ApiClient _apiClient;
        private readonly FlightDto _selectedFlight;
        private readonly SocketClient _socketClient;
        private PassengerDto _passenger;
        public CheckInForm(ApiClient apiClient, SocketClient socketClient,FlightDto selectedFlight)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _selectedFlight = selectedFlight;
            _socketClient = socketClient;
        }
        private async void CheckInForm_Load(object sender, EventArgs e)
        {
            lblFlightInfo.Text = $"Flight: {_selectedFlight.FlightNumber} | {_selectedFlight.Origin} → {_selectedFlight.Destination}";
            lblStatus.Text = $"Status: {_selectedFlight.Status}";
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassport.Text))
            {
                MessageBox.Show("Please enter a passport number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _passenger = await _apiClient.GetPassengerByPassportAsync(txtPassport.Text);
                if (_passenger == null)
                {
                    MessageBox.Show("Passenger not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblPassengerName.Text = $"Passenger: {_passenger.FirstName} {_passenger.LastName}";
                btnSelectSeat.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching passenger: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelectSeat_Click(object sender, EventArgs e)
        {
            var seatForm = new SeatSelectionForm(_apiClient,_socketClient,_selectedFlight, _passenger);
            if (seatForm.ShowDialog() == DialogResult.OK)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
