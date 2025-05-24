using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WinClient.Services;

namespace WinClient.Forms
{
    public partial class FlightStatusForm : Form
    {
        private readonly ApiClient _apiClient;
        private System.Windows.Forms.Timer _refreshTimer;

        public FlightStatusForm(ApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
        }

        private async void FlightStatusForm_Load(object sender, EventArgs e)
        {
            await RefreshFlightData();

            // Set up timer to refresh every 30 seconds
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 30000;
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            await RefreshFlightData();
        }

        private async Task RefreshFlightData()
        {
            try
            {
                var flights = await _apiClient.GetAllFlightsAsync();
                dgvFlights.DataSource = flights;
                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing flight data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            dgvFlights.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFlights.Columns["Id"].Visible = false;

            // Format status column with colors
            dgvFlights.Columns["Status"].DefaultCellStyle.ForeColor = Color.White;
            foreach (DataGridViewRow row in dgvFlights.Rows)
            {
                var status = row.Cells["Status"].Value?.ToString();
                if (status == "Delayed")
                    row.Cells["Status"].Style.BackColor = Color.Orange;
                else if (status == "Cancelled")
                    row.Cells["Status"].Style.BackColor = Color.Red;
                else if (status == "Boarding")
                    row.Cells["Status"].Style.BackColor = Color.Green;
                else if (status == "Departed")
                    row.Cells["Status"].Style.BackColor = Color.LightGray;
            }
        }

        private void FlightStatusForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
        }
    }
}
