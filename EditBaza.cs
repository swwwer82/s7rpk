using DocumentFormat.OpenXml.Office.CustomXsn;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace KR123
{
    public partial class EditBaza : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        private string selectedId = string.Empty;
        public EditBaza(CheckAdmin user)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _user = user;
            FormClosing += ApplicationManager.HandleFormClosing;
        }

        private void EditBaza_Load(object sender, EventArgs e)
        {
            dataGridView1.CellClick += dataGridView1_CellClick;
            LoadData();
        }

        private void LoadData()
        {
            string query = $@"SELECT id, type_mat FROM Polimer_materials";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns["type_mat"].HeaderText = "Тим материала";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                string clickedId = selectedRow.Cells["id"].Value.ToString();

                // Проверяем, выбрана ли уже эта запись
                if (clickedId != selectedId)
                {
                    selectedId = clickedId;
                    string name = selectedRow.Cells["type_mat"].Value.ToString();

                    textBox1.Text = name;
                    string matId = SQLClass.Select(
                    $"SELECT id FROM Polimer_materials WHERE type_mat = '{name}';").FirstOrDefault();

                    PopulateTextBoxes(matId, new[] { "3", "4", "5" });
                }
            }
        }

        private void PopulateTextBoxes(string matId, string[] coefMatModelIds)
        {
            var textBoxList = new List<TextBox> { textBox2, textBox3, textBox4 };

            if (!string.IsNullOrEmpty(matId))
            {
                for (int i = 0; i < coefMatModelIds.Length; i++)
                {
                    string selectedValue = SQLClass.Select($"SELECT values_coef FROM Values_coef_model WHERE coef_mat_model_id = '{coefMatModelIds[i]}' AND polimer_mat_id = '{matId}';").FirstOrDefault();

                    textBoxList[i].Text = selectedValue;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newType = textBox1.Text;
            string v3 = textBox2.Text;
            string v4 = textBox3.Text;
            string v5 = textBox4.Text;

            if (string.IsNullOrWhiteSpace(newType) || string.IsNullOrWhiteSpace(v3) || string.IsNullOrWhiteSpace(v4) || string.IsNullOrWhiteSpace(v5))
            {
                MessageBox.Show("Ошибка, одно поле или несколько пустое!");
                return;
            }

            string query1 = $"INSERT INTO Polimer_materials (type_mat) VALUES ('{newType}')";
            MySqlCommand cmd1 = new MySqlCommand(query1, SQLClass.conn);
            cmd1.ExecuteNonQuery(); 

            string matId = SQLClass.Select($"SELECT id FROM Polimer_materials WHERE type_mat = '{newType}';").FirstOrDefault();

            string query2 = $"INSERT INTO Values_coef_model (polimer_mat_id, coef_mat_model_id, values_coef) VALUES ('{matId}', '3', '{v3}')";
            MySqlCommand cmd2 = new MySqlCommand(query2, SQLClass.conn);
            cmd2.ExecuteNonQuery();

            string query3 = $"INSERT INTO Values_coef_model (polimer_mat_id, coef_mat_model_id, values_coef) VALUES ('{matId}', '4', '{v4}')";
            MySqlCommand cmd3 = new MySqlCommand(query3, SQLClass.conn);
            cmd3.ExecuteNonQuery();

            string query4 = $"INSERT INTO Values_coef_model (polimer_mat_id, coef_mat_model_id, values_coef) VALUES ('{matId}', '5', '{v5}')";
            MySqlCommand cmd4 = new MySqlCommand(query4, SQLClass.conn);
            cmd4.ExecuteNonQuery();
            
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string selectedId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();
                string newType = textBox1.Text;
                string v3 = textBox2.Text;
                string v4 = textBox3.Text;
                string v5 = textBox4.Text;

                if (string.IsNullOrWhiteSpace(newType) || string.IsNullOrWhiteSpace(v3) || string.IsNullOrWhiteSpace(v4) || string.IsNullOrWhiteSpace(v5))
                {
                    MessageBox.Show("Ошибка, одно поле или несколько пустое!");
                    return;
                }


                string query1 = $"UPDATE Polimer_materials SET type_mat = '{newType}' WHERE id = {selectedId}";
                MySqlCommand cmd1 = new MySqlCommand(query1, SQLClass.conn);
                cmd1.ExecuteNonQuery();

                string query2 = $"UPDATE Values_coef_model SET values_coef = '{v3}' WHERE polimer_mat_id = {selectedId}";
                MySqlCommand cmd2 = new MySqlCommand(query2, SQLClass.conn);
                cmd2.ExecuteNonQuery();

                string query3 = $"UPDATE Values_coef_model SET values_coef = ' {v4} ' WHERE polimer_mat_id =  {selectedId}";
                MySqlCommand cmd3 = new MySqlCommand(query3, SQLClass.conn);
                cmd3.ExecuteNonQuery();

                string query4 = $"UPDATE Values_coef_model SET values_coef = '{v5}' WHERE polimer_mat_id = {selectedId}";
                MySqlCommand cmd4 = new MySqlCommand(query4, SQLClass.conn);
                cmd4.ExecuteNonQuery();

                LoadData();
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

                string query1 = $"DELETE FROM Polimer_materials WHERE id = {selectedId}";
                MySqlCommand cmd1 = new MySqlCommand(query1, SQLClass.conn);

                string query2 = $"DELETE FROM Values_coef_model WHERE polimer_mat_id = {selectedId}";
                MySqlCommand cmd2 = new MySqlCommand(query2, SQLClass.conn);

                if (cmd1.ExecuteNonQuery() == 1 || cmd2.ExecuteNonQuery() == 1)
                {
                    LoadData();
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
            EnterAdmin rf = new EnterAdmin(_user);
            rf.userLogin = _user.Login;
            rf.Show();
            rf.Activate();
            Hide();
        }
    }
}
