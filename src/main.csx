#load "config.csx"
#load "paths.csx"
#load "utfileio.csx"
#load "patches.csx"
#load "presets.csx"
#load "dialog.csx"

using System;
using System.Text;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Collections.ObjectModel;

using UndertaleModLib.Compiler;
using UndertaleModLib.Decompiler;

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

string GetReplacementValue(string replacement) {
    return Regex.Replace(
        Regex.Replace(
            replacement.Replace("$", "$$"), @"^[^\S\n]+|[^\S\n]+$", "", RegexOptions.Multiline
        ), @"\*(\w+)\*", "$${$1}", RegexOptions.Multiline
    );
}

List<uint> FindBlockAddresses(IList<UndertaleInstruction> instructions) {
    List<uint> addresses = new List<uint>();

    if (instructions.Count != 0)
        addresses.Add(0);

    foreach (var inst in instructions)
    {
        switch (inst.Kind)
        {
            case UndertaleInstruction.Opcode.B:
            case UndertaleInstruction.Opcode.Bf:
            case UndertaleInstruction.Opcode.Bt:
            case UndertaleInstruction.Opcode.PushEnv:
                addresses.Add(inst.Address + 1);
                addresses.Add((uint)(inst.Address + inst.JumpOffset));
                break;
            case UndertaleInstruction.Opcode.PopEnv:
                if (!inst.JumpOffsetPopenvExitMagic)
                    addresses.Add((uint)(inst.Address + inst.JumpOffset));
                break;
            case UndertaleInstruction.Opcode.Exit:
            case UndertaleInstruction.Opcode.Ret:
                addresses.Add(inst.Address + 1);
                break;
        }
    }

    addresses.Sort();
    return addresses;
}

int sub = 0;
string CompileGMLFragments(string hybrid, UndertaleCode code) {
    return Regex.Replace(hybrid, @"#{((?>(?!{|}).|{(?<Depth>)|}(?<-Depth>))*)(?(Depth)(?!))}", new MatchEvaluator(new Func<Match, string> ((Match m) => {
        UndertaleCodeLocals locals = Data.CodeLocals.For(code);
        ObservableCollection<UndertaleCodeLocals.LocalVar> prevlocals = new ObservableCollection<UndertaleCodeLocals.LocalVar> (locals.Locals);
        IList<UndertaleInstruction> instructions = CompGML(m.Groups[1].Value, code);
        for (int i = 0; i < prevlocals.Count; i++) {
            if (locals.Locals.IndexOf(prevlocals[i]) == -1) locals.Locals.Add(prevlocals[i]);
        }

        StringBuilder sb = new StringBuilder(200);

        Dictionary<uint, string> fragments = new(code.ChildEntries.Count);
        foreach (var dup in code.ChildEntries)
        {
            fragments.Add(dup.Offset / 4, (dup.Name?.Content ?? "<null>") + $" (locals={dup.LocalsCount}, argc={dup.ArgumentsCount})");
        }

        List<uint> blocks = FindBlockAddresses(instructions);

        foreach (var inst in instructions)
        {
            bool doNewline = true;
            if (fragments.TryGetValue(inst.Address, out string entry))
            {
                sb.AppendLine();
                sb.AppendLine($"> {entry}");
                doNewline = false;
            }

            int ind = blocks.IndexOf(inst.Address);
            if (ind != -1)
            {
                if (doNewline)
                    sb.AppendLine();
                sb.AppendLine($":[sub{sub}_{ind}]");
            }

            string inst_str = inst.ToString(code, blocks);
            if (inst_str[0] == 'b') inst_str = inst_str.Replace("[", $"[sub{sub}_");
            sb.Append(inst_str);

            sb.Append(Environment.NewLine);
        }

        sub++;

        return sb.ToString();
    })), RegexOptions.Singleline);
}

IList<UndertaleInstruction> CompGML(string source, UndertaleCode code) {
    CompileContext context = Compiler.CompileGMLText(source, Data, code);
    if (!context.SuccessfulCompile || context.HasError)
    {
        Console.WriteLine(source);
        throw new Exception("GML Compile Error: " + context.ResultError + "\n----\n" + source);
    }

    foreach (KeyValuePair<string, string> v in context.GlobalVars)
        Data.Variables.EnsureDefined(v.Key, UndertaleInstruction.InstanceType.Global, false, Data.Strings, Data);

    return context.ResultAssembly;
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
        if (DEBUGMODE) ScriptMessage(GetReplacementValue(replacement));
        if (case_sensitive)
            passBack = Regex.Replace(decompiled_text, GetReplacerRegex(keyword), GetReplacementValue(replacement), RegexOptions.None);
        else
            passBack = Regex.Replace(decompiled_text, GetReplacerRegex(keyword), GetReplacementValue(replacement), RegexOptions.IgnoreCase);
    }
    else
    {
        if (case_sensitive)
            passBack = Regex.Replace(decompiled_text, keyword, replacement, RegexOptions.None);
        else
            passBack = Regex.Replace(decompiled_text, keyword, replacement, RegexOptions.IgnoreCase);
    }
    if (DEBUGMODE) ScriptMessage("PB\n\n"+passBack);
    if (DEBUGMODE) Clipboard.SetText(passBack);
    return passBack;
}
void ReplaceTextInASM(string codeName, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null)
{
    UndertaleCode code = Data.Code.ByName(codeName);
    if (code is null)
        throw new ScriptException($"No code named \"{codeName}\" was found!");

    ReplaceTextInASM(code, keyword, replacement, caseSensitive, isRegex, context);
}
string GetASM(UndertaleCode code, GlobalDecompileContext context = null)
{
    EnsureDataLoaded();

    string passBack = "";
    string codeName = code.Name.Content;
    GlobalDecompileContext DECOMPILE_CONTEXT = context is null ? new(Data, false) : context;
    
    if (code is null)
        throw new Exception("Null code");
    string decompiled_text = Disassemble(code);
    
    if (DEBUGMODE) ScriptMessage("DT\n\n"+decompiled_text);
    if (DEBUGMODE) Clipboard.SetText(decompiled_text);

    return decompiled_text;
}
void SetASM(UndertaleCode code, string passBack) {
    try
    {
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
void ReplaceTextInASM(UndertaleCode code, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null)
{
    string f = CompileGMLFragments(replacement, code);
    if (DEBUGMODE) ScriptMessage("ASDF\n\n"+f);
    if (DEBUGMODE) Clipboard.SetText(f);
    SetASM(code, GetPassBack(GetASM(code, context), keyword, f, caseSensitive, isRegex));
}
string Disassemble(UndertaleCode code) {
    return code.Disassemble(Data.Variables, Data.CodeLocals.For(code));
}
UndertaleCode code(string codeName, bool makeIfNotExists = true) {
    UndertaleCode c = Data.Code.ByName(codeName);
    if (c == null && makeIfNotExists) {
        UndertaleCodeLocals l = new UndertaleCodeLocals();
        l.Name = Data.Strings.MakeString(codeName);
        c = new UndertaleCode();
        c.Name = l.Name;
        Data.Code.Add(c);
        Data.CodeLocals.Add(l);
    }
    return c;
}
void GMLFunction(string scode) {
    string funcname = Regex.Match(scode, @"^\s*function\s*(\w+)").Groups[1].Value;
    string filename = "gml_GlobalScript_"+funcname;
    code(filename).ReplaceGML(scode, Data);
}