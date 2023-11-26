using System;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace KR123
{
    public partial class Сhart : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        private double n, TZero, b, muZero, Tmin, Tmax, ΔT, Ymin, Ymax, ΔY;

        public Сhart(double n,double TZero,double b,double muZero,double tmin,double tmax,double Δt,double ymin,double ymax,double Δy, CheckAdmin user)
        {
            this.n = n;
            this.TZero = TZero;
            this.b = b;
            this.muZero = muZero;
            Tmin = tmin;
            Tmax = tmax;
            this.ΔT = Δt;
            Ymin = ymin;
            Ymax = ymax;
            ΔY = Δy;
            _user = user;

            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += ApplicationManager.HandleFormClosing;

            SplitContainer spltContainer = new SplitContainer();
            spltContainer.Dock = DockStyle.Fill;
            spltContainer.Orientation = Orientation.Vertical;
            this.Controls.Add(spltContainer);

            InitializeChart1(spltContainer.Panel1);
            InitializeChart2(spltContainer.Panel2);

            spltContainer.SplitterDistance = this.Width / 2;  
            this.Load += (s, e) => { spltContainer.SplitterDistance = this.Width / 2; };
        }

        private void InitializeChart1(Panel panel)
        {
            var cartesianChart1 = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Fill
            };

            SeriesCollection seriesCollection1 = new SeriesCollection();

            double T = Tmin;
            for (int i = 0; i<3; i++)
            {
                LineSeries series = new LineSeries
                {
                    Title = $"T = {T} °C",
                    Values = new ChartValues<double>(),
                    Fill = System.Windows.Media.Brushes.Transparent
                };

                double Y = Ymin;
                while (Y <= Ymax)
                {
                    double destructionIndex = CalculateDestructionIndex(Y, T);
                    series.Values.Add(destructionIndex);

                    Y += ΔY;
                }

                seriesCollection1.Add(series);

                T += (Tmax - Tmin) / 2;
            }

            cartesianChart1.AxisX.Add(new Axis { Title = "Скорость деформации, 1/с", Labels = GenerateLabels(Ymin, Ymax, ΔY) });
            cartesianChart1.AxisY.Add(new Axis { Title = "Вязкость экструданта, Па*с" });

            cartesianChart1.Series = seriesCollection1;

            panel.Controls.Add(cartesianChart1);
        }

        private void InitializeChart2(Panel panel)
        {
            var cartesianChart2 = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Fill
            };

            SeriesCollection seriesCollection2 = new SeriesCollection();

            double Y = Ymin;
            for (int i = 0; i < 3; i++)
            {
                LineSeries series = new LineSeries
                {
                    Title = $"Y = {Y} 1/с",
                    Values = new ChartValues<double>(),
                    Fill = System.Windows.Media.Brushes.Transparent
                };

                double T = Tmin;
                while (T <= Tmax)
                {
                    double destructionIndex = CalculateDestructionIndex(Y, T);
                    series.Values.Add(destructionIndex);

                    T += ΔT;
                }

                seriesCollection2.Add(series);
                Y += (Ymax - Ymin) / 2;
            }

            cartesianChart2.AxisX.Add(new Axis { Title = "Температура, °C", Labels = GenerateLabels(Tmin, Tmax, ΔT) });
            cartesianChart2.AxisY.Add(new Axis { Title = "Вязкость экструданта, Па*с" });

            cartesianChart2.Series = seriesCollection2;

            panel.Controls.Add(cartesianChart2);
        }

        private string[] GenerateLabels(double min, double max, double step)
        {
            int count = (int)((max - min) / step) + 1;
            string[] labels = new string[count];
            double current = min;

            for (int i = 0; i < count; i++)
            {
                labels[i] = $"{current}";
                current += step;
            }

            return labels;
        }

        private double CalculateDestructionIndex(double Y, double T)
        {
            double index = muZero * Math.Exp(-b * (T - TZero) * Math.Pow(Y, n - 1));

            return Math.Round(index, 3);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ShowMatrixForm();
        }

        private void ShowMatrixForm()
        {
            DataTable matrixTable = new DataTable();
            matrixTable.Columns.Add("Y, 1/c");

            for (double T = Tmin; T <= Tmax; T += ΔT)
            {
                matrixTable.Columns.Add($"T={T}°C");
            }

            for (double Y = Ymin; Y <= Ymax; Y += ΔY)
            {
                DataRow row = matrixTable.NewRow();
                row["Y, 1/c"] = Y;

                for (double T = Tmin; T <= Tmax; T += ΔT)
                {
                    double destructionIndex = CalculateDestructionIndex(Y, T);
                    row[$"T={T}°C"] = destructionIndex;
                }

                matrixTable.Rows.Add(row);
            }

            ShowDataTableForm(matrixTable);
        }

        private void ShowDataTableForm(DataTable table)
        {
            DataTableForm dataTableForm = new DataTableForm(table);
            dataTableForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EnterUser rf = new EnterUser(_user);
            rf.userLogin = _user.Login;
            rf.Show();
            rf.Activate();

            this.Hide();
        }
    }
}
