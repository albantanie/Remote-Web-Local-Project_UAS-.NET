using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_Web_Local_Project
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
            textBoxPassword.PasswordChar = '*';
        }

        private void btnMasuk_Click(object sender, EventArgs e)
        {

            if (textBoxUsername.Text == "admin" && textBoxPassword.Text == "admin")
            {
                MessageBox.Show("Login Successful");
                AdminMenu adminMenu = new AdminMenu();
                adminMenu.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Login Failed");
            }

        }
    }
}
