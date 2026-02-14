using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoLinkCore
{
    public static class AppTheme
    {
        // Light Modern Theme Colors
        public static readonly Color BackgroundLight = Color.FromArgb(245, 245, 245);
        public static readonly Color BackgroundWhite = Color.White;
        public static readonly Color BackgroundGray = Color.FromArgb(230, 230, 230);
        public static readonly Color ForegroundDark = Color.FromArgb(33, 33, 33);
        public static readonly Color AccentBlue = Color.FromArgb(0, 120, 215);
        public static readonly Color AccentBlueHover = Color.FromArgb(0, 95, 184);
        public static readonly Color BorderColor = Color.FromArgb(200, 200, 200);
        public static readonly Color SuccessGreen = Color.FromArgb(16, 124, 16);
        public static readonly Color ErrorRed = Color.FromArgb(232, 17, 35);
        public static readonly Color ShadowColor = Color.FromArgb(220, 220, 220);
        
        // Font
        public static readonly Font DefaultFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font HeaderFont = new Font("Segoe UI", 16F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 9.75F, FontStyle.Regular);
        public static readonly Font TitleFont = new Font("Segoe UI", 20F, FontStyle.Regular);
        
        public static void ApplyTheme(Control control)
        {
            control.BackColor = BackgroundLight;
            control.ForeColor = ForegroundDark;
            control.Font = DefaultFont;
            
            if (control is Form)
            {
                Form form = (Form)control;
                form.BackColor = BackgroundLight;
            }
            else if (control is Button)
            {
                Button button = (Button)control;
                button.BackColor = AccentBlue;
                button.ForeColor = Color.White;
                button.Font = ButtonFont;
                button.Cursor = Cursors.Hand;
                
                button.MouseEnter += Button_MouseEnter;
                button.MouseLeave += Button_MouseLeave;
            }
            else if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.BackColor = BackgroundWhite;
                textBox.ForeColor = ForegroundDark;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = DefaultFont;
            }
            else if (control is Label)
            {
                Label label = (Label)control;
                label.BackColor = Color.Transparent;
                label.ForeColor = ForegroundDark;
            }
            else if (control is Panel)
            {
                Panel panel = (Panel)control;
                panel.BackColor = BackgroundWhite;
            }
            else if (control is MenuStrip)
            {
                MenuStrip menu = (MenuStrip)control;
                menu.BackColor = BackgroundWhite;
                menu.ForeColor = ForegroundDark;
                menu.Renderer = new ModernMenuRenderer();
            }
            else if (control is ProgressBar)
            {
                ProgressBar progressBar = (ProgressBar)control;
                progressBar.BackColor = BackgroundGray;
            }
            
            foreach (Control child in control.Controls)
            {
                ApplyTheme(child);
            }
        }
        
        private static void Button_MouseEnter(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = AccentBlueHover;
        }
        
        private static void Button_MouseLeave(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = AccentBlue;
        }
    }
    
    public class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        public ModernMenuRenderer() : base(new ModernColorTable()) { }
    }
    
    public class ModernColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected 
        { 
            get { return AppTheme.BackgroundGray; }
        }
        
        public override Color MenuItemSelectedGradientBegin 
        { 
            get { return AppTheme.BackgroundGray; }
        }
        
        public override Color MenuItemSelectedGradientEnd 
        { 
            get { return AppTheme.BackgroundGray; }
        }
        
        public override Color MenuItemBorder 
        { 
            get { return AppTheme.BorderColor; }
        }
        
        public override Color MenuBorder 
        { 
            get { return AppTheme.BorderColor; }
        }
        
        public override Color MenuItemPressedGradientBegin 
        { 
            get { return AppTheme.AccentBlue; }
        }
        
        public override Color MenuItemPressedGradientEnd 
        { 
            get { return AppTheme.AccentBlue; }
        }
        
        public override Color ToolStripDropDownBackground 
        { 
            get { return AppTheme.BackgroundWhite; }
        }
        
        public override Color ImageMarginGradientBegin 
        { 
            get { return AppTheme.BackgroundWhite; }
        }
        
        public override Color ImageMarginGradientMiddle 
        { 
            get { return AppTheme.BackgroundWhite; }
        }
        
        public override Color ImageMarginGradientEnd 
        { 
            get { return AppTheme.BackgroundWhite; }
        }
    }
}
