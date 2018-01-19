namespace Splines
{
	partial class MainForm
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
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.mi_File = new System.Windows.Forms.ToolStripMenuItem();
			this.mi_File_SavePicture = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mi_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.mi_View = new System.Windows.Forms.ToolStripMenuItem();
			this.mi_View_ZoomIn = new System.Windows.Forms.ToolStripMenuItem();
			this.mi_View_ZoomOut = new System.Windows.Forms.ToolStripMenuItem();
			this.mi_Splines = new System.Windows.Forms.ToolStripMenuItem();
			this.mi_Splines_Generate = new System.Windows.Forms.ToolStripMenuItem();
			this.mi_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.statusStrip.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_File,
            this.mi_View,
            this.mi_Splines,
            this.mi_Save});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(592, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// mi_File
			// 
			this.mi_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_File_SavePicture,
            this.toolStripSeparator1,
            this.mi_File_Exit});
			this.mi_File.Name = "mi_File";
			this.mi_File.Size = new System.Drawing.Size(48, 20);
			this.mi_File.Text = "Файл";
			// 
			// mi_File_SavePicture
			// 
			this.mi_File_SavePicture.Name = "mi_File_SavePicture";
			this.mi_File_SavePicture.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.mi_File_SavePicture.Size = new System.Drawing.Size(258, 22);
			this.mi_File_SavePicture.Text = "Сохранить изображение...";
			this.mi_File_SavePicture.Click += new System.EventHandler(this.mi_File_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(255, 6);
			// 
			// mi_File_Exit
			// 
			this.mi_File_Exit.Name = "mi_File_Exit";
			this.mi_File_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.mi_File_Exit.Size = new System.Drawing.Size(258, 22);
			this.mi_File_Exit.Text = "Выход";
			this.mi_File_Exit.Click += new System.EventHandler(this.mi_File_Click);
			// 
			// mi_View
			// 
			this.mi_View.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_View_ZoomIn,
            this.mi_View_ZoomOut});
			this.mi_View.Name = "mi_View";
			this.mi_View.Size = new System.Drawing.Size(39, 20);
			this.mi_View.Text = "Вид";
			// 
			// mi_View_ZoomIn
			// 
			this.mi_View_ZoomIn.Name = "mi_View_ZoomIn";
			this.mi_View_ZoomIn.Size = new System.Drawing.Size(138, 22);
			this.mi_View_ZoomIn.Text = "Увеличить";
			this.mi_View_ZoomIn.Click += new System.EventHandler(this.mi_View_Click);
			// 
			// mi_View_ZoomOut
			// 
			this.mi_View_ZoomOut.Name = "mi_View_ZoomOut";
			this.mi_View_ZoomOut.Size = new System.Drawing.Size(138, 22);
			this.mi_View_ZoomOut.Text = "Уменьшить";
			this.mi_View_ZoomOut.Click += new System.EventHandler(this.mi_View_Click);
			// 
			// mi_Splines
			// 
			this.mi_Splines.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Splines_Generate});
			this.mi_Splines.Name = "mi_Splines";
			this.mi_Splines.Size = new System.Drawing.Size(61, 20);
			this.mi_Splines.Text = "Сплайн";
			// 
			// mi_Splines_Generate
			// 
			this.mi_Splines_Generate.Name = "mi_Splines_Generate";
			this.mi_Splines_Generate.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mi_Splines_Generate.Size = new System.Drawing.Size(143, 22);
			this.mi_Splines_Generate.Text = "Рисовать";
			this.mi_Splines_Generate.Click += new System.EventHandler(this.mi_Splines_Click);
			// 
			// mi_Save
			// 
			this.mi_Save.Name = "mi_Save";
			this.mi_Save.Size = new System.Drawing.Size(43, 20);
			this.mi_Save.Text = "Save";
			this.mi_Save.Click += new System.EventHandler(this.mi_Save_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.Color.Black;
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(256, 256);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox.TabIndex = 1;
			this.pictureBox.TabStop = false;
			this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar});
			this.statusStrip.Location = new System.Drawing.Point(0, 344);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(592, 22);
			this.statusStrip.TabIndex = 2;
			this.statusStrip.Text = "statusStrip1";
			// 
			// progressBar
			// 
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(400, 16);
			this.progressBar.Step = 1;
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "tif";
			this.saveFileDialog.Filter = "PNG|*.png|Все файлы|*.*";
			this.saveFileDialog.Title = "Сохранение изображения";
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.pictureBox);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(592, 320);
			this.panel1.TabIndex = 3;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(592, 366);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.MinimumSize = new System.Drawing.Size(200, 148);
			this.Name = "MainForm";
			this.Text = "Splines";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem mi_File;
		private System.Windows.Forms.ToolStripMenuItem mi_File_Exit;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripMenuItem mi_Splines;
		private System.Windows.Forms.ToolStripMenuItem mi_Splines_Generate;
		private System.Windows.Forms.ToolStripMenuItem mi_File_SavePicture;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripMenuItem mi_View;
		private System.Windows.Forms.ToolStripMenuItem mi_View_ZoomIn;
		private System.Windows.Forms.ToolStripMenuItem mi_View_ZoomOut;
		private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripMenuItem mi_Save;
	}
}

