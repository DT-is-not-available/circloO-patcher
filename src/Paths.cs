using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpatcher;

public partial class MainWindow
{
    public void InitializePaths()
    {
        Settings.Instance.CircloOPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "/Steam/steamapps/common/circloO/";
        if (!Directory.Exists(Settings.Instance.CircloOPath))
        {
            Settings.Instance.CircloOPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "/Steam/steamapps/common/circloO/";
            if (!Directory.Exists(Settings.Instance.CircloOPath))
            {
                MessageBox.Show("CircloO path not found! Verify your CircloO Steam installation and run the patcher again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        if (!File.Exists(Settings.Instance.CircloODataPath))
        {
            MessageBox.Show("CircloO's data.win not found! Verify your CircloO Steam installation and run the patcher again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        if (!Directory.Exists(Settings.Instance.CircloOPatchesPath))
        {
            Directory.CreateDirectory(Settings.Instance.CircloOPatchesPath);
        }

        if (!Directory.Exists(Settings.Instance.CircloOBackupPath))
        {
            Directory.CreateDirectory(Settings.Instance.CircloOBackupPath);
        }
    }
}
