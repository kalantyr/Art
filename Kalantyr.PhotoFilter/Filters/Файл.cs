using System;
using System.Drawing;

namespace Kalantyr.PhotoFilter.Filters
{
	[Filter("Файл", "Загружает картинку из файла")]
	public class Файл: FilterBase
	{
		private НастройкиФайла _settingsControl;

		public string FileName {
			get;
			set;
		}

		public int GetPoints {
			get;
			set;
		}

		public override System.Windows.Controls.UserControl SettingsControl
		{
			get { return _settingsControl ?? (_settingsControl = new НастройкиФайла {DataContext = this}); }
		}

		public Файл()
		{
			FileName = string.Empty;
		}

		public override void Work(Bitmap bitmap)
		{
			if (bitmap == null) throw new ArgumentNullException("bitmap");

			using (var image = LoadImage(FileName))
			{
				if (GetPoints == 0)
					using (var g = CreateGraphics(bitmap)) {
						var a = (double)bitmap.Width / image.Width;
						var b = (double)bitmap.Height / image.Height;
						var w = (int)(Math.Min(a, b) * image.Width);
						var h = (int)(Math.Min(a, b) * image.Height);
						g.DrawImage(image, (bitmap.Width - w) / 2, (bitmap.Height - h) / 2, w, h);
					}
				else
					using (var fileRaster = new Raster(image, true, false))
					using (var destRaster = new Raster(bitmap))
					{
						var average = GetAverageAlpha(fileRaster);

						var count = 0;
						while (count < GetPoints)
						{
							var x = Rand.Next(fileRaster.Width);
							var y = Rand.Next(fileRaster.Height);
							var pixel = fileRaster.GetPixel(x, y);
							if (pixel.A >= average)
							{
								destRaster.SetPixel(x, y, pixel);
								count++;
							}
						}
					}
			}
		}

		private static int GetAverageAlpha(Raster raster)
		{
			var minAlpha = int.MaxValue;
			var maxAlpha = int.MinValue;
			for (var y = 0; y < raster.Height; y++)
				for (var x = 0; x < raster.Width; x++)
				{
					var alpha = raster.GetAlpha(x, y);
					if (alpha < minAlpha)
						minAlpha = alpha;
					if (alpha > maxAlpha)
						maxAlpha = alpha;
				}
			return (minAlpha + maxAlpha)/2;
		}
	}
}
