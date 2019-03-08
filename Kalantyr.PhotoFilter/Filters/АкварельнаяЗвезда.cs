using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Kalantyr.PhotoFilter.Filters
{
    [Filter("Акварельная звезда", "")]
    public class АкварельнаяЗвезда : АкварельBase
    {
        public int МаксКоличествоЛучей { get; set; }

        public int КоличествоКолец { get; set; }

        public bool ПятноВЦентре { get; set; }

        public bool Лучи { get; set; }

		public int ИзменениеЦвета { get; set; }

		public int КоличествоСлучайныхТочек { get; set; }

		public string BkImageName { get; set; }

        public АкварельнаяЗвезда()
        {
            МаксКоличествоЛучей = 12;
            КоличествоКолец = 7;
            ПятноВЦентре = false;
        	ИзменениеЦвета = 64;
        	КоличествоСлучайныхТочек = 256;
        	BkImageName = string.Empty;
        }

        private НастройкиАкварельнойЗвезды _settingsControl;

        public override void Work(Bitmap bitmap)
        {
            using (var gr = CreateGraphics(bitmap))
                gr.Clear(Color.FromArgb(0, 0, 0, 0));

            using (var layer = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb))
            using (var layerRaster = new Raster(layer, false, false))
            using (var targetRaster = new Raster(bitmap))
            {
				НарисоватьКартину(targetRaster, layerRaster);

				//if (Лучи)
				//    НарисоватьЛучи(targetRaster, layerRaster);
				//else
				//    НарисоватьСпираль(targetRaster, layerRaster);
            }

            OnPreview();
        }

        private void НарисоватьЛучи(Raster targetRaster, Raster layerRaster)
        {
        	var colors = new List<Tuple<int, int, Color>>();
            var color = Color.FromArgb(Rand.Next(255), Rand.Next(255), Rand.Next(255));

            for (var номерЛуча = 0; номерЛуча < МаксКоличествоЛучей; номерЛуча++)
				colors.Add(new Tuple<int, int, Color>(номерЛуча, 0, Color.Transparent));

			for (var номерКольца = 1; номерКольца < КоличествоКолец; номерКольца++)
            {
                var b = (float)номерКольца / КоличествоКолец;
            	color = GetNextColor(color, ИзменениеЦвета);
                for (var номерЛуча = 0; номерЛуча < МаксКоличествоЛучей; номерЛуча++)
                {
                    var alpha = МаксНепрозрачность * Math.Sin(b * Math.PI);
					colors.Add(new Tuple<int, int, Color>(номерЛуча, номерКольца, Color.FromArgb((int)alpha, color)));
                }
            }

        	var starPoints = НачальныеТочки.CreateStarPoints(targetRaster.Size, МаксКоличествоЛучей, КоличествоКолец, 0.9f,
				(ray, ring) => colors.Single(cp => cp.Item1 == ray && cp.Item2 == ring).Item3).ToList();

			var points = starPoints.Where(colorPoint => colorPoint.Color.A > 0).ToList();
			points.Add(new ColorPoint
			{
				Point = new Point(targetRaster.Width / 2, targetRaster.Height / 2),
				Color = Color.FromArgb(МаксНепрозрачность, GetNextColor(color, ИзменениеЦвета))
			});

			Draw(points, targetRaster, layerRaster);
        }

    	private void Draw(IEnumerable<ColorPoint> points, Raster targetRaster, Raster layerRaster)
    	{
    		var count = 0;
    		foreach (var colorPoint in points)
    		{
				layerRaster.Clear();

    			var параметры = new ПараметрыАкварельногоПятна
				{
					НачальнаяТочка = colorPoint.Point,
					НачальныйЦвет = colorPoint.Color
				};
    			НарисоватьАкварельноеПятно(параметры, layerRaster, targetRaster);

    			targetRaster.CopyDataToBitmap();
    			OnPreview();

    			var args = new ProgressEventArgs(count, points.Count());
    			OnProgress(args);
    			if (args.Stop)
    				return;

    			count++;
    		}
    	}

		private void НарисоватьКартину(Raster targetRaster, Raster layerRaster)
		{
			var points = new List<ColorPoint>(КоличествоСлучайныхТочек);
			using (var image = LoadImage(BkImageName))
			using (var raster = new Raster(image, true, false))
			{
				var xr = (double) targetRaster.Width/image.Width;
				var xy = (double) targetRaster.Height/image.Height;
				var r = Math.Min(xr, xy);
				var dx = (targetRaster.Width - image.Width * r) / 2;
				var dy = (targetRaster.Height - image.Height * r) / 2;

				for (var i = 0; i < points.Capacity; i++)
				{
					var x = Rand.Next(image.Width);
					var y = Rand.Next(image.Height);

					points.Add(new ColorPoint
					{
					    Color = raster.GetPixel(x, y),
					    X = (int)(dx + x * r),
						Y = (int)(dy + y * r)
					});
				}
			}
			Draw(points, targetRaster, layerRaster);
		}

    	private void НарисоватьСпираль(Raster targetRaster, Raster layerRaster)
        {
			var maxRadius = Math.Sqrt(targetRaster.Width * targetRaster.Width + targetRaster.Height * targetRaster.Height);
			var color = Color.FromArgb(МаксНепрозрачность, Rand.Next(255), Rand.Next(255), Rand.Next(255));

			var dAngle = 1d / МаксКоличествоЛучей;
			var dRadius = maxRadius / (КоличествоКолец * МаксКоличествоЛучей);
    		var starPoints = НачальныеТочки.CreateSwirlPoints(targetRaster.Size, dRadius, dAngle,
				(angle, radius) =>
				{
					var alpha = ((maxRadius - radius) / maxRadius) * МаксНепрозрачность;
					return GetNextColor(Color.FromArgb((int)alpha, color), ИзменениеЦвета);
				});

			Draw(starPoints, targetRaster, layerRaster);
		}

        public override System.Windows.Controls.UserControl SettingsControl
        {
            get
            {
                return _settingsControl ?? (_settingsControl = new НастройкиАкварельнойЗвезды { DataContext = this });
            }
        }
    }
}
