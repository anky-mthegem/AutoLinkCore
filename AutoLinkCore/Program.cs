using System;
using System.Windows.Forms;

namespace AutoLinkCore
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MainForm());
            }
        }
    }
}

