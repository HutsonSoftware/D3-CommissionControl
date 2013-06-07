using System;
using System.Windows.Forms;

namespace D3.Commission
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CommissionControl());
        }
    }
}