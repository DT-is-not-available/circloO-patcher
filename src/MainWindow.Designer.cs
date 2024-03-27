namespace cpatcher
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TreeNode treeNode1 = new TreeNode("Experimental");
            TreeNode treeNode2 = new TreeNode("Reccomended");
            TreeNode treeNode3 = new TreeNode("Presets", new TreeNode[] { treeNode1, treeNode2 });
            TreeNode treeNode4 = new TreeNode("patches go brrrrrrrrrrrrr");
            TreeNode treeNode5 = new TreeNode("Patches", new TreeNode[] { treeNode4 });
            patchList = new TreeView();
            patchButton = new Button();
            launchGameButton = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            browseFilesToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            enableAllToolStripMenuItem = new ToolStripMenuItem();
            disableAllToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // patchList
            // 
            patchList.Anchor = AnchorStyles.Top;
            patchList.CheckBoxes = true;
            patchList.Location = new Point(9, 31);
            patchList.Name = "patchList";
            treeNode1.Name = "Experimental";
            treeNode1.Text = "Experimental";
            treeNode2.Name = "Reccomended";
            treeNode2.Text = "Reccomended";
            treeNode3.Name = "Presets";
            treeNode3.Text = "Presets";
            treeNode4.Name = "Node2";
            treeNode4.Text = "patches go brrrrrrrrrrrrr";
            treeNode4.ToolTipText = "My patch";
            treeNode5.Name = "Patches";
            treeNode5.Text = "Patches";
            patchList.Nodes.AddRange(new TreeNode[] { treeNode3, treeNode5 });
            patchList.Size = new Size(409, 351);
            patchList.TabIndex = 1;
            patchList.AfterSelect += patchList_AfterSelect;
            // 
            // patchButton
            // 
            patchButton.Anchor = AnchorStyles.Bottom;
            patchButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            patchButton.Location = new Point(9, 388);
            patchButton.Name = "patchButton";
            patchButton.Size = new Size(196, 50);
            patchButton.TabIndex = 2;
            patchButton.Text = "Patch!";
            patchButton.UseVisualStyleBackColor = true;
            // 
            // launchGameButton
            // 
            launchGameButton.Anchor = AnchorStyles.Bottom;
            launchGameButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            launchGameButton.Location = new Point(211, 388);
            launchGameButton.Name = "launchGameButton";
            launchGameButton.Size = new Size(207, 50);
            launchGameButton.TabIndex = 3;
            launchGameButton.Text = "Launch game";
            launchGameButton.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, settingsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(430, 28);
            menuStrip1.TabIndex = 10;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { browseFilesToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { enableAllToolStripMenuItem, disableAllToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(49, 24);
            editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(55, 24);
            viewToolStripMenuItem.Text = "View";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(76, 24);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // browseFilesToolStripMenuItem
            // 
            browseFilesToolStripMenuItem.Name = "browseFilesToolStripMenuItem";
            browseFilesToolStripMenuItem.Size = new Size(224, 26);
            browseFilesToolStripMenuItem.Text = "Browse files...";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(55, 24);
            helpToolStripMenuItem.Text = "Help";
            // 
            // enableAllToolStripMenuItem
            // 
            enableAllToolStripMenuItem.Name = "enableAllToolStripMenuItem";
            enableAllToolStripMenuItem.Size = new Size(224, 26);
            enableAllToolStripMenuItem.Text = "Enable all patches";
            enableAllToolStripMenuItem.Click += enableAllToolStripMenuItem_Click;
            // 
            // disableAllToolStripMenuItem
            // 
            disableAllToolStripMenuItem.Name = "disableAllToolStripMenuItem";
            disableAllToolStripMenuItem.Size = new Size(224, 26);
            disableAllToolStripMenuItem.Text = "Disable all patches";
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
            StartPosition = FormStartPosition.CenterScreen;
            Text = " CircloO Patcher";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TreeView patchList;
        private Button patchButton;
        private Button launchGameButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem browseFilesToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem enableAllToolStripMenuItem;
        private ToolStripMenuItem disableAllToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
    }
}
