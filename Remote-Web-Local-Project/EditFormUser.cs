using System;
using System.Data;
using System.Windows.Forms;
using BCrypt;
using MySql.Data.MySqlClient;

namespace Remote_Web_Local_Project
{
    public partial class EditFormUser : Form
    {
        private MySqlConnection mysqlCon;
        private int userId;
        private DataRow dataRow;

        public EditFormUser(int userId)
        {
            InitializeComponent();
            mysqlCon = new MySqlConnection("server=localhost;user id=root;database=ep");
            this.userId = userId;
            this.Load += EditFormUser_Load;
        }

        public EditFormUser(DataRow dataRow)
        {
            InitializeComponent();
            mysqlCon = new MySqlConnection("server=localhost;user id=root;database=ep");
            this.dataRow = dataRow;
            this.Load += EditFormUser_Load;
        }

        private void EditFormUser_Load(object sender, EventArgs e)
        {
            try
            {
                if (dataRow != null)
                {
                    textBoxName.Text = dataRow["name"].ToString();
                    textBoxUsername.Text = dataRow["username"].ToString();
                    radioButtonUstadz.Checked = Convert.ToInt32(dataRow["is_admin"]) == 1;
                    radioButtonSantri.Checked = Convert.ToInt32(dataRow["is_admin"]) == 0;
                }
                else
                {
                    MessageBox.Show("User data is not provided.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private void btnUpdateData_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "" || textBoxUsername.Text == "")
            {
                MessageBox.Show("Mohon isi semua data");
            }
            else
            {
                string query = "UPDATE users SET name = @name, username = @username, is_admin = @is_admin";

                if (!string.IsNullOrEmpty(textBoxNewPassword.Text))
                {
                    query += ", password = @password";
                }

                query += " WHERE id = @userId";

                try
                {
                    mysqlCon.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, mysqlCon))
                    {
                        cmd.Parameters.AddWithValue("@name", textBoxName.Text);
                        cmd.Parameters.AddWithValue("@username", textBoxUsername.Text);
                        cmd.Parameters.AddWithValue("@is_admin", radioButtonUstadz.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@userId", dataRow["id"]);

                        if (!string.IsNullOrEmpty(textBoxNewPassword.Text))
                        {
                            string hashedPassword = HashPassword(textBoxNewPassword.Text);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                        }

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data berhasil diupdate");
                        AdminMenu adminMenu = new AdminMenu();
                        adminMenu.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace);
                }
                finally
                {
                    mysqlCon.Close();
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            textBoxName.Text = "";
            textBoxUsername.Text = "";
            textBoxNewPassword.Text = "";
            radioButtonUstadz.Checked = false;
            radioButtonSantri.Checked = false;
            radioButtonSantri.Enabled = false;
            radioButtonUstadz.Enabled = false;
        }
    }
}
