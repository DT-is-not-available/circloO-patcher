using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cpatcher
{
    internal static class Program
    {
        public static string? GetExecutableDirectory()
        {
            return Path.GetDirectoryName(Environment.ProcessPath);
        }

        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(GlobalThreadExceptionHandler);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalUnhandledExceptionHandler);

            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindow());
        }

        private static void GlobalThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            LogException(e.Exception);
            ShowException(e.Exception);
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException(ex);
                ShowException(ex);
            }
            else
            {
                LogException(new Exception("Unhandled exception of unknown type"));
            }
        }

        static void LogException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unhandled Exception: " + ex.ToString());
        }

        static void ShowException(Exception ex)
        {
            MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}