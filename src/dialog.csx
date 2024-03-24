using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

public class DMenuItem {
    public string Label;
    public Action Handler;
    public DMenuItem(string label, Action handler) {
        this.Label = label;
        this.Handler = handler;
    }
}

public partial class cirQOLdialog : System.Windows.Forms.Form
{
    public System.Windows.Forms.CheckedListBox patchlist_ui;
    public System.Windows.Forms.Button AcceptButton;
    public System.Windows.Forms.Button CancelButton;
    public System.Windows.Forms.ListBox installMode_ui;
    public System.Windows.Forms.MenuStrip Menu;
    public System.ComponentModel.Container components;
    public List<string> selectedPatches;
    public List<string> patches;
    public List<string> patchids;
    public List<string> presets;
    public List<bool> requiredpatch;
    public List<List<string>> presetpatches;
    public bool isinput = false;
    public int offset;
    public string circloO_filepath;
    public UndertaleData Data;

    public cirQOLdialog(List<string> selectedPatches, List<string> patches, List<string> patchids, List<string> presets, List<List<string>> presetpatches, string circloO_filepath, UndertaleData Data, List<bool> requiredpatch) {
        this.Data = Data;
        this.circloO_filepath = circloO_filepath;
        this.selectedPatches = selectedPatches;
        this.patches = patches;
        this.patchids = patchids;
        this.presets = presets;
        this.presetpatches = presetpatches;
        this.offset = 0;
        this.requiredpatch = requiredpatch;

        InitializeComponent();

        for (int i = this.offset; i < this.patches.Count; i++) {
            patchlist_ui.Items.Add(this.patches[i]);
        }

        for (int i = 0; i < this.presets.Count; i++) {
            installMode_ui.Items.Add(this.presets[i]);
        }

        installMode_ui.SelectedIndex = presets.IndexOf("Recommended");

        this.isinput = true;

        // Changes the selection mode from double-click to single click.
        patchlist_ui.CheckOnClick = true;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.CenterToScreen();
    }

