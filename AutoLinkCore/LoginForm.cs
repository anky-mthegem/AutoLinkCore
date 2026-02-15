using System;
using System.Windows.Forms;

namespace AutoLinkCore
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            AppTheme.ApplyTheme(this);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "admin" && txtPassword.Text == "123456")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid ID or Password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }
    }
}
