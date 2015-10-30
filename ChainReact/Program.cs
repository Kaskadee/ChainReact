using System;
using System.IO;
using System.Windows.Forms;
using ChainReact.Core.Game;

namespace ChainReact
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var game = new MainGame();
            game.Run();
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null) return;
            MessageBox.Show(@"An unhandled UI exception has been occured: " + Environment.NewLine +
                             ex.ToString(), @"Unhandled UI Expception");
            Console.WriteLine(ex.ToString());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(@"An unhandled thread exception has been occured: " + Environment.NewLine +
                            e.Exception.ToString(), @"Unhandled Thread Exception");
            Console.WriteLine(e.Exception.ToString());
        }
    }
}
