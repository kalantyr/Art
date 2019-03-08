using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kalantyr.PhotoFilter.Filters {
	[Filter("Акварель", "Создает эффект растекания красок")]
    public class Акварель : АкварельBase
	{
        private НастройкиАкварели _settingsControl;

        public string ColorImagePath
        {
			get;
			set;
		}

        public bool ИспользоватьГраницы
        {
            get;
            set;
        }

		public string LinesImagePath {
			get;
			set;
		}

		public int КоличествоПятен {
			get;
			set;
		}

        public bool ДорисоватьЛинии
        {
            get;
            set;
        }

        public int НепрозрачностьГраниц
        {
            get;
            set;
        }

		public Акварель()
		{
            ColorImagePath = string.Empty;
            LinesImagePath = string.Empty;
			КоличествоПятен = 100;
		    ИспользоватьГраницы = true;
		    ДорисоватьЛинии = true;
		    НепрозрачностьГраниц = 32;
        }

		public override void Work(Bitmap bitmap)
		{
            using (var gr = CreateGraphics(bitmap))
                gr.Clear(Color.White);

			using (var colorImage = LoadImage(ColorImagePath))
			using (var linesImage = LoadImage(LinesImagePath))
			{
				var w = colorImage.Width;
				var h = colorImage.Height;
                if (ИспользоватьГраницы)
				    if (w != linesImage.Width || h != linesImage.Height)
					    throw new InvalidOperationException("Размеры исходников не совпадают");

				if (bitmap.Width != w || bitmap.Height != h)
					throw new InvalidOperationException(string.Format("Размер изображения должен быть {0} x {1} пикселов", w, h));

                using (var sourceColorRaster = new Raster(colorImage))
                using (var layer = new Bitmap(w, h, PixelFormat.Format32bppArgb))
                using (var layerRaster = new Raster(layer, false, false))
                using (var targetRaster = new Raster(bitmap))
                using (var контуры = new Raster(linesImage))
                    for (var i = 0; i < КоличествоПятен; i++)
				    {
					    var x = Rand.Next(w);
					    var y = Rand.Next(h);
                        if (ИспользоватьГраницы)
                            if (контуры.GetAlpha(x, y) > НепрозрачностьГраниц)
                                continue;

				        layerRaster.Clear();
				        var начальныйЦвет = ВзятЦветХолста(sourceColorRaster, x, y, 3);
				        var параметры = new ПараметрыАкварельногоПятна
				        {
				            НачальнаяТочка = new Point(x, y),
                            НачальныйЦвет = Color.FromArgb(Rand.Next(МаксНепрозрачность), начальныйЦвет),
				        };
                        НарисоватьАкварельноеПятно(параметры, layerRaster, targetRaster,
                            (xx, yy) =>
                            {
                                if (!ИспользоватьГраницы)
                                    return true;

                                return контуры.GetAlpha(xx, yy) < НепрозрачностьГраниц;

                            });

                        targetRaster.CopyDataToBitmap();
                        OnPreview();

				        var args = new ProgressEventArgs(i, КоличествоПятен);
                        OnProgress(args);
                        if (args.Stop)
                            break;
                    }

                if (ИспользоватьГраницы && ДорисоватьЛинии)
                    using (var gr = CreateGraphics(bitmap))
                        gr.DrawImageUnscaled(linesImage, 0, 0);
            }

            OnPreview();
        }

	    private static Color ВзятЦветХолста(Raster raster, int x, int y, int pixelSize = 1)
	    {
	        var count = 0;
	        var r = 0;
	        var g = 0;
	        var b = 0;

            for (var y1 = -pixelSize; y1 <= pixelSize; y1++)
            {
                var newY = y + y1;
                if (newY < 0 || newY >= raster.Height)
                    continue;
                for (var x1 = -pixelSize; x1 <= pixelSize; x1++)
                {
                    if (x1 == 0 && y1 == 0)
                        continue;

                    var newX = x + x1;
                    if (newX < 0 || newX >= raster.Width)
                        continue;

                    var color = raster.GetPixel(newX, newY);
                    r += color.R;
                    g += color.G;
                    b += color.B;
                    count++;
                }
            }

	        r = (int)((float)r / count);
	        g = (int)((float)g / count);
	        b = (int)((float)b / count);

	        return Color.FromArgb(r, g, b);
	    }

        public override System.Windows.Controls.UserControl SettingsControl
        {
            get
            {
                return _settingsControl ?? (_settingsControl = new НастройкиАкварели { DataContext = this });
            }
        }
    }
}
