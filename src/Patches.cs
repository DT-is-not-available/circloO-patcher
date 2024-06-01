using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using static UndertaleModLib.Compiler.Compiler.AssemblyWriter;

namespace cpatcher;

// everything what will be included in setup phase
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

    public void Execute()
    {
        // roslyn RunCSharpScript here
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
        Match match = Regex.Match(source, $@"#{directiveName}\s([^\n]*)(?=\n|$)");
        
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
        return GetSelectedPatches().Any(p => p.Info.Name == patch.Info.Name);
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
