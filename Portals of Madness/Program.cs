using System;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Controller Controller = new Controller();
            Application.Run(new MenuForm(Controller));
        }
    }
}
