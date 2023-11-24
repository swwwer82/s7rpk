using System.Windows.Forms;

namespace KR123
{
    public class ApplicationManager
    {
        public static void HandleFormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
