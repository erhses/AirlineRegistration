using System;
using System.Windows.Forms;
using AirlineRegistration.WinClient.Services;
using BusinessLogic.DTO;
using Microsoft.AspNetCore.Hosting.Server;
using Models.Enum;
using WinClient.Forms;
using WinClient.Services;

namespace WinClient
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _apiClient;
        private readonly SignalRClient _signalRClient;
        private readonly SocketClient _socketClient;

        public Form1()
        {
            InitializeComponent();
            _apiClient = new ApiClient();

            _socketClient = new SocketClient("localhost", 8888);
            _socketClient.Connected += SocketClient_Connected;
            _socketClient.Disconnected += SocketClient_Disconnected;
            _socketClient.ConnectionError += SocketClient_ConnectionError;
            _socketClient.MessageReceived += SocketClient_MessageReceived;

            _signalRClient = new SignalRClient("https://localhost:7109/flightinfo");

            // Subscribe to SignalR events
            _signalRClient.FlightStatusChanged += SignalRClient_FlightStatusChanged;
            _signalRClient.BoardingStatusChanged += SignalRClient_BoardingStatusChanged;

            this.FormClosing += Form1_FormClosing;
        }

        private void SignalRClient_FlightStatusChanged(object sender, FlightStatusChangedEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                AppendToLog($"Flight {e.FlightNumber} status changed to {e.Status}");
            }));
        }

        private void SignalRClient_BoardingStatusChanged(object sender, BoardingStatusChangedEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                AppendToLog($"Flight {e.FlightId} boarding progress: {e.BoardedPassengers}/{e.TotalPassengers} ({e.BoardingPercentage}%)");
            }));
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // signalr connection
                await _signalRClient.StartAsync();
                await _signalRClient.SubscribeToAllFlightsAsync();

                await _socketClient.ConnectAsync();

                var flights = await _apiClient.GetAllFlightsAsync();
                cboFlights.DataSource = flights;
                cboFlights.DisplayMember = "FlightNumber";
                cboFlights.ValueMember = "Id";

                cboFlightStatus.DataSource = Enum.GetValues(typeof(FlightStatus));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_socketClient.IsConnected)
            {
                await _socketClient.DisconnectAsync();
            }

            _signalRClient?.Dispose();
        }

        private void SocketClient_Connected(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                statusStripMain.Items["lblSocketStatus"].Text = "Socket Status: Connected";
                statusStripMain.Items["lblSocketStatus"].ForeColor = System.Drawing.Color.Green;
            }));
        }

        private void SocketClient_Disconnected(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                statusStripMain.Items["lblSocketStatus"].Text = "Socket Status: Disconnected";
                statusStripMain.Items["lblSocketStatus"].ForeColor = System.Drawing.Color.Red;
            }));
        }

        private void SocketClient_ConnectionError(object sender, string e)
        {
            this.Invoke(new Action(() =>
            {
                statusStripMain.Items["lblSocketStatus"].Text = "Socket Status: Error";
                statusStripMain.Items["lblSocketStatus"].ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Socket connection error: {e}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        private void SocketClient_MessageReceived(object sender, string e)
        {
            this.Invoke(new Action(() =>
            {
                AppendToLog($"Message received: {e}");
            }));
        }

        private void AppendToLog(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            if (cboFlights.SelectedItem == null)
            {
                MessageBox.Show("Please select a flight first.", "Flight Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedFlight = (FlightDto)cboFlights.SelectedItem;
            var checkInForm = new CheckInForm(_apiClient, _socketClient, selectedFlight);
            checkInForm.ShowDialog();
        }

        private async void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (cboFlights.SelectedItem == null || cboFlightStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a flight and status.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedFlight = (FlightDto)cboFlights.SelectedItem;
            var selectedStatus = cboFlightStatus.SelectedItem.ToString();

            try
            {
                // update flight status API this should trigger SignalR notifications on the server side
                var success = await _apiClient.UpdateFlightStatusAsync(selectedFlight.Id, selectedStatus);

                if (success)
                {
                    MessageBox.Show($"Flight {selectedFlight.FlightNumber} status updated to {selectedStatus}.",
                        "Status Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AppendToLog($"Updated flight {selectedFlight.FlightNumber} status to {selectedStatus}");

                    
                }
                else
                {
                    MessageBox.Show("Failed to update flight status.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating flight status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewFlightStatus_Click(object sender, EventArgs e)
        {
            var flightStatusForm = new FlightStatusForm(_apiClient);
            flightStatusForm.ShowDialog();
        }
    }
}