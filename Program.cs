using System;
using System.Data;
using System.Windows.Forms;

namespace KR123
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                SQLClass.conn = new MySql.Data.MySqlClient.MySqlConnection();
                SQLClass.conn.ConnectionString = "Server=localhost;Database=KR123;port=3306;User Id=root";
                SQLClass.conn.Open();
                Application.Run(new Entry());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных, проверьте соединение: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (SQLClass.conn != null && SQLClass.conn.State == ConnectionState.Open)
                {
                    SQLClass.conn.Close();
                }
            }
        }
    }
}