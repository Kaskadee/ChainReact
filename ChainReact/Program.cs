using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using ChainReact.Extensions;

namespace ChainReact
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(new FrmLoading());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null) return;
            if (!Debugger.IsAttached)
            {
                MessageBox.Show(@"An unhandled UI exception has been occured: " + Environment.NewLine +
                                ex.Message + Environment.NewLine + ex.ToShortString(), @"Unhandled UI Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.ToString());
            }
            else if (ex.Message.Contains("OpenGL 3.3"))
            {
                MessageBox.Show(@"Failed to create OpenGL 3.3 Context. If you are debugging try restarting the application.", @"Visual Studio - OpenGL 3.3 Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.Start(Assembly.GetExecutingAssembly().Location, "Debugger Enabled");
                Environment.Exit(-1);
            }

        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            MessageBox.Show(@"An unhandled thread exception has been occured: " + Environment.NewLine +
                            ex.Message + Environment.NewLine + ex.ToShortString(), @"Unhandled Thread Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.WriteLine(e.Exception.ToString());
        }
    }
}
