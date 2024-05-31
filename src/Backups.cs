using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace cpatcher;

public struct BackupInfo
{
    public BackupInfo(string fullPath)
    {
        // we're just gonna trust that all backups are data wins
        string[] fileSplit = Path.GetFileNameWithoutExtension(fullPath).Split("-");
        if (fileSplit.Length == 2)
        {
            Name = fileSplit[0];
            Version = fileSplit[1];
            FullPath = fullPath;
        } else
        {
            throw new ArgumentException("Full path must be in the format of name-version", "fullPath");
        }
    }
    public string Name;
    public string Version;
    public string FullPath;
}

public partial class MainWindow
{
    public List<BackupInfo> Backups = new List<BackupInfo>();

    public void LoadBackups()
    {
        string[] backups = Directory.GetFiles(BackupsPath, "*.win", SearchOption.TopDirectoryOnly);
        foreach (string backup in backups)
        {
            try
            {
                Backups.Add(new BackupInfo(backup));
            } catch
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load backup {backup}");
            }
        }
    }
    public void CreateNewBackup()
    {
        string GetDataWinBackup(bool firstTry)
        {
            string dataWinBackup = Path.Join(BackupsPath, $"vanilla-{CircloOVersion}");
            if (File.Exists(dataWinBackup) == true)
            {
                // tell the user that there already exists a backup with data win with current version of circloo.
                // update the software so CircloOVersion would update, or type the verison manually
                // if firstTry
                string userInput = "h";
                dataWinBackup = Path.Join(BackupsPath, $"vanilla-{userInput}");
                if (File.Exists(dataWinBackup) == true)
                {
                    GetDataWinBackup(false);
                }
            }
            return dataWinBackup;
        }
        string dataWinBackup = GetDataWinBackup(true);
        string dataWin = GetDataWinPath();
        if (dataWin != "")
        {
            try
            {
                File.Copy(dataWin, Path.Join(BackupsPath, dataWinBackup));
                Backups.Add(new BackupInfo(dataWinBackup));
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create backup {dataWinBackup} for {dataWin}");
            }
        }
    }
}
