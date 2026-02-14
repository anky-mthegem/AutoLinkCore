using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AutoLinkCore
{
    public partial class AnalogClockControl : UserControl
    {
        private Timer timer;
        
        public AnalogClockControl()
        {
            InitializeComponent();
            
            this.DoubleBuffered = true;
            this.BackColor = AppTheme.BackgroundLight;
            
            timer = new Timer();
            timer.Interval = 1000; // Update every second
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invalidate(); // Trigger repaint
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Calculate center and radius
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            int radius = Math.Min(centerX, centerY) - 20;
            
            // Draw clock face
            using (Pen circlePen = new Pen(Color.FromArgb(60, 60, 60), 4))
            {
                g.DrawEllipse(circlePen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }
            
            // Draw hour markers
            for (int i = 0; i < 12; i++)
            {
                double angle = (i * 30) * Math.PI / 180;
                int x1 = (int)(centerX + (radius - 15) * Math.Sin(angle));
                int y1 = (int)(centerY - (radius - 15) * Math.Cos(angle));
                int x2 = (int)(centerX + radius * Math.Sin(angle));
                int y2 = (int)(centerY - radius * Math.Cos(angle));
                
                using (Pen markerPen = new Pen(Color.FromArgb(80, 80, 80), 3))
                {
                    g.DrawLine(markerPen, x1, y1, x2, y2);
                }
            }
            
            // Draw minute markers
            for (int i = 0; i < 60; i++)
            {
                if (i % 5 != 0) // Skip hour markers
                {
                    double angle = (i * 6) * Math.PI / 180;
                    int x1 = (int)(centerX + (radius - 8) * Math.Sin(angle));
                    int y1 = (int)(centerY - (radius - 8) * Math.Cos(angle));
                    int x2 = (int)(centerX + radius * Math.Sin(angle));
                    int y2 = (int)(centerY - radius * Math.Cos(angle));
                    
                    using (Pen markerPen = new Pen(Color.FromArgb(150, 150, 150), 1))
                    {
                        g.DrawLine(markerPen, x1, y1, x2, y2);
                    }
                }
            }
            
            DateTime now = DateTime.Now;
            
            // Draw hour hand
            double hourAngle = ((now.Hour % 12) + now.Minute / 60.0) * 30 * Math.PI / 180;
            int hourX = (int)(centerX + (radius * 0.5) * Math.Sin(hourAngle));
            int hourY = (int)(centerY - (radius * 0.5) * Math.Cos(hourAngle));
            using (Pen hourPen = new Pen(Color.FromArgb(40, 40, 40), 8))
            {
                hourPen.StartCap = LineCap.Round;
                hourPen.EndCap = LineCap.Round;
                g.DrawLine(hourPen, centerX, centerY, hourX, hourY);
            }
            
            // Draw minute hand
            double minuteAngle = (now.Minute + now.Second / 60.0) * 6 * Math.PI / 180;
            int minuteX = (int)(centerX + (radius * 0.7) * Math.Sin(minuteAngle));
            int minuteY = (int)(centerY - (radius * 0.7) * Math.Cos(minuteAngle));
            using (Pen minutePen = new Pen(Color.FromArgb(60, 60, 60), 6))
            {
                minutePen.StartCap = LineCap.Round;
                minutePen.EndCap = LineCap.Round;
                g.DrawLine(minutePen, centerX, centerY, minuteX, minuteY);
            }
            
            // Draw second hand
            double secondAngle = now.Second * 6 * Math.PI / 180;
            int secondX = (int)(centerX + (radius * 0.8) * Math.Sin(secondAngle));
            int secondY = (int)(centerY - (radius * 0.8) * Math.Cos(secondAngle));
            using (Pen secondPen = new Pen(AppTheme.ErrorRed, 2))
            {
                secondPen.StartCap = LineCap.Round;
                secondPen.EndCap = LineCap.Round;
                g.DrawLine(secondPen, centerX, centerY, secondX, secondY);
            }
            
            // Draw center dot
            using (SolidBrush centerBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
            {
                g.FillEllipse(centerBrush, centerX - 8, centerY - 8, 16, 16);
            }
            
            // Draw digital time below clock
            string timeString = now.ToString("HH:mm:ss");
            using (Font timeFont = new Font("Segoe UI", 24, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
            {
                SizeF textSize = g.MeasureString(timeString, timeFont);
                g.DrawString(timeString, timeFont, textBrush, 
                    centerX - textSize.Width / 2, centerY + radius + 30);
            }
            
            // Draw date
            string dateString = now.ToString("dddd, MMMM dd, yyyy");
            using (Font dateFont = new Font("Segoe UI", 12))
            using (SolidBrush dateBrush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                SizeF dateSize = g.MeasureString(dateString, dateFont);
                g.DrawString(dateString, dateFont, dateBrush, 
                    centerX - dateSize.Width / 2, centerY + radius + 70);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