    public void updatePatchChecklist() {
        this.isinput = false;
        for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
            if (this.requiredpatch[i + this.offset]) {
                patchlist_ui.SetItemCheckState(i, CheckState.Indeterminate);
            } else {
                patchlist_ui.SetItemChecked(i, presetpatches[installMode_ui.SelectedIndex].Contains(this.patchids[i + this.offset]));
            }
        }
        this.isinput = true;
    }

    public void updatePatchChecklist(List<string> preset) {
        this.isinput = false;
        for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
            patchlist_ui.SetItemChecked(i, preset.Contains(this.patchids[i + this.offset]));
        }
        this.isinput = true;
        try {
            if (this.isinput) this.installMode_ui.ClearSelected();
        } catch (Exception error) {

        }
    }

    protected override void Dispose( bool disposing )
    {
        if( disposing )
        {
            if (components != null) 
            {
                components.Dispose();
            }
        }
        base.Dispose( disposing );
    }

    public void patchlist_ui_ItemCheck(object sender, ItemCheckEventArgs e) {
        if (e.CurrentValue == CheckState.Indeterminate) e.NewValue = CheckState.Indeterminate;
    }

    public void styleButton(System.Windows.Forms.Button button, bool disabled = false) {
        button.FlatStyle = FlatStyle.Flat;

        if (disabled) {
            button.BackColor = Color.LightGray;
            button.ForeColor = Color.DarkGray;
        } else {
            button.BackColor = Color.FromArgb(234, 234, 234);
            button.ForeColor = Color.Black;
        }
        
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 234, 255);

        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 214, 235);
    }

    public void styleButtonGreen(System.Windows.Forms.Button button, bool disabled = false) {
        button.FlatStyle = FlatStyle.Flat;

        if (disabled) {
            button.BackColor = Color.LightGray;
            button.ForeColor = Color.DarkGray;
        } else {
            button.BackColor = Color.FromArgb(234, 234, 234);
            button.ForeColor = Color.Black;
        }

        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 255, 200);

        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 235, 180);
    }

    public void InitializeComponent()
    {
        this.BackColor = Color.White;
        this.initMenuBar();
        this.ShowIcon = false;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.components = new System.ComponentModel.Container();
        this.patchlist_ui = new System.Windows.Forms.CheckedListBox();
        this.installMode_ui = new System.Windows.Forms.ListBox();
        this.AcceptButton = new System.Windows.Forms.Button();
        this.CancelButton = new System.Windows.Forms.Button();
        this.patchlist_ui.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.itemCheck);
        this.patchlist_ui.IntegralHeight = false;
        this.patchlist_ui.BorderStyle = BorderStyle.FixedSingle;
        this.patchlist_ui.Location = new System.Drawing.Point(220, 110);
        this.patchlist_ui.Size = new System.Drawing.Size(400, 250);
        this.patchlist_ui.TabIndex = 1;
        this.patchlist_ui.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.patchlist_ui_ItemCheck);
        this.patchlist_ui.ItemHeight = 20;
        this.installMode_ui.BorderStyle = BorderStyle.FixedSingle;
        this.installMode_ui.IntegralHeight = false;
        this.installMode_ui.Location = new System.Drawing.Point(10, 110);
        this.installMode_ui.Size = new System.Drawing.Size(200, 250);
        this.installMode_ui.TabIndex = 0;
        this.installMode_ui.SelectedValueChanged += new EventHandler(this.presetChanged);
        this.installMode_ui.ItemHeight = 20;
        this.AcceptButton.Enabled = true;
        this.AcceptButton.Location = new System.Drawing.Point(220, 370);
        this.AcceptButton.Size = new System.Drawing.Size(400, 30);
        this.AcceptButton.TabIndex = 3;
        this.AcceptButton.Text = "Patch!";
        this.AcceptButton.Click += new System.EventHandler(this.AcceptButton_Click);
        this.styleButtonGreen(this.AcceptButton);
        this.CancelButton.Enabled = true;
        this.CancelButton.Location = new System.Drawing.Point(10, 370);
        this.CancelButton.Size = new System.Drawing.Size(200, 30);
        this.CancelButton.TabIndex = 2;
        this.CancelButton.Text = "Cancel";
        this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
        this.styleButton(this.CancelButton);
        this.ClientSize = new System.Drawing.Size(630, 440);
        this.Controls.AddRange(new System.Windows.Forms.Control[] {
            this.installMode_ui,
            this.AcceptButton,
            this.CancelButton,
            this.patchlist_ui
        });
        this.Text = "cirQOL Patcher "+VERSION;
        
        makeLabel(@"cirQOL is a circloO mod that aims to give small QOL improvements in both the main game and the level editor.
You can customize the patches that get applied below, or just hit 'Patch!' to apply the defaults.", 10, 40);

        makeLabel("Presets:", 10, 90);
        makeLabel("Customize Patches:", 220, 90);

        Label credit = new Label();
        credit.Text = "Created by DT";
        credit.AutoSize = true;
        credit.Location = new Point(315 - credit.Width / 2, 415);
        this.Controls.Add(credit);
        
        this.updatePatchChecklist();
    }

    public Label makeLabel(string text, int x, int y) {
        Label label = new Label(); 
        label.Text = text; 
        label.Location = new Point(x, y); 
        label.AutoSize = true;
        this.Controls.Add(label);
        return label;
    }

    public void presetChanged(object sender, EventArgs e)
    {
        this.updatePatchChecklist();
    }

    public bool Warn(string text, string title="Warning!") {
        return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
    }

    public bool Question(string text, string title="Confirm?") {
        return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    public void Inform(string text, string title="DT forgot to change the default title of the information box you should make fun of them") {
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void Error(string text, string title="Error!") {
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Adds the string if the text box has data in it.
    async public void AcceptButton_Click(object sender, System.EventArgs e)
    {
        UndertaleData Data = this.Data;
        string circloO_path = Regex.Replace(this.circloO_filepath, @"(.+/).+\.win", "$1");
        string backupsPath = circloO_path + "backups/";
        string backupName = backupsPath + Data.GeneralInfo.FileName.Content + "-" + Data.GeneralInfo.Timestamp + ".win";
        bool hasbackup = false;
        bool skipwarn = false;
        bool shouldcontinue = false;

        Func<bool, bool> doWarnings = (dontgiveup) => {
            if (dontgiveup) {
                if (skipwarn) {
                    if (ISBETA != 0) {
                        return this.Question(BETAWARNTEXT+CONFIRMTEXT);
                    } else return true;
                } else {
                    if (hasbackup) {
                        if (ISBETA != 0) {
                            return this.Question(BETAWARNTEXT+CONFIRMTEXT);
                        } else {
                            return this.Question(CONFIRMTEXT);
                        }
                    } else {
                        if (ISBETA != 0) {
                            return this.Warn(BETAWARNTEXT+WARNTEXT);
                        } else {
                            return this.Warn(WARNTEXT);
                        }
                    }
                }
            } else return false;
        };

        if (Data.Code.ByName("hasbeenmodded") != null) {
            if (File.Exists(backupName)) {
                if (shouldcontinue = doWarnings(this.Question("You must restore your previously created backup in order to modify the game. Do this and apply changes?", "Restore backup?"))) {
                    hasbackup = true;
                    skipwarn = true;
                    File.Copy(backupName, circloO_filepath, true);
                    // await App.Current.MainWindow.LoadFile(circloO_path, false, false);
                    // fuck you private methods, ill do what i want
                    await LoadFile(circloO_filepath);
                } else {
                    return;
                }
            } else {
                MessageBox.Show("I couldn't find an automatically created backup in your backups folder. If you have a manually created backup, please copy it to the game files and try again.", "Backup not found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        } else {
            if (!File.Exists(backupName)) {
                if (shouldcontinue = this.Question("Would you like to automatically create a backup of your game? It is recommended that you hit yes so the tool can automatically revert the changes if need be.", "Create backup?")) {
                    File.Copy(circloO_filepath, backupName, true);
                } else return;
            }
            shouldcontinue = doWarnings(hasbackup = true);
        }

        if (!shouldcontinue) return;
        
        for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
            if (this.patchlist_ui.CheckedItems.Contains(this.patchlist_ui.Items[i])) {
                this.selectedPatches.Add(this.patchids[i+this.offset]);
            }
        }
        this.Dispose(true);
        this.Close();
    }

    // Adds the string if the text box has data in it.
    public void CancelButton_Click(object sender, System.EventArgs e)
    {
        this.Dispose(true);
        this.Close();
    }

    public void initMenuBar() {

        MenuStrip ms = new MenuStrip();
        ms.BackColor = Color.FromArgb(244, 244, 244);
        this.Menu = ms;

        this.addMenu("File", new DMenuItem[] {
            new DMenuItem("Import patch configuration", () => {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Text file|*.txt";
                openDialog.Title = "Export patch configuration";
                openDialog.ShowDialog();
                if(openDialog.FileName != "") {
                    List<string> importpatchids = new List<string> (File.ReadAllText(openDialog.FileName).Split("\n"));
                    this.updatePatchChecklist(importpatchids);
                    this.Inform("Patch configuration imported!", "You can close this window now");
                }
            }),
            new DMenuItem("Export patch configuration", () => {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Text file|*.txt";
                saveDialog.Title = "Export patch configuration";
                saveDialog.ShowDialog();
                if(saveDialog.FileName != "") {
                    List<string> exportpatchids = new List<string> {};
                    for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
                        if (this.patchlist_ui.CheckedItems.Contains(this.patchlist_ui.Items[i])) {
                            exportpatchids.Add(this.patchids[i+this.offset]);
                        }
                    }
                    File.WriteAllText(saveDialog.FileName, String.Join("\n", exportpatchids));
                    this.Inform("Current patch configuration exported!", "You can close this window now");
                }
            }),
        });

        this.addMenu("Help", new DMenuItem[] {
            new DMenuItem("About", () => {
                string computedversion = (ISBETA != 0 ? CIRCLOO_VERSION + " beta "+ISBETA : CIRCLOO_VERSION);
                this.Inform(
$@"cirQOL Patcher {VERSION}
Made for circloO version {computedversion}
Fun fact I started making this right after surgery because I had nothing better to do",

"About");
            }),
            new DMenuItem("Credits", () => {
                this.Inform(
$@"cirQOL and cirQOL Patcher created by DT with lots of love
and pain
lots of pain
(c# is the worst language i have ever had to deal with)

Thanks Ewoly for the name idea because quite frankly naming this thing 'circloO editor hack and mod patcher' is not a good idea",

"Credits");
            }),
        });

        ms.Dock = DockStyle.Top;
        ms.Padding = new Padding(0);

        this.Controls.Add(ms);
    }

    public void addMenu(string menuname, DMenuItem[] items) {
        ToolStripMenuItem menu = new ToolStripMenuItem(menuname);
        this.Menu.MdiWindowListItem = menu;
        this.Menu.Items.Add(menu);
        for (int i = 0; i < items.Length; i++) {
            Action Handler = items[i].Handler;
            menu.DropDownItems.Add(new ToolStripMenuItem(items[i].Label, null, (object sender, System.EventArgs e) => {
                // MessageBox.Show("thingy was clicked!!!", "hurrah!!!!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Handler();
            }));
        }
        ((ToolStripDropDownMenu)(menu.DropDown)).ShowImageMargin = false;
        ((ToolStripDropDownMenu)(menu.DropDown)).ShowCheckMargin = true;
    }

    // when checked
    public void itemCheck(object sender, ItemCheckEventArgs e) {
        
    }
}