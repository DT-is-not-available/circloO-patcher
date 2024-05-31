using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

using UndertaleModLib;
using UndertaleModLib.Scripting;
using UndertaleModLib.Util;
using UndertaleModLib.Models;
using System.Xml.Serialization;

namespace cpatcher;

public partial class MainWindow
{
    public UndertaleData Data { get; set; }

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
}
