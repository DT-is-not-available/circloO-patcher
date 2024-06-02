using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
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
using static UndertaleModLib.Compiler.Compiler.AssemblyWriter;

namespace cpatcher;

public partial class MainWindow
{
    public List<Patch> Patches { get; set; } = new();
    public List<Patch> SelectedPatches { get; set; } = new();

    private bool AnyPatch(Patch patch, List<Patch> listPatch)
    {
        return listPatch.Any(p => p.Info.Name == patch.Info.Name);
    }

    public List<Patch> GetSelectedPatches()
    {
        List<Patch> selectedPatches = new();
        if (Properties.Settings.Default.SelectedPatches != null)
        {
            foreach (string? patchName in Properties.Settings.Default.SelectedPatches)
            {
                Patch? foundPatch = Patches.Find(p => p.Info.Name == patchName);
                if (foundPatch != null && !AnyPatch(foundPatch, selectedPatches))
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
        if (!AnyPatch(patch, Patches))
            Patches.Add(patch);
    }

    public bool HasPatch(Patch patch)
    {
        return AnyPatch(patch, SelectedPatches);
    }

    public bool ExecuteAllPatches()
    {
        bool success = false;
        foreach (Patch patch in Patches)
        {
            if (HasPatch(patch))
            {
                bool patchSuccess = patch.Execute();
                if (patchSuccess)
                    success = true;
            }
        }
        // atleast a single patch should pass
        return success;
    }

    public void ReloadPatchList(string searchTerm = "")
    {
        Debug.WriteLine("ReloadPatchList SelectedPatches.Count: " + SelectedPatches.Count);
        patchList.Items.Clear();
        patchList.BeginUpdate();
        foreach (Patch patch in Patches) {
            if (string.IsNullOrEmpty(searchTerm) || patch.Info.DisplayName.ToLower().Contains(searchTerm.ToLower()) ||
                patch.Info.Version.ToLower().Contains(searchTerm.ToLower()) ||
                patch.Info.Author.ToLower().Contains(searchTerm.ToLower()))
            {
                AddPatchToList(patch);
            }
        }
        patchList.EndUpdate();
    }

    private void AddPatchToList(Patch patch)
    {
        string description = string.IsNullOrEmpty(patch.Info.Description) ? "" : "\n\n" + patch.Info.Description;
        string author = "\n\nMade by: " + (string.IsNullOrEmpty(patch.Info.Author) ? "N/A" : patch.Info.Author);

        ListViewItem listItem = new ListViewItem(new[] { patch.Info.DisplayName, patch.Info.Version });
        listItem.Tag = patch;
        listItem.ToolTipText = $"{patch.Info.DisplayName} ({patch.Info.Name}){description}{author}\nVersion: {patch.Info.Version}";

        if (HasPatch(patch))
            listItem.Checked = true;

        patchList.Items.Add(listItem);
        Debug.WriteLine("AddPatchToList SelectedPatches.Count: " + SelectedPatches.Count);
    }
            
    private void reloadPatches_Click(object sender, EventArgs e)
    {
        LoadPatches(true);
        ReloadPatchList();
    }

    private void searchTextBox_TextChanged(object sender, EventArgs e)
    {
        // LoadPatches(true);
        ReloadPatchList(searchTextBox.Text);
        /*ListViewItem? foundItem =
            patchList.FindItemWithText(searchTextBox.Text, true, 0, true);
        if (foundItem != null)
        {
            patchList.TopItem = foundItem;
        }*/
    }

    private void patchList_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
        Patch? patch = (Patch?)e.Item.Tag;
        if (patch != null)
        {
            if (e.Item.Checked && !HasPatch(patch))
                SelectedPatches.Add(patch);

            if (!e.Item.Checked && HasPatch(patch))
                SelectedPatches.Remove(patch);

            SaveSelectedPatches();
        }
    }

    private void enableAll_Click(object sender, EventArgs e)
    {
        foreach (Patch patch in Patches)
        {
            if (!HasPatch(patch))
                SelectedPatches.Add(patch);
        }
        Debug.WriteLine("enableAll_Click SelectedPatches.Count: " + SelectedPatches.Count);
        SaveSelectedPatches();
        ReloadPatchList();
    }

    private void disableAll_Click(object sender, EventArgs e)
    {
        Debug.WriteLine("disableAll_Click before SelectedPatches.Count: " + SelectedPatches.Count);
        foreach (Patch patch in Patches)
        {
            if (HasPatch(patch))
                SelectedPatches.Remove(patch);
        }
        Debug.WriteLine("disableAll_Click SelectedPatches.Count: " + SelectedPatches.Count);
        SaveSelectedPatches();
        ReloadPatchList();
    }
}
