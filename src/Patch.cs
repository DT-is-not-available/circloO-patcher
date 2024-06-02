using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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

namespace cpatcher;

/// <summary>
/// exception what is thrown during an error in execution of a patch
/// </summary>
public class PatchException : Exception
{
    public PatchException() { }
    public PatchException(string message) : base(message) { }
}

public class PatchGlobals
{
    /// <summary>
    /// the circloo data win
    /// </summary>
    public UndertaleData Data;

    /// <summary>
    /// path to the root of circloo
    /// </summary>
    public string CircloORootPath { get; init; }

    /// <summary>
    /// path to the currently executed patch
    /// </summary>
    public string PatchPath { get; init; }

    /// <summary>
    /// Info about current patch
    /// </summary>
    public PatchInfo Info { get; init; }

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

    /// <summary>
    /// show some message (information) to the user
    /// </summary>
    public void PatchMessage(string message)
    {
        MessageBox.Show(message, $"Message from {Info.DisplayName}", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// show some error to the user
    /// </summary>
    public void PatchError(string error)
    {
        MessageBox.Show(error, $"Error from {Info.DisplayName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// ask a yes/no question to the user
    /// </summary>
    public bool PatchQuestion(string message)
    {
        DialogResult res = MessageBox.Show(message, $"Question from {Info.DisplayName}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        return res == DialogResult.Yes;
    }

    public PatchGlobals(UndertaleData data, PatchInfo info, string rootPath, string patchPath)
    {
        Data = data;
        Info = info;
        CircloORootPath = rootPath;
        PatchPath = patchPath;
    }
}

public readonly struct PatchInfo
{
    // aka internal name, init makes it so it cant be changes afterwards. this should be unique for all patches
    // this is not the best solution, because two patches will break if they have the same name
    // for now this is what it is
    public string Name { get; init; }
    public string DisplayName { get; init; }
    public string Description { get; init; }
    public string Version { get; init; }
    public string Author { get; init; }
    public PatchInfo(string name, string displayName, string description, string version, string author)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        Version = version;
        Author = author;
    }
}

public class Patch
{
    public Guid Id { get; init; }
    public PatchInfo Info { get; init; }
    public string CodePath { get; set; }

    public Patch(string codePath, PatchInfo info)
    {
        Id = Guid.NewGuid();
        CodePath = codePath;
        Info = info;
    }

    // samples:
    // https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md
    // https://github.com/UnderminersTeam/UndertaleModTool/blob/fb312d9a2b64063dcd458b9cdec3acb06c081db0/UndertaleModTool/ScriptingFunctions.cs#L58
    // https://github.com/UnderminersTeam/UndertaleModTool/blob/fb312d9a2b64063dcd458b9cdec3acb06c081db0/UndertaleModTool/MainWindow.xaml.cs#L248
    public bool Execute()
    {
        if (!File.Exists(CodePath))
        {
            return false;
        }

        try
        {
            ScriptOptions scriptOptions = ScriptOptions.Default
                .AddImports("UndertaleModLib", "UndertaleModLib.Models", "UndertaleModLib.Decompiler",
                    "UndertaleModLib.Scripting", "UndertaleModLib.Compiler",
                    "UndertaleModTool", "System", "System.IO", "System.Collections.Generic",
                    "System.Text.RegularExpressions", "System.Linq")
                .AddReferences(
                    typeof(UndertaleObject).GetTypeInfo().Assembly, 
                    typeof(PatchException).GetTypeInfo().Assembly,
                    typeof(System.Text.RegularExpressions.Regex).GetTypeInfo().Assembly)
                .WithEmitDebugInformation(true); // when script throws an exception, add a exception location (line number)

            //ScriptOptions scriptOptions = ScriptOptions.Default;

            CancellationTokenSource source = new CancellationTokenSource(100);
            CancellationToken token = source.Token;

            PatchGlobals globals = new PatchGlobals(MainWindow.Data, Info, MainWindow.CircloORootPath, CodePath);

            object result = CSharpScript.EvaluateAsync(
                File.ReadAllText(CodePath),
                scriptOptions,
                globals,
                typeof(PatchGlobals),
                token
            );

            MainWindow.Data = globals.Data;
        }
        catch (CompilationErrorException exc)
        {
            MessageBox.Show($"Compilation error of patch {Info.DisplayName}: {exc}", $"Error from {Info.DisplayName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        catch (Exception)
        {
            return true;
        }

        return true;
    }
}
