using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cpatcher;

public partial class MainWindow : Form
{
    public static string PatcherVersion { get; } = "0.1.0";
    public static string CircloOVersion { get; } = "1.12";

    public bool AppLocked = false;
    public MainWindow()
    {
        InitializeComponent();
        ReloadPaths();

        //this.Data = ReadDataFile(new FileInfo(Settings.Default.CircloODataPath));
        // gonna add the message and warning handlers later
    }
}
