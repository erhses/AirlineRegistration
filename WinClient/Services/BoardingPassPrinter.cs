using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
//using AirlineRegistration.WinClient.Models;
using BusinessLogic.DTO;

namespace WinClient.Services
{
    public class BoardingPassPrinter
    {
        private readonly BoardingPassDto _boardingPass;
        private readonly Font _titleFont;
        private readonly Font _headerFont;
        private readonly Font _normalFont;
        private readonly Font _boldFont;

        public BoardingPassPrinter(BoardingPassDto boardingPass)
        {
            _boardingPass = boardingPass ?? throw new ArgumentNullException(nameof(boardingPass));

            // Define fonts for printing
            _titleFont = new Font("Arial", 18, FontStyle.Bold);
            _headerFont = new Font("Arial", 12, FontStyle.Bold);
            _normalFont = new Font("Arial", 10, FontStyle.Regular);
            _boldFont = new Font("Arial", 10, FontStyle.Bold);
        }

        // Create a panel with the boarding pass to display on the form
        public Panel CreateBoardingPassPanel()
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = 600,
                Height = 320,
                BackColor = Color.White
            };

            // Airline logo/name
            var airlineLabel = new Label
            {
                Text = "MONGOLIAN AIRLINES",
                Font = _titleFont,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            panel.Controls.Add(airlineLabel);

            // Boarding pass title
            var titleLabel = new Label
            {
                Text = "BOARDING PASS",
                Font = _headerFont,
                AutoSize = true,
                Location = new Point(420, 25)
            };
            panel.Controls.Add(titleLabel);

            // Horizontal line
            var line1 = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Height = 1,
                Width = panel.Width - 40,
                Location = new Point(20, 60),
                BackColor = Color.Black
            };
            panel.Controls.Add(line1);

            // Passenger info section
            AddLabelPair(panel, "Passenger:", _boardingPass.PassengerName, 20, 70);
            AddLabelPair(panel, "Passport:", _boardingPass.PassportNumber, 20, 95);

            // Flight info section
            AddLabelPair(panel, "Flight:", _boardingPass.FlightNumber, 20, 130);
            AddLabelPair(panel, "From:", _boardingPass.Origin, 20, 155);
            AddLabelPair(panel, "To:", _boardingPass.Destination, 20, 180);

            // Time info section
            AddLabelPair(panel, "Departure:", _boardingPass.DepartureTime.ToString("MMM dd, yyyy HH:mm"), 300, 130);
            AddLabelPair(panel, "Boarding:", _boardingPass.BoardingTime?.ToString("HH:mm") ?? "N/A", 300, 155);

            // Gate and seat info
            AddLabelPair(panel, "Gate:", _boardingPass.Gate, 300, 180);

            // Seat number with larger font to highlight
            var seatLabelCaption = new Label
            {
                Text = "Seat:",
                Font = _headerFont,
                AutoSize = true,
                Location = new Point(450, 90)
            };
            panel.Controls.Add(seatLabelCaption);

            var seatNumberLabel = new Label
            {
                Text = _boardingPass.SeatNumber,
                Font = new Font("Arial", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(500, 85)
            };
            panel.Controls.Add(seatNumberLabel);

            // Horizontal line
            var line2 = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Height = 1,
                Width = panel.Width - 40,
                Location = new Point(20, 220),
                BackColor = Color.Black
            };
            panel.Controls.Add(line2);

