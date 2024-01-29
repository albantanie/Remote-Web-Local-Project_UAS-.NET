using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BCrypt;
using MySql.Data.MySqlClient;

namespace Remote_Web_Local_Project
{
    public partial class AdminMenu : Form
    {
        private MySqlConnection mysqlCon;
        private DataGridView dataGridViewUser;

        public AdminMenu()
        {
            InitializeComponent();
            mysqlCon = new MySqlConnection("server=localhost;user id=root;database=ep");
            this.Load += AdminMenu_Load;
        }

        private void AdminMenu_Load(object sender, EventArgs e)
        {
            try
            {
                mysqlCon.Open();
                string query = "SELECT * FROM users";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, mysqlCon))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    AddDeleteButtonColumn();
                    AddUpdateButtonColumn();
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

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private void AddDeleteButtonColumn()
        {
            DataGridViewButtonColumn btnDeleteColumn = new DataGridViewButtonColumn();
            btnDeleteColumn.Name = "btnDelete";
            btnDeleteColumn.HeaderText = "Hapus";
            btnDeleteColumn.Text = "Hapus";
            btnDeleteColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(btnDeleteColumn);
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
        }

        private void AddUpdateButtonColumn()
        {
            DataGridViewButtonColumn btnUpdateColumn = new DataGridViewButtonColumn();
            btnUpdateColumn.Name = "btnUpdate";
            btnUpdateColumn.HeaderText = "Edit";
            btnUpdateColumn.Text = "Edit";
            btnUpdateColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(btnUpdateColumn);
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                    (dataGridView1.Columns[e.ColumnIndex].Name == "btnDelete" || dataGridView1.Columns[e.ColumnIndex].Name == "btnUpdate"))
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name == "btnDelete")
                    {
                        DialogResult result = MessageBox.Show("Apakah kamu yakin akan menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;

                        int userId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                        DeleteUser(userId);
                        MessageBox.Show("Data berhasil dihapus");
                    }
                    else if (dataGridView1.Columns[e.ColumnIndex].Name == "btnUpdate")
                    {
                        int userId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                        ShowEditForm(userId);
                    }

                    AdminMenu_Load(sender, e);
                }
            }
        }

        private void ShowEditForm(int userId)
        {
            try
            {
                DataGridViewRow selectedRow = dataGridView1.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => r.Cells["id"].Value.ToString().Equals(userId.ToString()))
                    .FirstOrDefault();

                if (selectedRow != null)
                {
                    DataRow selectedRowData = ((DataRowView)selectedRow.DataBoundItem).Row;
                    EditFormUser editForm = new EditFormUser(selectedRowData);
                    editForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Pengguna tidak ditemukan di DataGridView.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DeleteUser(int userId)
        {
            try
            {
                mysqlCon.Open();
                string query = "DELETE FROM users WHERE id = @userId";
                using (MySqlCommand cmd = new MySqlCommand(query, mysqlCon))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error menghapus pengguna: " + ex.Message);
            }
            finally
            {
                mysqlCon.Close();
            }
        }

        private void UpdateUser(int userId)
        {
            // Implementasi logika pembaruan pengguna
            // ...
        }

        private void btnTambahData_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "" || textBoxUsername.Text == "" || textBoxPassword.Text == "")
            {
                MessageBox.Show("Nama, username, dan password harus diisi");
                return;
            }
            else
            {
                string query = "INSERT INTO users (name, username, password, is_admin) VALUES (@name, @username, @password, @is_admin)";
                try
                {
                    mysqlCon.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, mysqlCon))
                    {
                        cmd.Parameters.AddWithValue("@name", textBoxName.Text);
                        cmd.Parameters.AddWithValue("@username", textBoxUsername.Text);
                        cmd.Parameters.AddWithValue("@password", HashPassword(textBoxPassword.Text));
                        cmd.Parameters.AddWithValue("@is_admin", radioButtonUstadz.Checked ? 1 : 0);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data berhasil ditambahkan");
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

        private void btnKeluar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah kamu yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Portal portal = new Portal();
                portal.Show();
                this.Hide();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            AdminMenu_Load(sender, e);
        }
    }
}
