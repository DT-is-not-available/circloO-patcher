using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace cpatcher;

public partial class MainWindow
{
    public static string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        @"cpatcher"
    );

    public static string CircloORootPath = "";
    public static string PatchesPath => Path.Combine(AppDataPath, @"patches");
    public static string BackupsPath => Path.Combine(AppDataPath, @"backups");
    public static string CircloODataPath => Path.Combine(CircloORootPath, @"data.win");

    public void LoadPaths()
    {
        CircloORootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Steam\steamapps\common\circloO\");
        if (!Directory.Exists(CircloORootPath))
        {
            CircloORootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Steam\steamapps\common\circloO\");
            if (!Directory.Exists(CircloORootPath))
            {
                MessageBox.Show("CircloO root path not found! Verify your CircloO Steam installation and run the patcher again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        if (!File.Exists(CircloODataPath))
        {
            MessageBox.Show("CircloO win file not found in the CircloO root directory! Verify your CircloO Steam installation and run the patcher again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }

        if (!Directory.Exists(PatchesPath))
        {
            Directory.CreateDirectory(PatchesPath);
        }

        if (!Directory.Exists(BackupsPath))
        {
            Directory.CreateDirectory(BackupsPath);
        }
    }
}
