using System;
using System.Threading.Tasks;
using System.Reflection;

Task LoadFile(string path) {
    object[] args = new object[] {path, false, false};
    Task result = (Task) typeof(MainWindow).GetMethod("LoadFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
    return result;
}

Task SaveFile(string path) {
    object[] args = new object[] {path, false};
    Task result = (Task) typeof(MainWindow).GetMethod("SaveFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
    return result;
}

public partial class cirQOLdialog {
    public Task LoadFile(string path) {
        object[] args = new object[] {path, false, false};
        Task result = (Task) typeof(MainWindow).GetMethod("LoadFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
        return result;
    }

    public Task SaveFile(string path) {
        object[] args = new object[] {path, false};
        Task result = (Task) typeof(MainWindow).GetMethod("SaveFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
        return result;
    }
}