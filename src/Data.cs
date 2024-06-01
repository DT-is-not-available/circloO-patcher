﻿using System;
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

    public void LoadDataFile()
    {
        string filePath = CircloODataPath;
        UndertaleData data = ReadDataFile(new FileInfo(filePath));
        string backupPath = Path.Combine(BackupsPath, $"{data.GeneralInfo.FileName.Content}-{data.GeneralInfo.Timestamp}.win");

        if (data.Code.ByName("hasbeenmodded") != null) // was modded
        {
            if (!File.Exists(backupPath) || filePath == backupPath)
            {
                MessageBox.Show("Automated backup not found in backups folder. If you have a manually created backup, please copy it to application data and try again.", "CircloO Patcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else
            {
                File.Copy(backupPath, filePath, true);
                LoadDataFile();
            }
        } else // wasn't modded
        {
            if (!File.Exists(backupPath))
            {
                File.Copy(filePath, backupPath, true);
            }
        }

        this.Data = data;
    }

    public void SaveDataFile(string outputPath, UndertaleWriter.MessageHandlerDelegate messageHandler = null)
    {
        using FileStream fs = new FileInfo(outputPath).OpenWrite();
        UndertaleIO.Write(fs, Data, messageHandler);
    }

    public UndertaleData ReadDataFile(FileInfo datafile, UndertaleReader.WarningHandlerDelegate warningHandler = null, UndertaleReader.MessageHandlerDelegate messageHandler = null)
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
}
