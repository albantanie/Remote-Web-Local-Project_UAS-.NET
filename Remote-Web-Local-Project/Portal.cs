using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Remote_Web_Local_Project
{
    public partial class Portal : Form
    {
        private MySqlConnection mysqlCon;

        public Portal()
        {
            InitializeComponent();
            mysqlCon = new MySqlConnection("server=localhost;user id=root;database=ep");
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Admin admin = new Admin();
            admin.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ustadz ustadz = new Ustadz();
            ustadz.Show();
            this.Hide();
        }

        private void button3_click_1(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Santri santri = new Santri();
            santri.Show();
            this.Hide();
        }
    }
}
