using System;
using System.Data;

using UndertaleModLib;

public partial class MainWindow {

    List<CircloOPatch> patches { get; init; };
    List<CircloOPatch> selectedPatches { get; init; };

    public MainWindow() {
        this.patches = new List<CircloOPatch> {};
        this.selectedPatches = new List<CircloOPatch> {};
    }

    void newPatch(string name, Action callback) {
        patches.Add(new CircloOPatch(name, name, name, callback, false, false););
    }

    void newPatch(string name, bool recommended, bool required, Action callback) {
        patches.Add(new CircloOPatch(name, name, name, callback, recommended, required););
    }

    void newPatch(string name, string displayName, string description, bool recommended, bool required, Action callback) {
        patches.Add(new CircloOPatch(name, displayName, description, callback, recommended, required););
    }

    bool hasPatch(string name) {
        return selectedPatches.Any(p => p.Name == name) || patches.Find(p => p.Name == name).Required
    }

    void executeAllPatches() {
        foreach (CircloOPatch patch in patches) {
            if (hasPatch(patch.Name)) {
                patch.Execute();
            }
        }
    }

    // TODO: add the functions for actually generating GML code in the patches

}