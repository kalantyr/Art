using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Kalantyr.PhotoFilter.Filters
{
	[Filter("Растекание", "Создает эффект растекания исходного изображения")]
	public class Растекание: FilterBase
	{
		private FirstPropertiesControl _settingsControl;
		
		public int ПорогТаяния {
			get;
			set;
		}

		public int ИзменениеЦвета
		{
			get;
			set;
		}

		public bool ChangeColor {
			get;
			set;
		}

		public int Таяние {
			get;
			set;
		}

		public int КоличествоИтераций
		{
			get;
			set;
		}

		public Растекание()
		{
			ИзменениеЦвета = 2;
			КоличествоИтераций = 25;
			Таяние = 4;
			ChangeColor = true;
			ПорогТаяния = 2;
		}

		public override void Work(Bitmap bitmap)
		{
			using (var raster = new Raster(bitmap, true, false))
				Work(raster);
		}

		public void Work(Raster raster)
		{
			var colorPoints = new List<Point>(raster.Width * raster.Height);
			InitColorPoints(raster, colorPoints);

			var emptyPointsCount = colorPoints.Capacity - colorPoints.Count;
			var progressStep = emptyPointsCount/КоличествоИтераций;
			var count = 0;

			while (count < emptyPointsCount - 1)
			{
				var index = Rand.Next(colorPoints.Count);
				var colorPoint = colorPoints[index];

				var nearPoint = new Point(colorPoint.X + Rand.Next(3) - 1, colorPoint.Y + Rand.Next(3) - 1);
				if (nearPoint.X < 0 || nearPoint.X >= raster.Width || nearPoint.Y < 0 || nearPoint.Y >= raster.Height)
					continue;

				if (raster.GetAlpha(nearPoint) > 0)
					continue;

				var color = GetAverageColor(nearPoint, raster);
				if (ChangeColor && ИзменениеЦвета > 0)
					color = GetNextColor(color, ИзменениеЦвета);
				if (Таяние > 0)
					if (Rand.Next(ПорогТаяния) == 0)
					{
						var a = color.A - Rand.Next(1 + Таяние);
						if (a < 1)
							a = 1;
						color = Color.FromArgb(a, color);
					}

				raster.SetPixel(nearPoint, color);
				colorPoints.Add(nearPoint);

				if (count > 0 && count%(progressStep + 1) == 0)
				{
					raster.CopyDataToBitmap();
					OnPreview();

					var args = new ProgressEventArgs(count/(progressStep + 1), КоличествоИтераций);
					OnProgress(args);
					if (args.Stop)
						return;

					InitColorPoints(raster, colorPoints);
				}

				count++;
			}

			raster.CopyDataToBitmap();
			OnPreview();
		}

		private static Color GetAverageColor(Point emptyPoint, Raster raster)
        {
            var a = 0;
            var r = 0;
            var g = 0;
            var b = 0;
            var count = 0;

            foreach (var point in АкварельBase.GetNearPoints(emptyPoint, raster.Width, raster.Height))
            {
                var c = raster.GetPixel(point);
                if (c.A == 0)
                    continue;

                a += c.A;
                r += c.R;
                g += c.G;
                b += c.B;
                count++;
            }

            a = (int)Math.Round((float)a / count);
            r = (int)Math.Round((float)r / count);
            g = (int)Math.Round((float)g / count);
            b = (int)Math.Round((float)b / count);

            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

	    private static void InitColorPoints(Raster raster, ICollection<Point> colorPoints)
		{
			colorPoints.Clear();
            for (var y = 0; y < raster.Height; y++)
                for (var x = 0; x < raster.Width; x++)
				    if (raster.GetAlpha(x, y) > 0 && FindEmpty(new Point(x, y), raster))
						colorPoints.Add(new Point(x, y));
		}

		private static bool FindEmpty(Point colorPoint, Raster raster)
		{
            return АкварельBase.GetNearPoints(colorPoint, raster.Width, raster.Height)
                .Select(raster.GetAlpha)
                .Any(alpha => alpha == 0);
		}

	    public override System.Windows.Controls.UserControl SettingsControl
		{
			get
			{
				return _settingsControl ?? (_settingsControl = new FirstPropertiesControl {DataContext = this});
			}
		}
	}
}
