using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using UndertaleModLib;

namespace cpatcher;

public partial class MainWindow : Form
{
    public static string PatcherVersion { get; } = "0.1.0";
    public static string CircloOVersion { get; } = "1.12";

    public MainWindow()
    {
        LoadPaths();
        LoadPatches();
        LoadSelectedPatches();
        InitializeComponent();
        ReloadPatchList();
    }

    private void PatchData()
    {
        //UseWaitCursor = true;
        Cursor.Current = Cursors.WaitCursor;
        try
        {
            bool success;

            Debug.WriteLine("LoadDataFile start");
            success = LoadDataFile();
            if (!success)
            {
                throw new Exception("Failed to load data file");
            }

            Debug.WriteLine("ExecuteAllPatches start");
            success = ExecuteAllPatches();
            if (!success)
            {
                MessageBox.Show("Nothing was patched, every selected patch failed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw new Exception("Nothing was patched");
            }

            Debug.WriteLine("Code hasbeenmodded start");
            Code("hasbeenmodded", true);

            Debug.WriteLine("SaveDataFile start");
            SaveDataFile(Data, CircloODataPath);

            MessageBox.Show("Finished patching! Now you can open the game.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error happened during patching process: {ex}", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor.Current = Cursors.Default;
            Debug.WriteLine("Disposing UndertaleData");
            Data.Dispose();
        }
    }

    private void RestoreUnmodded()
    {
        Cursor.Current = Cursors.WaitCursor;
        try
        {
            bool success;

            Debug.WriteLine("RestoreUnmodded LoadDataFile start");
            success = LoadDataFile();
            if (!success)
            {
                throw new Exception("Failed to load data file");
            }

            MessageBox.Show("Finished restoring the unmodded version! Now you can open the game.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error happened during restoration process: {ex}", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor.Current = Cursors.Default;
            Debug.WriteLine("Disposing UndertaleData");
            Data.Dispose();
        }
    }

    private void patchButton_Click(object sender, EventArgs e)
    {
        if (SelectedPatches.Count > 0)
        {
            DialogResult res = MessageBox.Show("Selected patches will be applied to the unmodded version of the game. Do you want to continue?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (res == DialogResult.OK)
                PatchData();
        } else
        {
            DialogResult res = MessageBox.Show("No patches were selected, the patcher will now restore the unmodded version of the game. Do you want to continue?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (res == DialogResult.OK)
                RestoreUnmodded();
        }
    }

    private void launchGameButton_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo("steam://rungameid/2195630") { UseShellExecute = true });
        }
        catch (Exception)
        {
            MessageBox.Show("Something went wrong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void browseCircloOFiles_Click(object sender, EventArgs e)
    {
        if (Directory.Exists(CircloORootPath))
            Process.Start("explorer.exe", CircloORootPath);
    }

    private void browsePatcherFiles_Click(object sender, EventArgs e)
    {
        if (Directory.Exists(AppDataPath))
            Process.Start("explorer.exe", AppDataPath);
    }
}
