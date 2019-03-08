using System.IO;
using Microsoft.Win32;

namespace Kalantyr.PhotoFilter.Filters
{
	public partial class НастройкиФайла
	{
		public НастройкиФайла()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var файл = (Файл) DataContext;

			var dialog = new OpenFileDialog
			             	{
			             		Filter = "Картинки|*.jpg;*.png;*.bmp|Все файлы|*.*"
			             	};
			if (File.Exists(файл.FileName))
				dialog.FileName = файл.FileName;
			if (dialog.ShowDialog() == true)
			{
				файл.FileName = dialog.FileName;
				fileNameTextBox.Text = dialog.FileName;
			}
		}
	}
}
