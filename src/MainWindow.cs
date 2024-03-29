using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using UndertaleModLib;
using UndertaleModLib.Scripting;
using UndertaleModLib.Util;
using UndertaleModLib.Models;

namespace cpatcher;

public partial class MainWindow : Form
{
    public UndertaleData Data { get; set; }
    public MainWindow()
    {
        new Settings();
        InitializeComponent();
        //InitializePaths();
        //this.Data = ReadDataFile(new FileInfo(Settings.Instance.CircloODataPath));
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
