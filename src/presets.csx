using System;
using System.Data;

List<string> newPreset(string name, List<string> patches) {
    presets.Add(name);
    presetpatches.Add(patches);
    return patches;
}
List<string> newPreset(string name) {
    return newPreset(name, new List<string> {});
}
void applyPreset(int id) {
    if (presetpatches.Count > id) {
        List<string> selectedPatches = new List<string> (presetpatches[id]);
    }
}

// built-in presets
// TODO: make this dynamically loaded

newPreset("Everything", patchids);
List<string> experimental = newPreset("Experimental Features");
List<string> recommended = newPreset("Recommended");
List<string> QOLmods = newPreset("QOL modifications only");
List<string> editmods = newPreset("Editor modifications only");
newPreset("Nothing");
for (int i = 0; i < patchids.Count; i++) {
    string id = patchids[i];
    if (id.StartsWith("e-")) {
        editmods.Add(id);
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("q-")) {
        QOLmods.Add(id);
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("b-")) {
        QOLmods.Add(id);
        editmods.Add(id);
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("r-")) {
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("ex-")) {
        experimental.Add(id);
    }
}