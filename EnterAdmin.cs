using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Windows.Forms;
using System.Data;
using DocumentFormat.OpenXml.Bibliography;

namespace KR123
{
    public partial class EnterAdmin : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        private string searchQuery = "";
        public EnterAdmin(CheckAdmin user)
        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += ApplicationManager.HandleFormClosing;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadUsersData();
        }

        private void EnterAdmin_Load(object sender, System.EventArgs e)
        {
            LoadUsersData();
            dataGridView1.CellClick += dataGridView1_CellClick;
        }
        private void LoadUsersData()
        {
            string query = searchQuery != "" ? searchQuery :
            "SELECT users.id, users.login, users.password, users.is_admin " +
            "FROM users ";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["login"].HeaderText = "Логин";
            dataGridView1.Columns["password"].Visible = false;
            dataGridView1.Columns["is_admin"].HeaderText = "Администратор?";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                string login = selectedRow.Cells["login"].Value.ToString();
                string password = selectedRow.Cells["password"].Value.ToString();
                bool is_admin = Convert.ToBoolean(selectedRow.Cells["is_admin"].Value);
                textBox1.Text = login;
                textBox2.Text = password;
                comboBox1.SelectedIndex = (is_admin) ? comboBox1.FindStringExact("да") : comboBox1.FindStringExact("нет");
            }
        }
        private bool IsNameExists(string login)
        {
            string query = $"SELECT COUNT(*) FROM users WHERE login = '{login}'";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private bool IsName1Exists(string login, int userId)
        {
            string query = $"SELECT COUNT(*) FROM users WHERE login = '{login}' AND id <> {userId}";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string newLogin = textBox1.Text;
            string newPass = textBox2.Text;
            string selectedAdminText = comboBox1.SelectedItem.ToString();

            int newAdmin = (selectedAdminText.ToLower() == "да") ? 1 : 0;

            if (string.IsNullOrWhiteSpace(newLogin))
            {
                MessageBox.Show("Ошибка, поле для имени пустое!");
                return;
            }

            if (IsNameExists(newLogin))
            {
                MessageBox.Show("Ошибка, запись с таким логином уже существует!");
                return;
            }

            string hashedPassword = ComputeSha256Hash(newPass);

            string query2 = $"INSERT INTO users (login, password, is_admin) VALUES ('{newLogin}', '{hashedPassword}', '{newAdmin}')";
            MySqlCommand cmd = new MySqlCommand(query2, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                LoadUsersData();
            }
            else
            {
                MessageBox.Show("Ошибка, данные не внесены!");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                string newLogin = textBox1.Text;
                string newPass = textBox2.Text;
                string selectedAdminText = comboBox1.SelectedItem.ToString();

                int newAdmin = (selectedAdminText.ToLower() == "да") ? 1 : 0;

                if (string.IsNullOrWhiteSpace(newLogin))
                {
                    MessageBox.Show("Ошибка, поле для имени пустое!");
                    return;
                }

                if (IsName1Exists(newLogin, selectedId))
                {
                    MessageBox.Show("Ошибка, запись с таким логином уже существует!");
                    return;
                }

                string hashedPassword = ComputeSha256Hash(newPass);

                string query3 = $"UPDATE users SET login = '{newLogin}', password = '{hashedPassword}', is_admin = '{newAdmin}' WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query3, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadUsersData();
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }
            }
            else
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string selectedId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();

                string query4 = $"DELETE FROM users WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query4, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadUsersData();
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }
            }
            else
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Entry form = (Entry)Application.OpenForms["Entry"];
            if (form != null)
            {
                form.Show();
            }
            Hide();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            searchQuery = $"SELECT id, login, password, is_admin " +
                $"FROM users " +
                $"WHERE login LIKE '%{searchTerm}%'";

            LoadUsersData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            EditBaza rf = new EditBaza(_user);
            rf.userLogin = _user.Login;
            Hide();
            rf.Show();
            rf.Activate();
        }
    }
}
