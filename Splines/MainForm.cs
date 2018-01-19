using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace Splines
{
	internal partial class MainForm : Form
	{
	    private FileInfo _lastFileInfo;
	    private int _counter;

		#region Constructors

		public MainForm()
		{
			InitializeComponent();
			TuneControls();
		}

		#endregion

		#region Methods

		private static Color GetRandomColor()
		{
			var r = Parameters.Random.Next(256);
			var g = Parameters.Random.Next(256);
			var b = Parameters.Random.Next(256);
			return Color.FromArgb(r, g, b);
		}

		private void DrawStars()
		{
			try
			{
				Cursor = Cursors.WaitCursor;

				Parameters.Color = GetRandomColor();

				var stars = new Star[Parameters.Random.Next(Parameters.MinStarCount, Parameters.MaxStarCount)];
				for (var i = 0; i < stars.Length; i++)
				{
					stars[i] = new Star();
					stars[i].GenerateRandom();
				}

				progressBar.Maximum = stars.Length + 1;
				var bitmap = new Bitmap(Parameters.PictureSide, Parameters.PictureSide);
				progressBar.Increment(1);
				using (var gr = Graphics.FromImage(bitmap))
				{
					gr.SmoothingMode = SmoothingMode.HighQuality;
					gr.Clear(Color.Empty);
					gr.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2);
					foreach (var star in stars)
					{
						star.Draw(gr);
						progressBar.Increment(1);
					}
				}
				if (pictureBox.Image != null)
				{
					pictureBox.Image.Dispose();
					GC.Collect();
				}
				pictureBox.Image = bitmap;
				progressBar.Value = 0;
			}
			catch (Exception exc)
			{
				MessageBox.Show(exc.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void TuneControls()
		{
			mi_File_SavePicture.Enabled = pictureBox.Image != null;
		}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled)
                return;

            if (e.KeyCode == Keys.Space)
            {
                e.Handled = true;
                mi_Splines_Generate.PerformClick();
            }
        }

		#endregion

		#region Handlers

		private void mi_Splines_Click(object sender, EventArgs e)
		{
			if (sender == mi_Splines_Generate)
			{
				DrawStars();
				TuneControls();
			}
		}

		private void mi_File_Click(object sender, EventArgs e)
		{
			if (sender == mi_File_Exit)
				Close();

            if (sender == mi_File_SavePicture)
            {
                _counter++;
                saveFileDialog.FileName = ToString(_counter) + ".png";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    _lastFileInfo = Save(saveFileDialog.FileName);
            }
		}

        private static string ToString(int counter)
        {
            var s = counter.ToString();
            while (s.Length < 2)
                s = "0" + s;
            return s;
        }

	    private FileInfo Save(string fileName)
	    {
	        FileStream stream = null;
	        try
	        {
	            Cursor = Cursors.WaitCursor;
	            stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
	            pictureBox.Image.Save(stream, ImageFormat.Png);
	            return new FileInfo(fileName);
	        }
	        catch (Exception exc)
	        {
	            MessageBox.Show(exc.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
	            return null;
	        }
	        finally
	        {
	            if (stream != null)
	            {
	                stream.Close();
	                stream.Dispose();
	            }
	            Cursor = Cursors.Default;
	        }
	    }

	    private void mi_View_Click(object sender, EventArgs e)
        {
            if (sender == mi_View_ZoomIn)
                pictureBox.Size = new Size(pictureBox.Width * 2, pictureBox.Height * 2);

            if (sender == mi_View_ZoomOut)
                pictureBox.Size = new Size(pictureBox.Width / 2, pictureBox.Height / 2);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            mi_Splines_Generate.PerformClick();
        }

        private void mi_Save_Click(object sender, EventArgs e)
        {
            if (_lastFileInfo == null)
            {
                mi_File_SavePicture.PerformClick();
                return;
            }

            string fileName;
            do
            {
                _counter++;
                fileName = Path.Combine(_lastFileInfo.Directory.FullName, ToString(_counter) + ".png");
            } while (File.Exists(fileName));
            _lastFileInfo = Save(fileName);
        }

		#endregion
	}
}