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
        private double Ve, τd, Ed, Td, Tmin, Tmax, ΔT, Qmin, Qmax, ΔQ;

        public Сhart(double ve,double τd,double ed,double td,double tmin,double tmax,double Δt,double qmin,double qmax,double Δq, CheckAdmin user)
        {
            Ve = ve;
            this.τd = τd;
            Ed = ed;
            Td = td;
            Tmin = tmin;
            Tmax = tmax;
            this.ΔT = Δt;
            Qmin = qmin;
            Qmax = qmax;
            ΔQ = Δq;
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
            while (T <= Tmax)
            {
                LineSeries series = new LineSeries
                {
                    Title = $"T = {T} °C",
                    Values = new ChartValues<double>(),
                };

                double Q = Qmin;
                while (Q <= Qmax)
                {
                    double destructionIndex = CalculateDestructionIndex(Q, T);
                    series.Values.Add(destructionIndex);

                    Q += ΔQ;
                }

                seriesCollection1.Add(series);

                T += ΔT;
            }

            cartesianChart1.AxisX.Add(new Axis { Title = "Расход потока, л/мин", Labels = GenerateLabels(Qmin, Qmax, ΔQ) });
            cartesianChart1.AxisY.Add(new Axis { Title = "Индекс термической деструкции, %" });

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

            double Q = Qmin;
            while (Q <= Qmax)
            {
                LineSeries series = new LineSeries
                {
                    Title = $"Q = {Q} л/мин",
                    Values = new ChartValues<double>(),
                };

                double T = Tmin;
                while (T <= Tmax)
                {
                    double destructionIndex = CalculateDestructionIndex(Q, T);
                    series.Values.Add(destructionIndex);

                    T += ΔT;
                }

                seriesCollection2.Add(series);
                Q += ΔQ;
            }

            cartesianChart2.AxisX.Add(new Axis { Title = "Температура, °C", Labels = GenerateLabels(Tmin, Tmax, ΔT) });
            cartesianChart2.AxisY.Add(new Axis { Title = "Индекс термической деструкции, %" });

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

        private double CalculateDestructionIndex(double Q, double T)
        {
            double index = (Ve / (τd * Q)) * Math.Exp(Ed / (8.31 * (T + 273.15) * (Td + 273.15)) * (T - Td)) * 100;

            return Math.Round(index, 3);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ShowMatrixForm();
        }

        private void ShowMatrixForm()
        {
            DataTable matrixTable = new DataTable();
            matrixTable.Columns.Add("Q, л/мин");

            for (double T = Tmin; T <= Tmax; T += ΔT)
            {
                matrixTable.Columns.Add($"T={T}°C");
            }

            for (double Q = Qmin; Q <= Qmax; Q += ΔQ)
            {
                DataRow row = matrixTable.NewRow();
                row["Q, л/мин"] = Q;

                for (double T = Tmin; T <= Tmax; T += ΔT)
                {
                    double destructionIndex = CalculateDestructionIndex(Q, T);
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
