using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KR123
{
    public partial class EnterUser : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        public EnterUser(CheckAdmin user)
        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            FormClosing += ApplicationManager.HandleFormClosing;

            textBox1.KeyPress += textBox_KeyPress;
            textBox2.KeyPress += textBox_KeyPress;
            textBox3.KeyPress += textBox_KeyPress;
            textBox4.KeyPress += textBox_KeyPress;
            textBox5.KeyPress += textBox_KeyPress;
            textBox6.KeyPress += textBox_KeyPress;
            textBox7.KeyPress += textBox_KeyPress;
            textBox8.KeyPress += textBox_KeyPress;
            textBox9.KeyPress += textBox_KeyPress;
            textBox10.KeyPress += textBox_KeyPress;
        }

        private void EnterUser_Load(object sender, EventArgs e)
        {
            Polimer();
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;

                comboBox1_SelectedIndexChanged(sender, e);
            }

        
            textBox5.Text = "175"; //Tmin
            textBox6.Text = "205"; //Tmax
            textBox7.Text = "2"; //ΔT
            textBox8.Text = "30"; //Ymin
            textBox9.Text = "60"; //Ymax
            textBox10.Text = "2"; //ΔY
        }

        private void Polimer()
        {
            comboBox1.Items.Clear();

            List<string> type = SQLClass.Select("SELECT type_mat FROM Polimer_materials ORDER BY id;");
            comboBox1.Items.AddRange(type.ToArray());
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTypeMat = comboBox1.SelectedItem.ToString();

            string matId = SQLClass.Select(
                $"SELECT id FROM Polimer_materials WHERE type_mat = '{selectedTypeMat}';").FirstOrDefault();

            PopulateTextBoxes(matId, new[] { "3", "4", "5", "6" });
        }

        private void PopulateTextBoxes(string matId, string[] coefMatModelIds)
        {
            var textBoxList = new List<TextBox> { textBox1, textBox2, textBox3, textBox4 };

            if (!string.IsNullOrEmpty(matId))
            {
                for (int i = 0; i < coefMatModelIds.Length; i++)
                {
                    string selectedValue = SQLClass.Select($"SELECT values_coef FROM Values_coef_model WHERE coef_mat_model_id = '{coefMatModelIds[i]}' AND polimer_mat_id = '{matId}';").FirstOrDefault();

                    textBoxList[i].Text = selectedValue;
                }
            }
        }

        public string nVal => textBox4.Text;
        public string tZeroVal => textBox3.Text;
        public string bVal => textBox2.Text;
        public string muZeroVal => textBox1.Text;
        public string TminVal => textBox5.Text;
        public string TmaxVal => textBox6.Text;
        public string ΔTVal => textBox7.Text;
        public string YminVal => textBox8.Text;
        public string YmaxVal => textBox9.Text;
        public string ΔYVal => textBox10.Text;
        public string mat => comboBox1.SelectedItem as string;

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nVal) ||
        string.IsNullOrWhiteSpace(tZeroVal) ||
        string.IsNullOrWhiteSpace(bVal) ||
        string.IsNullOrWhiteSpace(muZeroVal) ||
        string.IsNullOrWhiteSpace(TminVal) ||
        string.IsNullOrWhiteSpace(TmaxVal) ||
        string.IsNullOrWhiteSpace(ΔTVal) ||
        string.IsNullOrWhiteSpace(YminVal) ||
        string.IsNullOrWhiteSpace(YmaxVal) ||
        string.IsNullOrWhiteSpace(ΔYVal))
            {
                MessageBox.Show("Заполните все значения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Сhart chartForm = new Сhart(
                       double.Parse(nVal),
                       double.Parse(tZeroVal),
                       double.Parse(bVal),
                       double.Parse(muZeroVal),
                       double.Parse(TminVal),
                       double.Parse(TmaxVal),
                       double.Parse(ΔTVal),
                       double.Parse(YminVal),
                       double.Parse(YmaxVal),
                       double.Parse(ΔYVal),
                       _user,
                       mat
               );
                chartForm.Show();

                this.Hide();
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Entry form = (Entry)Application.OpenForms["Entry"];
            if (form != null)
            {
                form.Show();
            }
                Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
