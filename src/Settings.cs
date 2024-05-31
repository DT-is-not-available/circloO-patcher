using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace cpatcher;

internal class Settings
{
    public static Settings Default;

    //public static string AppDataFolder = Path.Combine(
    //    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
    //    "cpatcher"
    // );

    //public string PatcherVersion { get; } = "0.1.0"; //MainWindow.Version;
    //public static string PatcherVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    //public static string CircloOVersion = "1.11";
    //public bool AppLocked { get; set; } = false;

    //public static string PatchesFolder => Path.Combine(AppDataFolder, "patches");
    //public static string BackupsFolder => Path.Combine(AppDataFolder, "backups");
    //public string CircloOPath { get; set; } = "";

    public Settings() {
        Default = this;
    }
}
