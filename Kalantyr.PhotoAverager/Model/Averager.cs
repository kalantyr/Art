using System;
using System.Drawing;
using System.Drawing.Imaging;
using Kalantyr.PhotoFilter;

namespace Kalantyr.PhotoAverager.Model
{
	class Averager: IImageSummator
	{
		public Bitmap CreateSumImage(IPhotoFinder photoFinder)
		{
			if (photoFinder == null) throw new ArgumentNullException("photoFinder");

			var redMatrix = new int[0, 0];
			var greenMatrix = new int[0, 0];
			var blueMatrix = new int[0, 0];
			var size = Size.Empty;
			var count = 0;
			foreach (var photo in photoFinder.GetPhotos())
			{
				if (count == 0)
				{
					size = new Size(photo.Width, photo.Height);
					redMatrix = new int[size.Height, size.Width];
					greenMatrix = new int[size.Height, size.Width];
					blueMatrix = new int[size.Height, size.Width];
				}

				if (size.Width != photo.Width || size.Height != photo.Height)
					throw new InvalidOperationException("Разный размер фотографий.");

				Calculate(photo, size, redMatrix, greenMatrix, blueMatrix);

                photo.Dispose();

				count++;
			}

			if (count == 0)
				throw new InvalidOperationException("Не найдено ни одной фотографии.");

			return GetResult(count, size, redMatrix, greenMatrix, blueMatrix);
		}

		private static void Calculate(Bitmap photo, Size size, int[,] redMatrix, int[,] greenMatrix, int[,] blueMatrix)
		{
			using (var raster = new Raster(photo, true, false))
				for (var y = 0; y < size.Height; y++)
					for (var x = 0; x < size.Width; x++) {
						var color = raster.GetPixel(x, y);
						redMatrix[y, x] += color.R;
						greenMatrix[y, x] += color.G;
						blueMatrix[y, x] += color.B;
					}
		}

		private static Bitmap GetResult(int count, Size size, int[,] redMatrix, int[,] greenMatrix, int[,] blueMatrix)
		{
			var result = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);

			using (var raster = new Raster(result, false))
				for (var y = 0; y < size.Height; y++)
					for (var x = 0; x < size.Width; x++) {
						var ar = Math.Round((double)redMatrix[y, x] / count);
						var ag = Math.Round((double)greenMatrix[y, x] / count);
						var ab = Math.Round((double)blueMatrix[y, x] / count);
						raster.SetPixel(x, y, Color.FromArgb((byte)ar, (byte)ag, (byte)ab));
					}

			return result;
		}
	}
}