            // Barcode info
            var barcodeLabel = new Label
            {
                Text = _boardingPass.Barcode,
                Font = new Font("Courier New", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(200, 240)
            };
            panel.Controls.Add(barcodeLabel);

            // Additional info
            var infoLabel = new Label
            {
                Text = $"Boarding pass issued: {_boardingPass.IssuedAt:yyyy-MM-dd HH:mm:ss}",
                Font = new Font("Arial", 8, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(20, 280)
            };
            panel.Controls.Add(infoLabel);

            // Warning text
            var warningLabel = new Label
            {
                Text = "Please be at the boarding gate at least 30 minutes before departure.",
                Font = new Font("Arial", 8, FontStyle.Italic),
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(20, 300)
            };
            panel.Controls.Add(warningLabel);

            return panel;
        }

        // Helper method to add label pairs (caption: value)
        private void AddLabelPair(Panel panel, string caption, string value, int x, int y)
        {
            var captionLabel = new Label
            {
                Text = caption,
                Font = _boldFont,
                AutoSize = true,
                Location = new Point(x, y)
            };
            panel.Controls.Add(captionLabel);

            var valueLabel = new Label
            {
                Text = value,
                Font = _normalFont,
                AutoSize = true,
                Location = new Point(x + 100, y)
            };
            panel.Controls.Add(valueLabel);
        }

        // Print preview support (not actually printing to a printer)
        public void ShowPrintPreview()
        {
            // This would be used if you want to implement actual printing
            // For now, we're just showing the boarding pass in a panel
            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
            previewDialog.Document = printDocument;
            previewDialog.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // This would handle the actual printing to a printer
            // But we're not implementing actual printing functionality
            Graphics g = e.Graphics;

            // Draw airline logo/name
            g.DrawString("MONGOLIAN AIRLINES", _titleFont, Brushes.Black, 50, 50);
            g.DrawString("BOARDING PASS", _headerFont, Brushes.Black, 500, 60);

            // Draw horizontal line
            g.DrawLine(Pens.Black, 50, 100, 750, 100);

            // Draw passenger info
            g.DrawString("Passenger:", _boldFont, Brushes.Black, 50, 120);
            g.DrawString(_boardingPass.PassengerName, _normalFont, Brushes.Black, 150, 120);

            g.DrawString("Passport:", _boldFont, Brushes.Black, 50, 145);
            g.DrawString(_boardingPass.PassportNumber, _normalFont, Brushes.Black, 150, 145);

            // Draw flight info
            g.DrawString("Flight:", _boldFont, Brushes.Black, 50, 180);
            g.DrawString(_boardingPass.FlightNumber, _normalFont, Brushes.Black, 150, 180);

            g.DrawString("From:", _boldFont, Brushes.Black, 50, 205);
            g.DrawString(_boardingPass.Origin, _normalFont, Brushes.Black, 150, 205);

            g.DrawString("To:", _boldFont, Brushes.Black, 50, 230);
            g.DrawString(_boardingPass.Destination, _normalFont, Brushes.Black, 150, 230);

            // Draw time info
            g.DrawString("Departure:", _boldFont, Brushes.Black, 400, 180);
            g.DrawString(_boardingPass.DepartureTime.ToString("MMM dd, yyyy HH:mm"), _normalFont, Brushes.Black, 500, 180);

            g.DrawString("Boarding:", _boldFont, Brushes.Black, 400, 205);
            g.DrawString(_boardingPass.BoardingTime?.ToString("HH:mm") ?? "N/A", _normalFont, Brushes.Black, 500, 205);

            // Draw gate info
            g.DrawString("Gate:", _boldFont, Brushes.Black, 400, 230);
            g.DrawString(_boardingPass.Gate, _normalFont, Brushes.Black, 500, 230);

            // Draw seat info (highlighted)
            g.DrawString("Seat:", _headerFont, Brushes.Black, 550, 120);
            g.DrawString(_boardingPass.SeatNumber, new Font("Arial", 24, FontStyle.Bold), Brushes.Black, 620, 115);

            // Draw second horizontal line
            g.DrawLine(Pens.Black, 50, 280, 750, 280);

            // Draw barcode
            g.DrawString(_boardingPass.Barcode, new Font("Courier New", 12, FontStyle.Bold), Brushes.Black, 300, 300);

            // Draw additional info
            g.DrawString($"Boarding pass issued: {_boardingPass.IssuedAt:yyyy-MM-dd HH:mm:ss}",
                new Font("Arial", 8, FontStyle.Italic), Brushes.Black, 50, 350);

            g.DrawString("Please be at the boarding gate at least 30 minutes before departure.",
                new Font("Arial", 8, FontStyle.Italic), Brushes.Red, 50, 370);
        }
    }
}