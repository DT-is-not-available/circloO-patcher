using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace cpatcher;

public partial class MainWindow : Form
{
    public static string PatcherVersion { get; } = "0.1.0";
    public static string CircloOVersion { get; } = "1.12";

    public MainWindow()
    {
        LoadPaths();
        InitializeComponent();
        ReloadPatchList();

        //this.Data = ReadDataFile(new FileInfo(Settings.Default.CircloODataPath));
        // gonna add the message and warning handlers later
    }

    private void patchButton_Click(object sender, EventArgs e)
    {
        LoadDataFile();
        ExecuteAllPatches();
        SaveDataFile(CircloODataPath);
    }

    private void launchGameButton_Click(object sender, EventArgs e)
    {
        Process.Start("steam://rungameid/2195630");
    }

    private void browseCircloOFiles_Click(object sender, EventArgs e)
    {
        if (Directory.Exists(CircloODataPath))
            Process.Start("explorer.exe", CircloORootPath);
    }

    private void browsePatcherFiles_Click(object sender, EventArgs e)
    {
        if (Directory.Exists(AppDataPath))
            Process.Start("explorer.exe", AppDataPath);
    }
}
