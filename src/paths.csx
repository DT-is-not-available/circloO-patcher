using System;

string circloO_path;
string circloO_filepath;
if (Data == null) {
    circloO_path = "C:/Program Files (x86)/Steam/steamapps/common/circloO/";
    if (!Directory.Exists(circloO_path)) {
        circloO_path = "C:/Program Files/Steam/steamapps/common/circloO/";
    }
    circloO_filepath = circloO_path + "data.win";
    if (File.Exists(circloO_filepath)) {
        // await App.Current.MainWindow.LoadFile(circloO_path, false, false);
        // fuck you private methods, ill do what i want
        await LoadFile(circloO_filepath);
    } else {
        ScriptError("No data.win file is currently opened, and I couldn't find a valid installation of circloO on your computer. Please open a data.win and try running this script again.", "Error");
        return;
    }
} else {
    circloO_filepath = FilePath.Replace("\\", "/");
    circloO_path = Regex.Replace(circloO_filepath, @"(.+/).+\.win", "$1");
}

string backupsPath = circloO_path + "backups/";
if (!Directory.Exists(backupsPath)) {
    Directory.CreateDirectory(backupsPath);
}