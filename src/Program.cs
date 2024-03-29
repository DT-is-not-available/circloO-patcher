using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindow());
        }
    }
}