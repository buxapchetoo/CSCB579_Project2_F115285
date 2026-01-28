namespace CSCB579_Project2_F115285
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bulgarianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerAnim = new System.Windows.Forms.Timer(this.components);
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabDraw = new System.Windows.Forms.TabPage();
            this.pictureBoxCanvas = new System.Windows.Forms.PictureBox();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupColor = new System.Windows.Forms.GroupBox();
            this.cbBlue = new System.Windows.Forms.CheckBox();
            this.cbRed = new System.Windows.Forms.CheckBox();
            this.groupSettings = new System.Windows.Forms.GroupBox();
            this.rbArc = new System.Windows.Forms.RadioButton();
            this.rbLine = new System.Windows.Forms.RadioButton();
            this.rbFreeDraw = new System.Windows.Forms.RadioButton();
            this.panelMain = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabDraw.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).BeginInit();
            this.tabSettings.SuspendLayout();
            this.groupColor.SuspendLayout();
            this.groupSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.languageToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openImageToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // openImageToolStripMenuItem
            // 
            resources.ApplyResources(this.openImageToolStripMenuItem, "openImageToolStripMenuItem");
            this.openImageToolStripMenuItem.Name = "openImageToolStripMenuItem";
            this.openImageToolStripMenuItem.Click += new System.EventHandler(this.openImageToolStripMenuItem_Click);
            // 
            // saveImageToolStripMenuItem
            // 
            resources.ApplyResources(this.saveImageToolStripMenuItem, "saveImageToolStripMenuItem");
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // languageToolStripMenuItem
            // 
            resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bulgarianToolStripMenuItem,
            this.englishToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            // 
            // bulgarianToolStripMenuItem
            // 
            resources.ApplyResources(this.bulgarianToolStripMenuItem, "bulgarianToolStripMenuItem");
            this.bulgarianToolStripMenuItem.Name = "bulgarianToolStripMenuItem";
            this.bulgarianToolStripMenuItem.Click += new System.EventHandler(this.bulgarianToolStripMenuItem_Click);
            // 
            // englishToolStripMenuItem
            // 
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // timerAnim
            // 
            this.timerAnim.Enabled = true;
            // 
            // tabMain
            // 
            resources.ApplyResources(this.tabMain, "tabMain");
            this.tabMain.Controls.Add(this.tabDraw);
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            // 
            // tabDraw
            // 
            resources.ApplyResources(this.tabDraw, "tabDraw");
            this.tabDraw.Controls.Add(this.pictureBoxCanvas);
            this.tabDraw.Name = "tabDraw";
            this.tabDraw.UseVisualStyleBackColor = true;
            // 
            // pictureBoxCanvas
            // 
            resources.ApplyResources(this.pictureBoxCanvas, "pictureBoxCanvas");
            this.pictureBoxCanvas.BackColor = System.Drawing.Color.White;
            this.pictureBoxCanvas.Name = "pictureBoxCanvas";
            this.pictureBoxCanvas.TabStop = false;
            this.pictureBoxCanvas.Click += new System.EventHandler(this.pictureBoxCanvas_Click);
            this.pictureBoxCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxCanvas_MouseDown);
            this.pictureBoxCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxCanvas_MouseMove);
            this.pictureBoxCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxCanvas_MouseUp);
            // 
            // tabSettings
            // 
            resources.ApplyResources(this.tabSettings, "tabSettings");
            this.tabSettings.Controls.Add(this.btnClear);
            this.tabSettings.Controls.Add(this.groupColor);
            this.tabSettings.Controls.Add(this.groupSettings);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // groupColor
            // 
            resources.ApplyResources(this.groupColor, "groupColor");
            this.groupColor.Controls.Add(this.cbBlue);
            this.groupColor.Controls.Add(this.cbRed);
            this.groupColor.Name = "groupColor";
            this.groupColor.TabStop = false;
            // 
            // cbBlue
            // 
            resources.ApplyResources(this.cbBlue, "cbBlue");
            this.cbBlue.Checked = true;
            this.cbBlue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBlue.Name = "cbBlue";
            this.cbBlue.UseVisualStyleBackColor = true;
            // 
            // cbRed
            // 
            resources.ApplyResources(this.cbRed, "cbRed");
            this.cbRed.Checked = true;
            this.cbRed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRed.Name = "cbRed";
            this.cbRed.UseVisualStyleBackColor = true;
            // 
            // groupSettings
            // 
            resources.ApplyResources(this.groupSettings, "groupSettings");
            this.groupSettings.Controls.Add(this.rbArc);
            this.groupSettings.Controls.Add(this.rbLine);
            this.groupSettings.Controls.Add(this.rbFreeDraw);
            this.groupSettings.Name = "groupSettings";
            this.groupSettings.TabStop = false;
            // 
            // rbArc
            // 
            resources.ApplyResources(this.rbArc, "rbArc");
            this.rbArc.Name = "rbArc";
            this.rbArc.UseVisualStyleBackColor = true;
            // 
            // rbLine
            // 
            resources.ApplyResources(this.rbLine, "rbLine");
            this.rbLine.Name = "rbLine";
            this.rbLine.UseVisualStyleBackColor = true;
            // 
            // rbFreeDraw
            // 
            resources.ApplyResources(this.rbFreeDraw, "rbFreeDraw");
            this.rbFreeDraw.Checked = true;
            this.rbFreeDraw.Name = "rbFreeDraw";
            this.rbFreeDraw.TabStop = true;
            this.rbFreeDraw.UseVisualStyleBackColor = true;
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabDraw.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.groupColor.ResumeLayout(false);
            this.groupColor.PerformLayout();
            this.groupSettings.ResumeLayout(false);
            this.groupSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bulgarianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openImageToolStripMenuItem;
        private System.Windows.Forms.Timer timerAnim;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabDraw;
        private System.Windows.Forms.PictureBox pictureBoxCanvas;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupColor;
        private System.Windows.Forms.CheckBox cbBlue;
        private System.Windows.Forms.CheckBox cbRed;
        private System.Windows.Forms.GroupBox groupSettings;
        private System.Windows.Forms.RadioButton rbArc;
        private System.Windows.Forms.RadioButton rbLine;
        private System.Windows.Forms.RadioButton rbFreeDraw;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
    }
}

