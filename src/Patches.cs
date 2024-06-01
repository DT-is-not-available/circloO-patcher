using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Scripting;

namespace cpatcher;

public class PatchGlobals
{
    /// <summary>
    /// the circloo data win
    /// </summary>
    public UndertaleData Data = MainWindow.Data;

    /// <summary>
    /// path to the root of circloo
    /// </summary>
    public string CircloORootPath { get; } = MainWindow.CircloORootPath;

    /// <summary>
    /// path to the currently executed patch
    /// </summary>
    public string PatchPath { get; init; }

    /// <summary>
    /// ensures that a valid data file (<see cref="Data"/>) is loaded. an exception should be thrown if it isn't.
    /// </summary>
    public void EnsureDataLoaded()
    {
        if (Data is null)
            throw new Exception("No data file is currently loaded!");
    }

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

    public PatchGlobals(string patchPath)
    {
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
        this.Name = name;
        this.DisplayName = displayName;
        this.Description = description;
        this.Version = version;
        this.Author = author;
    }
}

public class Patch
{
    public Guid Id { get; init; }
    public PatchInfo Info { get; init; }
    public string CodePath { get; set; }

    public Patch(string codePath, PatchInfo info)
    {
        this.Id = Guid.NewGuid();
        this.CodePath = codePath;
        this.Info = info;
    }

    // samples:
    // https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md
    // https://github.com/UnderminersTeam/UndertaleModTool/blob/fb312d9a2b64063dcd458b9cdec3acb06c081db0/UndertaleModTool/ScriptingFunctions.cs#L58
    // https://github.com/UnderminersTeam/UndertaleModTool/blob/fb312d9a2b64063dcd458b9cdec3acb06c081db0/UndertaleModTool/MainWindow.xaml.cs#L248
    public async void Execute()
    {
        if (!File.Exists(CodePath))
        {
            return; // false
        }

        try
        {
            /*ScriptOptions scriptOptions = ScriptOptions.Default
                .AddImports("UndertaleModLib", "UndertaleModLib.Models", "UndertaleModLib.Decompiler",
                    "UndertaleModLib.Scripting", "UndertaleModLib.Compiler",
                    "UndertaleModTool", "System", "System.IO", "System.Collections.Generic",
                    "System.Text.RegularExpressions")
                .AddReferences(typeof(UndertaleObject).GetTypeInfo().Assembly, typeof(System.Text.RegularExpressions.Regex).GetTypeInfo().Assembly)
                .WithEmitDebugInformation(true); // when script throws an exception, add a exception location (line number)
            */

            ScriptOptions scriptOptions = ScriptOptions.Default;

            //CancellationTokenSource source = new CancellationTokenSource(100);
            //CancellationToken token = source.Token;

            PatchGlobals globals = new PatchGlobals(CodePath);

            object result = await CSharpScript.EvaluateAsync(
                File.ReadAllText(CodePath),
                scriptOptions,
                globals,
                typeof(PatchGlobals)
                //token
            );

            MainWindow.Data = globals.Data;
        }
        catch (OutOfMemoryException) { }
        /*catch (CompilationErrorException)
        {
            return; // false
        }
        catch (Exception)
        {
            return; // true
        }*/

        return; // true
    }
}

public partial class MainWindow
{
    public List<Patch> Patches { get; set; } = new List<Patch>();
    public List<Patch> SelectedPatches { get; set; } = new List<Patch>();

    public Patch? FindByName(string name)
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
        string name = String.IsNullOrEmpty(r) ? fileName : r;

        r = FindDirective(code, "displayname");
        string displayName = String.IsNullOrEmpty(r) ? name : r;

        r = FindDirective(code, "description");
        string description = String.IsNullOrEmpty(r) ? "" : r;

        r = FindDirective(code, "version");
        string version = String.IsNullOrEmpty(r) ? "0.1.0" : r;

        r = FindDirective(code, "author");
        string author = String.IsNullOrEmpty(r) ? "" : r;

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

    public void ExecuteAllPatches()
    {
        foreach (Patch patch in Patches)
        {
            if (HasPatch(patch))
            {
                patch.Execute();
            }
        }
    }

    public void ReloadPatchList()
    {
        LoadPatches(true);
        patchList.Items.Clear();
        patchList.BeginUpdate();
        foreach (Patch patch in Patches) {
            string description = String.IsNullOrEmpty(patch.Info.Description) ? "" : "\n\n" + patch.Info.Description;
            string author = "\n\nMade by: " + (String.IsNullOrEmpty(patch.Info.Author) ? "N/A" : patch.Info.Author);

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
