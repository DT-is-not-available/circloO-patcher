using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Scripting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace cpatcher;

/// <summary>
/// exception what is thrown during an error in execution of a patch
/// </summary>
public class PatchException : Exception
{
    public PatchException() { }
    public PatchException(string message) : base(message) { }
}

public class PatchGlobals
{
    /// <summary>
    /// the circloo data win
    /// </summary>
    public UndertaleData Data;

    /// <summary>
    /// path to the root of circloo
    /// </summary>
    public string CircloORootPath { get; init; }

    /// <summary>
    /// path to the currently executed patch
    /// </summary>
    public string PatchPath { get; init; }

    /// <summary>
    /// Info about current patch
    /// </summary>
    public PatchInfo Info { get; init; }

    // WIP: some actual error handling for patches, assembler (asm) integration

    // source: cirqol
    /// <summary>
    /// get undertale code inside data from code name, and create it if doesnt exist, as an option
    /// </summary>
    public UndertaleCode? Code(string codeName, bool makeIfNotExists = true)
    {
        UndertaleCode code = Data.Code.ByName(codeName);
        if (code == null && makeIfNotExists)
        {
            UndertaleCodeLocals locals = new UndertaleCodeLocals();
            locals.Name = Data.Strings.MakeString(codeName);
            code = new UndertaleCode();
            code.Name = locals.Name;
            Data.Code.Add(code);
            Data.CodeLocals.Add(locals);
        }
        return code;
    }

    /// <summary>
    /// create gml function from gml code
    /// </summary>
    public void CreateGMLFunction(string code)
    {
        string funcname = Regex.Match(code, @"^\s*function\s*(\w+)").Groups[1].Value;
        string filename = "gml_GlobalScript_" + funcname;
        Code(filename).ReplaceGML(code, Data);
    }

