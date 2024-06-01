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

    private string[] FindDirective(string source, string directiveName)
    {
        Match match = Regex.Match(source, $@"#\s*{directiveName}\s*([^\n]*)\");
        
        if (match.Success)
        {
            return match.Groups[1].Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        } else
        {
            return [];
        }
    }
    public PatchInfo GetPatchInfo(string fileName, string code)
    {
        string[] r;

        string name = fileName;
        r = FindDirective(code, "name");
        if (r.Length > 0)
            name = r[0];

        string displayName = name;
        r = FindDirective(code, "displayname");
        if (r.Length > 0)
            displayName = r[0];

        string description = "";
        r = FindDirective(code, "description");
        if (r.Length > 0)
            description = r[0];

        string version = "0.1.0";
        r = FindDirective(code, "version");
        if (r.Length > 0)
            version = r[0];

        string author = "";
        r = FindDirective(code, "author");
        if (r.Length > 0)
            author = r[0];

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
        foreach (Patch patch in Patches) {
            string author = String.IsNullOrEmpty(patch.Info.Author) ? "N/A" : patch.Info.Author;
            ListViewItem listItem = new ListViewItem(new[] { patch.Info.DisplayName, patch.Info.Version, author });
            listItem.ToolTipText = $"{patch.Info.DisplayName} ({patch.Info.Name}){System.Environment.NewLine}{System.Environment.NewLine}{patch.Info.Description}";
            patchList.Items.Add(listItem);
        }
    }
            
    private void reloadPatches_Click(object sender, EventArgs e)
    {
        ReloadPatchList();
    }
}
