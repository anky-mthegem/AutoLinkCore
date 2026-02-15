using System;
using System.Windows.Forms;

namespace AutoLinkCore
{
    public partial class EventLogControl : UserControl
    {
        private ListBox lstEvents;
        private Label lblEventCount;
        private Button btnClear;
        private DiagnosticEventLog _eventLog;

        public EventLogControl()
        {
            InitializeComponent();
            AppTheme.ApplyTheme(this);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Event list
            lstEvents = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 9F),
                BackColor = AppTheme.BackgroundWhite,
                ForeColor = AppTheme.ForegroundDark
            };
            this.Controls.Add(lstEvents);

            // Bottom panel for controls
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = AppTheme.BackgroundGray
            };
            this.Controls.Add(bottomPanel);

            // Event count label
            lblEventCount = new Label
            {
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true,
                Text = "Events: 0",
                Font = new System.Drawing.Font("Segoe UI", 9F),
                ForeColor = AppTheme.ForegroundDark
            };
            bottomPanel.Controls.Add(lblEventCount);

            // Clear button
            btnClear = new Button
            {
                Location = new System.Drawing.Point(this.Width - 110, 8),
                Size = new System.Drawing.Size(100, 25),
                Text = "Clear Log",
                BackColor = AppTheme.AccentBlue,
                ForeColor = System.Drawing.Color.White,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnClear.Click += (s, e) => ClearLog();
            bottomPanel.Controls.Add(btnClear);

            this.ResumeLayout();
        }

        public void SetEventLog(DiagnosticEventLog eventLog)
        {
            _eventLog = eventLog;
            _eventLog.EventAdded += (s, e) => AddEventToList(e);
            RefreshEventList();
        }

        private void AddEventToList(DiagnosticEvent diagEvent)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<DiagnosticEvent>(AddEventToList), diagEvent);
                return;
            }

            string displayText = diagEvent.ToString();
            lstEvents.Items.Insert(0, displayText); // Add to top

            // Limit to last 500 items for performance
            if (lstEvents.Items.Count > 500)
            {
                lstEvents.Items.RemoveAt(lstEvents.Items.Count - 1);
            }

            UpdateEventCount();
        }

        private void RefreshEventList()
        {
            if (_eventLog == null)
                return;

            lstEvents.Items.Clear();
            var events = _eventLog.GetRecentEvents(100);

            foreach (var evt in events)
            {
                lstEvents.Items.Insert(0, evt.ToString());
            }

            UpdateEventCount();
        }

        private void UpdateEventCount()
        {
            lblEventCount.Text = $"Events: {lstEvents.Items.Count}";
        }

        private void ClearLog()
        {
            if (_eventLog != null)
            {
                _eventLog.Clear();
            }
            lstEvents.Items.Clear();
            UpdateEventCount();
            AppLogger.Information("Event log cleared by user");
        }
    }
}