    /// <summary>
    /// show some message (information) to the user
    /// </summary>
    public void PatchMessage(string message)
    {
        MessageBox.Show(message, $"Message from {Info.DisplayName}", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// show some error to the user
    /// </summary>
    public void PatchError(string error)
    {
        MessageBox.Show(error, $"Error from {Info.DisplayName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// ask a yes/no question to the user
    /// </summary>
    public bool PatchQuestion(string message)
    {
        DialogResult res = MessageBox.Show(message, $"Question from {Info.DisplayName}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        return res == DialogResult.Yes;
    }

    public PatchGlobals(UndertaleData data, PatchInfo info, string rootPath, string patchPath)
    {
        Data = data;
        Info = info;
        CircloORootPath = rootPath;
        PatchPath = patchPath;
    }
}

public readonly struct PatchInfo
{
    public string Name { get; init; } // aka internal name, init makes it so it cant be changes afterwards
    public string DisplayName { get; init; }
    public string Description { get; init; }
    public string Version { get; init; }
    public string Author { get; init; }
    public PatchInfo(string name, string displayName, string description, string version, string author)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        Version = version;
        Author = author;
    }
}

public class Patch
{
    public Guid Id { get; init; }
    public PatchInfo Info { get; init; }
    public string CodePath { get; set; }

    public Patch(string codePath, PatchInfo info)
    {
        Id = Guid.NewGuid();
        CodePath = codePath;
        Info = info;
    }

    // samples:
    // https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md
    // https://github.com/UnderminersTeam/UndertaleModTool/blob/fb312d9a2b64063dcd458b9cdec3acb06c081db0/UndertaleModTool/ScriptingFunctions.cs#L58
    // https://github.com/UnderminersTeam/UndertaleModTool/blob/fb312d9a2b64063dcd458b9cdec3acb06c081db0/UndertaleModTool/MainWindow.xaml.cs#L248
    public bool Execute()
    {
        if (!File.Exists(CodePath))
        {
            return false;
        }

        try
        {
            ScriptOptions scriptOptions = ScriptOptions.Default
                .AddImports("UndertaleModLib", "UndertaleModLib.Models", "UndertaleModLib.Decompiler",
                    "UndertaleModLib.Scripting", "UndertaleModLib.Compiler",
                    "UndertaleModTool", "System", "System.IO", "System.Collections.Generic",
                    "System.Text.RegularExpressions", "System.Linq")
                .AddReferences(typeof(UndertaleObject).GetTypeInfo().Assembly, typeof(System.Text.RegularExpressions.Regex).GetTypeInfo().Assembly)
                .WithEmitDebugInformation(true); // when script throws an exception, add a exception location (line number)

            //ScriptOptions scriptOptions = ScriptOptions.Default;

            CancellationTokenSource source = new CancellationTokenSource(100);
            CancellationToken token = source.Token;

            PatchGlobals globals = new PatchGlobals(MainWindow.Data, Info, MainWindow.CircloORootPath, CodePath);

            object result = CSharpScript.EvaluateAsync(
                File.ReadAllText(CodePath),
                scriptOptions,
                globals,
                typeof(PatchGlobals),
                token
            );

            MainWindow.Data = globals.Data;
        }
        catch (CompilationErrorException exc)
        {
            MessageBox.Show($"Compilation error of patch {Info.DisplayName}: {exc}", $"Error from {Info.DisplayName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        catch (Exception)
        {
            return true;
        }

        return true;
    }
}

public partial class MainWindow
{
    public List<Patch> Patches { get; set; } = new List<Patch>();
    public List<Patch> SelectedPatches { get; set; } = new List<Patch>();

    private Patch? FindByName(string name)
    {
        return Patches.Find(p => p.Info.Name == name);
    }
    public List<Patch> GetSelectedPatches()
    {
        List<Patch> selectedPatches = new List<Patch>();
        if (Properties.Settings.Default.SelectedPatches != null)
        {
            foreach (string patchName in Properties.Settings.Default.SelectedPatches)
            {
                Patch? foundPatch = FindByName(patchName);
                if (foundPatch != null)
                {
                    selectedPatches.Add(foundPatch);
                }
            }
        }
        return selectedPatches;
    }
    public void LoadSelectedPatches()
    {
        SelectedPatches = GetSelectedPatches();
    }
    public void SaveSelectedPatches()
    {
        System.Collections.Specialized.StringCollection sc = new();
        sc.AddRange(SelectedPatches.Select(p => p.Info.Name).ToArray());
        Properties.Settings.Default.SelectedPatches = sc;
        Properties.Settings.Default.Save();
    }

    private string FindDirective(string source, string directiveName)
    {
        // #load\s([^\n]*)(?=\n|$)
        string directivePrefix = @"\/\/\$"; // im constantly changing this, just a directive like # wont work
        Match match = Regex.Match(source, $@"{directivePrefix}{directiveName}\s([^\n]*)(?=\n|$)");
        
        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        } else
        {
            return "";
        }
    }
    public PatchInfo GetPatchInfo(string fileName, string code)
    {
        string r;

        r = FindDirective(code, "name");
        string name = string.IsNullOrEmpty(r) ? fileName : r;

        r = FindDirective(code, "displayname");
        string displayName = string.IsNullOrEmpty(r) ? name : r;

        r = FindDirective(code, "description");
        string description = string.IsNullOrEmpty(r) ? "" : r;

        r = FindDirective(code, "version");
        string version = string.IsNullOrEmpty(r) ? "0.1.0" : r;

        r = FindDirective(code, "author");
        string author = string.IsNullOrEmpty(r) ? "" : r;

        return new PatchInfo(name, displayName, description, version, author);
    }

    public void LoadPatches(bool reload = true)
    {
        if (reload)
            Patches.Clear();

        string[] filePaths = Directory.GetFiles(PatchesPath, "*.csx", SearchOption.AllDirectories);
        foreach (string filePath in filePaths)
        {
            NewPatch(
                new Patch(
                    filePath,
                    GetPatchInfo(
                        Path.GetFileName(filePath),
                        File.ReadAllText(filePath)
                    )
                )
            );
        }
    }

    public void NewPatch(Patch patch)
    {
        Patches.Add(patch);
    }

    public bool HasPatch(Patch patch)
    {
        return SelectedPatches.Any(p => p.Info.Name == patch.Info.Name);
    }

    public bool ExecuteAllPatches()
    {
        bool success = false;
        foreach (Patch patch in Patches)
        {
            if (HasPatch(patch))
            {
                bool result = patch.Execute();
                if (result)
                    success = true;
            }
        }
        // atleast a single patch should pass
        return success;
    }

    public void ReloadPatchList()
    {
        LoadPatches(true);
        patchList.Items.Clear();
        patchList.BeginUpdate();
        foreach (Patch patch in Patches) {
            string description = string.IsNullOrEmpty(patch.Info.Description) ? "" : "\n\n" + patch.Info.Description;
            string author = "\n\nMade by: " + (string.IsNullOrEmpty(patch.Info.Author) ? "N/A" : patch.Info.Author);

            ListViewItem listItem = new ListViewItem(new[] { patch.Info.DisplayName, patch.Info.Version });
            listItem.Tag = patch;
            listItem.ToolTipText = $"{patch.Info.DisplayName} ({patch.Info.Name}){description}{author}\nVersion: {patch.Info.Version}";

            if (HasPatch(patch))
                listItem.Checked = true;

            patchList.Items.Add(listItem);
        }
        patchList.EndUpdate();
    }
            
    private void reloadPatches_Click(object sender, EventArgs e)
    {
        ReloadPatchList();
    }

    private void searchTextBox_TextChanged(object sender, EventArgs e)
    {
        ListViewItem? foundItem =
            patchList.FindItemWithText(searchTextBox.Text, true, 0, true);
        if (foundItem != null)
        {
            patchList.TopItem = foundItem;
        }
    }

    private void patchList_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
        Patch? checkedPatch = (Patch)e.Item.Tag;
        if (checkedPatch != null)
        {
            if (e.Item.Checked && SelectedPatches.Find(p => p.Info.Name == checkedPatch.Info.Name) == null)
                SelectedPatches.Add(checkedPatch);

            if (!e.Item.Checked && SelectedPatches.Find(p => p.Info.Name == checkedPatch.Info.Name) != null)
                SelectedPatches.Remove(checkedPatch);

            SaveSelectedPatches();
        }
    }
}
