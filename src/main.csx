#load "config.csx"
#load "paths.csx"
#load "utfileio.csx"
#load "patches.csx"
#load "presets.csx"
#load "dialog.csx"

using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

/*
    BIG LIST OF TODOS:
    - Make the ReplaceTextInASM function allow for wildcard numbers (something like ** when finding any number, and *1* to find a matching thing that could be any number, ie for assembly labels)
    - Condense patches into their own functions, then insert those functions into the main code which will allow for the next item
    - Easier removal and updating of cirQOL than "yeah just nuke your data.win file trust me it'll be fine"
    - Make this script auto generated because this is becoming a nightmare to edit and im tired of using "" to represent " in multiline strings and quite frankly theres a better way of doing this
*/
/* ideas:
    -   zip file with custom patches for adding custom patches
    -   patches involve 'hooks': hooks can be created and inserted anywhere in the code, and patches can latch onto that without worrying about conflicting with each other
    -   find some better way to replace and deactivate certain parts of code
*/

public Form form = new cirQOLdialog(selectedPatches, patches, patchids, presets, presetpatches, circloO_filepath, Data, requiredpatch);
form.ShowDialog();
if (selectedPatches.Count == 0) return;

string PATCHLIST = String.Join("\n", selectedPatches);

if (DEBUGMODE) {
    ScriptMessage("BEEP BOOP I DID IT\n\n"+PATCHLIST);
}

code("hasbeenmodded");

for (int i = 0; i < patchids.Count; i++) {
    if (requiredpatch[i] || selectedPatches.Contains(patchids[i])) {
        patchfunctions[i]();
    }
}

/// PATCHDESC: just read the fucking line of code
Data.GeneralInfo.DisplayName = Data.Strings.MakeString("cirQOL");

// await App.Current.MainWindow.SaveFile(circloO_path);
// fuck you private methods, ill do what i want
await SaveFile(circloO_filepath);

ScriptMessage("Done! Game file has been modified, all you need to do now is run circloO.");

/// FUNCDEFS:

string GetReplacerRegex(string keyword) {
    Regex reg = new Regex(@"\\\*(\w+)\\\*");
    return Regex.Replace(
        Regex.Replace(
            Regex.Replace(
                Regex.Escape(
                    Regex.Replace(
                        keyword, @"^[^\S\n]+|[^\S\n]+$", "", RegexOptions.Multiline
                    )
                ),  @"\\\*\\\*\\\*", @"\-?[\d\.]+", RegexOptions.None
            ), @"\\\*<(\w+)\\\*", @"(?'$1'\-?[\d\.]+)", RegexOptions.None
        ), @"\\\*>(\w+)\\\*", @"\k'$1'", RegexOptions.None
    );
}

string GetReplacementRegex(string replacement) {
    return Regex.Replace(
        Regex.Replace(
            replacement.Replace("$", "$$"), @"^[^\S\n]+|[^\S\n]+$", "", RegexOptions.Multiline
        ), @"\*(\w+)\*", "$${$1}", RegexOptions.Multiline
    );
}

string GetPassBack(string decompiled_text, string keyword, string replacement, bool case_sensitive = false, bool isRegex = false)
{
    decompiled_text = decompiled_text.Replace("\r\n", "\n");
    keyword = keyword.Replace("\r\n", "\n");
    replacement = replacement.Replace("\r\n", "\n");
    string passBack;
    if (!isRegex)
    {
        if (DEBUGMODE) ScriptMessage(GetReplacerRegex(keyword));
        if (DEBUGMODE) Clipboard.SetText(GetReplacerRegex(keyword));
        if (DEBUGMODE) ScriptMessage(GetReplacementRegex(replacement));
        if (case_sensitive)
            passBack = Regex.Replace(decompiled_text, GetReplacerRegex(keyword), GetReplacementRegex(replacement), RegexOptions.None);
        else
            passBack = Regex.Replace(decompiled_text, GetReplacerRegex(keyword), GetReplacementRegex(replacement), RegexOptions.IgnoreCase);
    }
    else
    {
        if (case_sensitive)
            passBack = Regex.Replace(decompiled_text, keyword, replacement, RegexOptions.None);
        else
            passBack = Regex.Replace(decompiled_text, keyword, replacement, RegexOptions.IgnoreCase);
    }
    return passBack;
}
void ReplaceTextInASM(string codeName, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null)
{
    UndertaleCode code = Data.Code.ByName(codeName);
    if (code is null)
        throw new ScriptException($"No code named \"{codeName}\" was found!");

    ReplaceTextInASM(code, keyword, replacement, caseSensitive, isRegex, context);
}
void ReplaceTextInASM(UndertaleCode code, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null)
{
    if (code.ParentEntry is not null)
        return;

    EnsureDataLoaded();

    string passBack = "";
    string codeName = code.Name.Content;
    GlobalDecompileContext DECOMPILE_CONTEXT = context is null ? new(Data, false) : context;

    try
    {
        // It would just be recompiling an empty string and messing with null entries seems bad
        if (code is null)
            return;
        string originalCode = code.Disassemble(Data.Variables, Data.CodeLocals.For(code));
        // ScriptMessage(originalCode.Substring(0, 1000));
        passBack = GetPassBack(originalCode, keyword, replacement, caseSensitive, isRegex);
        // No need to compile something unchanged
        if (passBack == originalCode) {
            ScriptMessage("its the same");
            return;
        }
        var instructions = Assembler.Assemble(passBack, Data);
        code.Replace(instructions);
    }
    catch (Exception exc)
    {
        if (ScriptQuestion("Error during ASM code replacement:\n" + exc.ToString() + "\n\nCopy passback?")) {
            Clipboard.SetText(passBack);
        };
        throw new Exception("a");
    }
}
string Disassemble(UndertaleCode code) {
    return code.Disassemble(Data.Variables, Data.CodeLocals.For(code));
}
UndertaleCode code(string codeName, bool makeIfNotExists = true) {
    UndertaleCode c = Data.Code.ByName(codeName);
    if (c == null && makeIfNotExists) {
        c = new UndertaleCode();
        c.Name = Data.Strings.MakeString(codeName);
        Data.Code.Add(c);
    }
    return c;
}