using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

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

    public ListView patchList = new();

    private TextBox searchTextBox = new();
    private Button reloadPatchesButton = new();
    private Button patchButton = new();
    private Button launchGameButton = new();

    private MenuStrip menuStrip1 = new();

    private ToolStripMenuItem fileToolStripMenuItem = new();
    private ToolStripMenuItem browseCircloOFilesToolStripMenuItem = new();
    private ToolStripMenuItem browsePatcherFilesToolStripMenuItem = new();

    private ToolStripMenuItem editToolStripMenuItem = new();
    private ToolStripMenuItem enableAllToolStripMenuItem = new();
    private ToolStripMenuItem disableAllToolStripMenuItem = new();

    private ToolStripMenuItem viewToolStripMenuItem = new();
    private ToolStripMenuItem reloadPatchesToolStripMenuItem = new();

    private ToolStripMenuItem settingsToolStripMenuItem = new();

    private ToolStripMenuItem helpToolStripMenuItem = new();

    private void InitializeComponent()
    {
        // 
        // patchList
        // 
        patchList.Anchor = AnchorStyles.Top;
        patchList.CheckBoxes = true;
        patchList.Scrollable = true;
        patchList.Location = new Point(9, 70); // 31
        patchList.Name = "patchList";
        patchList.Size = new Size(409, 300); // 351
        patchList.TabIndex = 1;
        patchList.Sorting = SortOrder.Ascending;
        patchList.ShowItemToolTips = true;
        patchList.View = View.Details;
        patchList.ItemChecked += patchList_ItemChecked;
        //patchList.GridLines = true;
        //patchList.LabelWrap = true;

        ColumnHeader patchListNameHeader = new ColumnHeader();
        patchListNameHeader.Text = "Name";
        patchListNameHeader.Width = -2;
        patchList.Columns.Add(patchListNameHeader);

        ColumnHeader patchListVersionHeader = new ColumnHeader();
        patchListVersionHeader.Text = "Version";
        patchListVersionHeader.Width = -2;
        patchList.Columns.Add(patchListVersionHeader);

        //ColumnHeader patchListAuthorHeader = new ColumnHeader();
        //patchListAuthorHeader.Text = "Author";
        //patchListAuthorHeader.Width = -2;
        //patchList.Columns.Add(patchListAuthorHeader);

        // 
        // searchTextBox
        // 
        searchTextBox.Anchor = AnchorStyles.Top;
        searchTextBox.Location = new Point(9, 31);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.PlaceholderText = "Search";
        searchTextBox.Size = new Size(380, 70);
        searchTextBox.TabIndex = 2;
        searchTextBox.TextChanged += searchTextBox_TextChanged;

        // 
        // reloadPatchesButton
        // 
        reloadPatchesButton.Anchor = AnchorStyles.Top;
        reloadPatchesButton.Location = new Point(390, 31);
        reloadPatchesButton.Name = "reloadPatchesButton";
        reloadPatchesButton.Size = new Size(28, 28);
        reloadPatchesButton.TabIndex = 2;
        reloadPatchesButton.Click += reloadPatches_Click;
        reloadPatchesButton.Image = Resource1.reload32.ToBitmap().GetThumbnailImage(12, 12, null, IntPtr.Zero);
        reloadPatchesButton.ImageAlign = ContentAlignment.MiddleCenter;
        reloadPatchesButton.TextAlign = ContentAlignment.MiddleCenter;


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
        viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { reloadPatchesToolStripMenuItem });
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
        Controls.Add(menuStrip1);
        Controls.Add(patchList);
        Controls.Add(searchTextBox);
        Controls.Add(reloadPatchesButton);
        Controls.Add(launchGameButton);
        Controls.Add(patchButton);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MainMenuStrip = menuStrip1;
        Name = "MainWindow";
        StartPosition = FormStartPosition.CenterScreen; // use this instead of CenterToScreen()
        Text = "CircloO Patcher v" + PatcherVersion;
        MaximizeBox = false;
        //ControlBox = false;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        //Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath);
        //Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("cpatcher.icon96.ico"));
        Icon = Resource1.icon96;
    }
}

