using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace cpatcher;

public partial class MainWindow
{
    public static List<string> Backups = new List<string>();
    public void LoadBackups()
    {
        Backups = Directory.GetFiles(Settings.Instance.CircloOBackupPath, "*.win", SearchOption.AllDirectories).ToList();
    }
    public string GetLatestVanilla()
    {
        foreach (string backup in Backups)
        {
            string[] fileSplit = Path.GetFileNameWithoutExtension(backup).Split("-"); // we're just gonna trust that all backups are data wins
            if (fileSplit[0] == "vanilla" && fileSplit[1] == Settings.Instance.CircloOVersion)
            {
                return backup;
            }
        }
        return "";
    }
}
