using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpatcher;

internal class Settings
{
    public static Settings Instance = null;


    public string PatcherVersion { get; set; } = "0.1.0"; //MainWindow.Version;
    public string CircloOVersion { get; set; } = "1.11"; //MainWindow.Version;

    public string CircloOPath { get; set; } = "";

    public string CircloODataPath => CircloOPath + "data.win"; // later add support for actually saving the config somewhere

    public string CircloOBackupPath => CircloOPath + "backups/";

    public string CircloOPatchesPath => CircloOPath + "patches/";

    public Settings() {
        Instance = this;
    }
}
