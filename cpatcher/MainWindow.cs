using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class MainWindow : Form {
    
    bool DebugMode { get; set; };
  
    public MainWindow() {
        this.ClientSize = new Size(630, 440);
        this.ShowIcon = false;
        this.Text = "cpatcher "+this.Version;
        this.CenterToScreen();
        this.InitializeComponents();

        this.DebugMode = true;
    }

    bool Warn(string text, string title="Warning!") {
        return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
    }

    bool Question(string text, string title="Confirm?") {
        return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    void Inform(string text, string title="DT forgot to change the default title of the information box you should make fun of them") {
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    void Error(string text, string title="Error!") {
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    Button OnClick(Button button, Action<object, EventArgs> function) {
        button.Click += new EventHandler(function);
        return button;
    }
  
    void InitializeComponents() {
        // haha testing ui
        this.Controls.Add(this.OnClick(new Button {
            Text = "Hello there!",
            Size = new Size(100, 25),
            Location = new Point(10, 10),
        }, delegate (object sender, EventArgs e) {
            this.Inform("This is an information box.", this.Text);
        }));
    }
} 