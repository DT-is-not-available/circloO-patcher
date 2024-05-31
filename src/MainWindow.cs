using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using UndertaleModLib;
using UndertaleModLib.Scripting;
using UndertaleModLib.Util;
using UndertaleModLib.Models;
using System.Xml.Serialization;

namespace cpatcher;

public partial class MainWindow : Form
{
    public static string PatcherVersion { get; } = "0.1.0";
    public static string CircloOVersion { get; } = "1.12";

    public bool AppLocked = false;
    public UndertaleData Data { get; set; }
    public MainWindow()
    {
        //new Settings();
        InitializeComponent();
        ReloadPaths();
        //InitializePaths();
        //this.Data = ReadDataFile(new FileInfo(Settings.Default.CircloODataPath));
        // gonna add the message and warning handlers later
    }

    private void SaveDataFile(string outputPath, UndertaleWriter.MessageHandlerDelegate messageHandler = null)
    {
        using FileStream fs = new FileInfo(outputPath).OpenWrite();
        UndertaleIO.Write(fs, Data, messageHandler);
    }

    private static UndertaleData ReadDataFile(FileInfo datafile, UndertaleReader.WarningHandlerDelegate warningHandler = null, UndertaleReader.MessageHandlerDelegate messageHandler = null)
    {
        try
        {
            using FileStream fs = datafile.OpenRead();
            UndertaleData gmData = UndertaleIO.Read(fs, warningHandler, messageHandler);
            return gmData;
        }
        catch (FileNotFoundException e)
        {
            throw new FileNotFoundException($"Data file '{e.FileName}' does not exist");
        }
    }

    private void patchButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show("You patched!");
    }

    private void launchGameButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show("You launched!");
    }
}
