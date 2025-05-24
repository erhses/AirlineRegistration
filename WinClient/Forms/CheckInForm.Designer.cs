namespace WinClient.Forms
{
    partial class CheckInForm
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
            this.lblFlightInfo = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtPassport = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblPassengerName = new System.Windows.Forms.Label();
            this.btnSelectSeat = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFlightInfo
            // 
            this.lblFlightInfo.AutoSize = true;
            this.lblFlightInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFlightInfo.Location = new System.Drawing.Point(12, 9);
            this.lblFlightInfo.Name = "lblFlightInfo";
            this.lblFlightInfo.Size = new System.Drawing.Size(87, 21);
            this.lblFlightInfo.TabIndex = 0;
            this.lblFlightInfo.Text = "Flight Info";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 40);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 15);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status:";
            // 
            // txtPassport
            // 
            this.txtPassport.Location = new System.Drawing.Point(12, 70);
            this.txtPassport.Name = "txtPassport";
            this.txtPassport.PlaceholderText = "Enter passport number";
            this.txtPassport.Size = new System.Drawing.Size(200, 23);
            this.txtPassport.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(218, 70);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblPassengerName
            // 
            this.lblPassengerName.AutoSize = true;
            this.lblPassengerName.Location = new System.Drawing.Point(12, 110);
            this.lblPassengerName.Name = "lblPassengerName";
            this.lblPassengerName.Size = new System.Drawing.Size(0, 15);
            this.lblPassengerName.TabIndex = 4;
            // 
            // btnSelectSeat
            // 
            this.btnSelectSeat.Enabled = false;
            this.btnSelectSeat.Location = new System.Drawing.Point(12, 150);
            this.btnSelectSeat.Name = "btnSelectSeat";
            this.btnSelectSeat.Size = new System.Drawing.Size(281, 30);
            this.btnSelectSeat.TabIndex = 5;
            this.btnSelectSeat.Text = "Select Seat";
            this.btnSelectSeat.UseVisualStyleBackColor = true;
            this.btnSelectSeat.Click += new System.EventHandler(this.btnSelectSeat_Click);
            // 
            // CheckInForm
            // 
            this.ClientSize = new System.Drawing.Size(305, 200);
            this.Controls.Add(this.btnSelectSeat);
            this.Controls.Add(this.lblPassengerName);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtPassport);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblFlightInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckInForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Passenger Check-In";
            this.Load += new System.EventHandler(this.CheckInForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private Label lblFlightInfo;
        private Label lblStatus;
        private TextBox txtPassport;
        private Button btnSearch;
        private Label lblPassengerName;
        private Button btnSelectSeat;
        #endregion
    }
}