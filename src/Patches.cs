using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cpatcher;

public class CircloOPatch
{
    public Guid Id { get; init; }
    public string Name { get; init; } // aka internal name, init makes it so it cant be changes afterwards
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public Action Callback { get; set; }
    public bool Required { get; set; }

    public CircloOPatch(string Name, string DisplayName, string Description, Action Callback, bool Required = false)
    {
        this.Id = Guid.NewGuid();
        this.Name = Name;
        this.DisplayName = DisplayName;
        this.Description = Description;
        this.Callback = Callback;
        this.Required = Required;
    }

    public void Execute()
    {
        this.Callback(); // pass some additional arguments in like data and helper functions
    }
}

public partial class MainWindow
{
    public List<CircloOPatch> Patches { get; set; } = new List<CircloOPatch>();
    public List<CircloOPatch> SelectedPatches { get; set; } = new List<CircloOPatch>();

    private CircloOPatch? FindByName(string name)
    {
        return Patches.Find(p => p.Name == name);
    }

    public void NewPatch(CircloOPatch patch)
    {
        Patches.Add(patch);
    }

    public void NewPatch(string name, string displayName, string description, bool required, Action callback)
    {
        NewPatch(new CircloOPatch(name, displayName, description, callback, required));
    }

    public void NewPatch(string name, bool required, Action callback)
    {
        NewPatch(new CircloOPatch(name, name, name, callback, required));
    }

    public void NewPatch(string name, Action callback)
    {
        NewPatch(new CircloOPatch(name, name, name, callback, false));
    }

    public bool HasPatch(CircloOPatch patch)
    {
        return SelectedPatches.Any(p => p.Name == patch.Name) || FindByName(patch.Name).Required;
    }

    public bool HasPatch(string name)
    {
        return SelectedPatches.Any(p => p.Name == name) || FindByName(name).Required;
    }

    public void ExecuteAllPatches()
    {
        foreach (CircloOPatch patch in Patches)
        {
            if (HasPatch(patch))
            {
                patch.Execute();
            }
        }
    }
}
