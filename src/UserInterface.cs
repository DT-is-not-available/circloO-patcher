using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace cpatcher;

partial class MainWindow
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // 
        // patchList
        // 
        patchList = new ListView();
        patchList.Anchor = AnchorStyles.Top;
        patchList.CheckBoxes = true;
        patchList.Location = new Point(9, 31);
        patchList.Name = "patchList";
        patchList.Size = new Size(409, 351);
        patchList.TabIndex = 1;
        patchList.Sorting = SortOrder.Ascending;
        patchList.GridLines = true;
        patchList.Columns.Add("Name");
        patchList.Columns.Add("Version");
        patchList.Columns.Add("Author");


        // 
        // patchButton
        // 
        patchButton = new Button();
        patchButton.Anchor = AnchorStyles.Bottom;
        patchButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
        patchButton.Location = new Point(9, 388);
        patchButton.Name = "patchButton";
        patchButton.Size = new Size(196, 50);
        patchButton.TabIndex = 2;
        patchButton.Text = "Patch!";
        patchButton.UseVisualStyleBackColor = true;
        patchButton.Click += patchButton_Click;
        // 
        // launchGameButton
        // 
        launchGameButton = new Button();
        launchGameButton.Anchor = AnchorStyles.Bottom;
        launchGameButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
        launchGameButton.Location = new Point(211, 388);
        launchGameButton.Name = "launchGameButton";
        launchGameButton.Size = new Size(207, 50);
        launchGameButton.TabIndex = 3;
        launchGameButton.Text = "Launch game";
        launchGameButton.UseVisualStyleBackColor = true;
        launchGameButton.Click += launchGameButton_Click;


        // 
        // browseCircloOFilesToolStripMenuItem
        // 
        browseCircloOFilesToolStripMenuItem = new ToolStripMenuItem();
        browseCircloOFilesToolStripMenuItem.Name = "browseCircloOFilesToolStripMenuItem";
        browseCircloOFilesToolStripMenuItem.Size = new Size(224, 26);
        browseCircloOFilesToolStripMenuItem.Text = "Browse CircloO files...";
        browseCircloOFilesToolStripMenuItem.Click += browseCircloOFiles_Click;
        // 
        // browsePatcherFilesToolStripMenuItem
        // 
        browsePatcherFilesToolStripMenuItem = new ToolStripMenuItem();
        browsePatcherFilesToolStripMenuItem.Name = "browsePatcherFilesToolStripMenuItem";
        browsePatcherFilesToolStripMenuItem.Size = new Size(224, 26);
        browsePatcherFilesToolStripMenuItem.Text = "Browse Patcher files...";
        browsePatcherFilesToolStripMenuItem.Click += browsePatcherFiles_Click;
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem = new ToolStripMenuItem();
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { browseCircloOFilesToolStripMenuItem, browsePatcherFilesToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(46, 24);
        fileToolStripMenuItem.Text = "File";


        // 
        // enableAllToolStripMenuItem
        // 
        enableAllToolStripMenuItem = new ToolStripMenuItem();
        enableAllToolStripMenuItem.Name = "enableAllToolStripMenuItem";
        enableAllToolStripMenuItem.Size = new Size(224, 26);
        enableAllToolStripMenuItem.Text = "Enable all patches";
        // 
        // disableAllToolStripMenuItem
        // 
        disableAllToolStripMenuItem = new ToolStripMenuItem();
        disableAllToolStripMenuItem.Name = "disableAllToolStripMenuItem";
        disableAllToolStripMenuItem.Size = new Size(224, 26);
        disableAllToolStripMenuItem.Text = "Disable all patches";
        // 
        // editToolStripMenuItem
        // 
        editToolStripMenuItem = new ToolStripMenuItem();
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { enableAllToolStripMenuItem, disableAllToolStripMenuItem });
        editToolStripMenuItem.Name = "editToolStripMenuItem";
        editToolStripMenuItem.Size = new Size(49, 24);
        editToolStripMenuItem.Text = "Edit";


        // 
        // reloadPatchesToolStripMenuItem
        // 
        reloadPatchesToolStripMenuItem = new ToolStripMenuItem();
        reloadPatchesToolStripMenuItem.Name = "reloadPatchesToolStripMenuItem";
        reloadPatchesToolStripMenuItem.Size = new Size(224, 26);
        reloadPatchesToolStripMenuItem.Text = "Reload patch list";
        reloadPatchesToolStripMenuItem.Click += reloadPatches_Click;
        // 
        // viewToolStripMenuItem
        // 
        viewToolStripMenuItem = new ToolStripMenuItem();
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { reloadPatchesToolStripMenuItem });
        viewToolStripMenuItem.Name = "viewToolStripMenuItem";
        viewToolStripMenuItem.Size = new Size(55, 24);
        viewToolStripMenuItem.Text = "View";


        // 
        // settingsToolStripMenuItem
        // 
        settingsToolStripMenuItem = new ToolStripMenuItem();
        settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
        settingsToolStripMenuItem.Size = new Size(76, 24);
        settingsToolStripMenuItem.Text = "Settings";


        // 
        // helpToolStripMenuItem
        // 
        helpToolStripMenuItem = new ToolStripMenuItem();
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(55, 24);
        helpToolStripMenuItem.Text = "Help";

        // 
        // menuStrip1
        // 
        menuStrip1 = new MenuStrip();
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, settingsToolStripMenuItem, helpToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(430, 28);
        menuStrip1.TabIndex = 10;
        menuStrip1.Text = "menuStrip1";

        // 
        // MainWindow
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(430, 450);
        Controls.Add(launchGameButton);
        Controls.Add(patchButton);
        Controls.Add(patchList);
        Controls.Add(menuStrip1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MainMenuStrip = menuStrip1;
        Name = "MainWindow";
        StartPosition = FormStartPosition.CenterScreen; // use this instead of CenterToScreen()
        Text = "CircloO Patcher v" + PatcherVersion;
    }

    public ListView patchList;

    private Button patchButton;
    private Button launchGameButton;

    private MenuStrip menuStrip1;

    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem browseCircloOFilesToolStripMenuItem;
    private ToolStripMenuItem browsePatcherFilesToolStripMenuItem;

    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem enableAllToolStripMenuItem;
    private ToolStripMenuItem disableAllToolStripMenuItem;

    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem reloadPatchesToolStripMenuItem;

    private ToolStripMenuItem settingsToolStripMenuItem;

    private ToolStripMenuItem helpToolStripMenuItem;
}

