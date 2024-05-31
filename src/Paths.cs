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
        "cpatcher"
    );

    public static string CircloORootPath => Properties.Settings.Default.CircloORootPath;
    public static string PatchesPath => Path.Combine(AppDataPath, @"patches");
    public static string BackupsPath => Path.Combine(AppDataPath, @"backups");
    public static string CircloODataPath => Path.Combine(CircloORootPath, @"data.win");

    public void ReloadPaths()
    {
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

        string rootPath = CircloORootPath;
        bool firstTime = string.IsNullOrEmpty(rootPath);

        if (firstTime)
        {
            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Steam\steamapps\common\circloO\");
            if (!Directory.Exists(rootPath))
            {
                rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Steam\steamapps\common\circloO\");
                if (!Directory.Exists(rootPath))
                {
                    Console.WriteLine(rootPath);
                    ShowRootPathNotFound();
                }
            }
        } else
        {
            if (!Directory.Exists(rootPath))
            {
                ShowRootPathNotFound();
            } else
            {
                AppLocked = false;
            }
        }

        if (!AppLocked)
        {
            Properties.Settings.Default.CircloORootPath = rootPath;
            Properties.Settings.Default.Save();

            string dataWinPath = GetDataWinPath();
            System.Diagnostics.Debug.WriteLine("dataWinPath: " + dataWinPath);
            if (!File.Exists(dataWinPath))
            {
                ShowDataPathNotFound();
            }
            else
            {
                if (Path.GetFileName(dataWinPath) != "data.win")
                {
                    File.Move(dataWinPath, "data.win");
                }
                if (firstTime)
                {
                    CreateNewBackup();
                }
            }
        }
    }

    private string GetDataWinPath()
    {
        if (string.IsNullOrEmpty(CircloORootPath))
        {
            return "";
        } else
        {
            string[] files = Directory.GetFiles(CircloORootPath, "*.win", SearchOption.TopDirectoryOnly);
            return files[0];
        }
    }

    private void ShowRootPathNotFound()
    {
        MessageBox.Show("CircloO root path not found! Verify your CircloO Steam installation and run the patcher again. Or if your game is stored in a different directory, go to settings and set the path manually.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
        AppLocked = true;
        //Application.Exit();
    }

    private void ShowDataPathNotFound()
    {
        MessageBox.Show("CircloO win file not found in the CircloO root directory! Verify your CircloO Steam installation and run the patcher again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
        AppLocked = true;
        //Application.Exit();
    }

    /*
    public void InitializePaths()
    {
        Settings.Default.CircloOPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "/Steam/steamapps/common/circloO/");
        if (!Directory.Exists(Settings.Default.CircloOPath))
        {
            Settings.Default.CircloOPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "/Steam/steamapps/common/circloO/");
            if (!Directory.Exists(Settings.Default.CircloOPath))
            {
                MessageBox.Show("CircloO path not found! Verify your CircloO Steam installation and run the patcher again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        if (!File.Exists(Settings.Default.CircloODataPath))
        {
            MessageBox.Show("CircloO's data.win not found! Verify your CircloO Steam installation and run the patcher again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        if (!Directory.Exists(Settings.Default.CircloOPatchesPath))
        {
            Directory.CreateDirectory(Settings.Default.CircloOPatchesPath);
        }

        if (!Directory.Exists(Settings.Default.CircloOBackupPath))
        {
            Directory.CreateDirectory(Settings.Default.CircloOBackupPath);
        }
    }
    */
}
