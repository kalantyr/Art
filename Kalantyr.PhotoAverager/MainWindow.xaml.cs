using System.Drawing.Imaging;
using System.IO;
using Kalantyr.PhotoAverager.Model;
using Microsoft.Win32;

namespace Kalantyr.PhotoAverager
{
	public partial class MainWindow
	{
		internal const string Filter = "Фотографии|*.jpg|Все файлы|*.*";

	    public MainWindow()
	    {
	        InitializeComponent();

            Loaded += (sender, e) => Work();
	    }

	    private void Work()
	    {
	    	IImageSummator s = new Cleaner();

			using (var result = s.CreateSumImage(new FirstPhotoFinder()))
	        {
	            var saveFileDialog = new SaveFileDialog {Filter = Filter, DefaultExt = ".jpg"};
	            if (saveFileDialog.ShowDialog(this) == true)
	                using (var file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
	                    result.Save(file, ImageFormat.Jpeg);
	        }
	    }
	}
}
