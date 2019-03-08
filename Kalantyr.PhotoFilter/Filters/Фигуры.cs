using System;
using System.Drawing;

namespace Kalantyr.PhotoFilter.Filters
{
    [Filter("Фигуры", "Случайные геометрические фигуры")]
    public class Фигуры: FilterBase
    {
        public override System.Windows.Controls.UserControl SettingsControl
        {
            get
            {
                return new НастройкиФигур { DataContext = this };
            }
        }

        public int КоличествоФигур
        {
            get; set;
        }

        public int РазмерФигур
        {
            get;
            set;
        }

        public int Непрозрачность
        {
            get;
            set;
        }

        public ТипФигур ТипФигур
        {
            get; set;
        }

        public Фигуры()
        {
            КоличествоФигур = 100;
            РазмерФигур = 100;
            ТипФигур = ТипФигур.Квадрат;
            Непрозрачность = 64;
        }

        public override void Work(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException("bitmap");

            using (var graphics = CreateGraphics(bitmap))
            {
                graphics.Clear(Color.FromArgb(0, 0, 0, 0));

                for (var i = 0; i < КоличествоФигур; i++)
                {
					var x = Rand.Next(bitmap.Width);
					var y = Rand.Next(bitmap.Height);
					var s = (bitmap.Width + bitmap.Height) / 2048;
					var sx = Rand.Next(РазмерФигур * s);
					var sy = Rand.Next(РазмерФигур * s);

                    var color = Color.FromArgb(Rand.Next(Непрозрачность), Rand.Next(256), Rand.Next(256), Rand.Next(256));
                    using (var brush = new SolidBrush(color))
                    {
                    	switch (GetТипФигур())
                    	{
                    		case ТипФигур.Квадрат:
                    			graphics.FillRectangle(brush, x - sx / 2, y - sy / 2, sx, sy);
                    			break;
                    		case ТипФигур.Круг:
                    			graphics.FillEllipse(brush, x - sx / 2, y - sy / 2, sx, sy);
                    			break;
                    	}
                    }

                	var progressEventArgs = new ProgressEventArgs(i, КоличествоФигур);
                    OnProgress(progressEventArgs);
                    if (progressEventArgs.Stop)
                        break;
                }
            }

			OnPreview();
        }

    	private ТипФигур GetТипФигур()
    	{
    		var типФигур = ТипФигур;
    		if (типФигур == ТипФигур.Случайно)
    			switch (Rand.Next(2))
    			{
    				case 0:
    					типФигур = ТипФигур.Квадрат;
    					break;
    				case 1:
    					типФигур = ТипФигур.Круг;
    					break;
    			}
    		return типФигур;
    	}
    }

    public enum ТипФигур
    {
        Квадрат,
        Круг,
		Случайно
    }
}
