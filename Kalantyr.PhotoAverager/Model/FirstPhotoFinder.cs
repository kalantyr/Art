using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Kalantyr.PhotoFilter;
using Microsoft.Win32;

namespace Kalantyr.PhotoAverager.Model
{
	class FirstPhotoFinder : IPhotoFinder
	{
	    public IEnumerable<Bitmap> GetPhotos()
		{
			var openFileDialog = new OpenFileDialog { Filter = MainWindow.Filter, Multiselect = true };
            if (openFileDialog.ShowDialog() != true)
                yield break;

            foreach (var fileName in openFileDialog.FileNames)
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					yield return Load(file);
		}

		public IEnumerable<Raster> GetRasters()
		{
			var openFileDialog = new OpenFileDialog { Filter = MainWindow.Filter, Multiselect = true };
			if (openFileDialog.ShowDialog() != true)
				yield break;

			foreach (var fileName in openFileDialog.FileNames)
				using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					yield return new Raster(Load(file));
		}

		private static Bitmap Load(Stream stream)
		{
			return (Bitmap)Image.FromStream(stream);
		}
	}
}
