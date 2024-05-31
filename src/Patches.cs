using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cpatcher;

// everything what will be included in setup phase
public readonly struct PatchInfo
{
    public string Name { get; init; } // aka internal name, init makes it so it cant be changes afterwards
    public string DisplayName { get; init; }
    public string Description { get; init; }
    public string Version { get; init; }
    public PatchInfo(string name, string displayName, string description, string version)
    {
        this.Name = name;
        this.DisplayName = displayName;
        this.Description = description;
        this.Version = version;
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

    private Patch? FindByName(string name)
    {
        return Patches.Find(p => p.Name == name);
    }

    public void NewPatch(Patch patch)
    {
        Patches.Add(patch);
    }

    public void NewPatch(string name, string displayName, string description, bool required, Action callback)
    {
        NewPatch(new Patch(name, displayName, description, callback, required));
    }

    public void NewPatch(string name, bool required, Action callback)
    {
        NewPatch(new Patch(name, name, name, callback, required));
    }

    public void NewPatch(string name, Action callback)
    {
        NewPatch(new Patch(name, name, name, callback, false));
    }

    public bool HasPatch(Patch patch)
    {
        return SelectedPatches.Any(p => p.Name == patch.Name) || FindByName(patch.Name).Required;
    }

    public bool HasPatch(string name)
    {
        return SelectedPatches.Any(p => p.Name == name) || FindByName(name).Required;
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
}
