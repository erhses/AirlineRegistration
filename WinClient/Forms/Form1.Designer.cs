namespace WinClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboFlights = new System.Windows.Forms.ComboBox();
            this.lblFlight = new System.Windows.Forms.Label();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.lblFlightStatus = new System.Windows.Forms.Label();
            this.cboFlightStatus = new System.Windows.Forms.ComboBox();
            this.btnUpdateStatus = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblLog = new System.Windows.Forms.Label();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.lblSocketStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnViewFlightStatus = new System.Windows.Forms.Button();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboFlights
            // 
            this.cboFlights.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFlights.FormattingEnabled = true;
            this.cboFlights.Location = new System.Drawing.Point(118, 23);
            this.cboFlights.Name = "cboFlights";
            this.cboFlights.Size = new System.Drawing.Size(180, 23);
            this.cboFlights.TabIndex = 0;
            // 
            // lblFlight
            // 
            this.lblFlight.AutoSize = true;
            this.lblFlight.Location = new System.Drawing.Point(23, 26);
            this.lblFlight.Name = "lblFlight";
            this.lblFlight.Size = new System.Drawing.Size(89, 15);
            this.lblFlight.TabIndex = 1;
            this.lblFlight.Text = "Select Flight:";
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.Location = new System.Drawing.Point(304, 23);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(170, 23);
            this.btnCheckIn.TabIndex = 2;
            this.btnCheckIn.Text = "Passenger Check-In";
            this.btnCheckIn.UseVisualStyleBackColor = true;
            this.btnCheckIn.Click += new System.EventHandler(this.btnCheckIn_Click);
            // 
            // lblFlightStatus
            // 
            this.lblFlightStatus.AutoSize = true;
            this.lblFlightStatus.Location = new System.Drawing.Point(23, 61);
            this.lblFlightStatus.Name = "lblFlightStatus";
            this.lblFlightStatus.Size = new System.Drawing.Size(89, 15);
            this.lblFlightStatus.TabIndex = 3;
            this.lblFlightStatus.Text = "Flight Status:";
            // 
            // cboFlightStatus
            // 
            this.cboFlightStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFlightStatus.FormattingEnabled = true;
            this.cboFlightStatus.Location = new System.Drawing.Point(118, 58);
            this.cboFlightStatus.Name = "cboFlightStatus";
            this.cboFlightStatus.Size = new System.Drawing.Size(180, 23);
            this.cboFlightStatus.TabIndex = 4;
            // 
            // btnUpdateStatus
            // 
            this.btnUpdateStatus.Location = new System.Drawing.Point(304, 58);
            this.btnUpdateStatus.Name = "btnUpdateStatus";
            this.btnUpdateStatus.Size = new System.Drawing.Size(170, 23);
            this.btnUpdateStatus.TabIndex = 5;
            this.btnUpdateStatus.Text = "Update Flight Status";
            this.btnUpdateStatus.UseVisualStyleBackColor = true;
            this.btnUpdateStatus.Click += new System.EventHandler(this.btnUpdateStatus_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(23, 130);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(451, 160);
            this.txtLog.TabIndex = 6;
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Location = new System.Drawing.Point(23, 112);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(127, 15);
            this.lblLog.TabIndex = 7;
            this.lblLog.Text = "System Log Messages:";
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblSocketStatus});
            this.statusStripMain.Location = new System.Drawing.Point(0, 305);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(498, 22);
            this.statusStripMain.TabIndex = 8;
            // 
            // lblSocketStatus
            // 
            this.lblSocketStatus.Name = "lblSocketStatus";
            this.lblSocketStatus.Size = new System.Drawing.Size(135, 17);
            this.lblSocketStatus.Text = "Socket Status: Disconnected";
            this.lblSocketStatus.ForeColor = System.Drawing.Color.Red;
            // 
            // btnViewFlightStatus
            // 
            this.btnViewFlightStatus.Location = new System.Drawing.Point(304, 93);
            this.btnViewFlightStatus.Name = "btnViewFlightStatus";
            this.btnViewFlightStatus.Size = new System.Drawing.Size(170, 23);
            this.btnViewFlightStatus.TabIndex = 9;
            this.btnViewFlightStatus.Text = "View Flight Status Board";
            this.btnViewFlightStatus.UseVisualStyleBackColor = true;
            this.btnViewFlightStatus.Click += new System.EventHandler(this.btnViewFlightStatus_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 327);
            this.Controls.Add(this.btnViewFlightStatus);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnUpdateStatus);
            this.Controls.Add(this.cboFlightStatus);
            this.Controls.Add(this.lblFlightStatus);
            this.Controls.Add(this.btnCheckIn);
            this.Controls.Add(this.lblFlight);
            this.Controls.Add(this.cboFlights);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Airline Registration System";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cboFlights;
        private System.Windows.Forms.Label lblFlight;
        private System.Windows.Forms.Button btnCheckIn;
        private System.Windows.Forms.Label lblFlightStatus;
        private System.Windows.Forms.ComboBox cboFlightStatus;
        private System.Windows.Forms.Button btnUpdateStatus;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel lblSocketStatus;
        private System.Windows.Forms.Button btnViewFlightStatus;
    }
}