using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;

namespace Kalantyr.PhotoFilter.Filters
{
	[Filter("Клонирование", "Повторяет картинку несколько раз")]
	public class Клонирование : FilterBase
	{
		private НастройкиКлонирования _settingsControl;

		public int Сглаживание {
			get;
			set;
		}

        public bool Градиент
        {
            get;
            set;
        }

	    public int КоличествоКопий
		{
			get; set;
		}

        public float НачальныйУгол
        {
            get;
            set;
        }

        public float КонечныйУгол
        {
            get;
            set;
        }

		public Клонирование()
		{
			КоличествоКопий = 12;
            Градиент = false;
            НачальныйУгол = 0;
            КонечныйУгол = 1;
			Сглаживание = 0;
		}

		public override void Work(Bitmap bitmap)
		{
            using (var temp = new Bitmap(bitmap.Width, bitmap.Height))
                if (Градиент)
                    using (var backup = new Bitmap(bitmap))
                        Draw(bitmap, i =>
                    {
                        using (var graphics = CreateGraphics(temp))
                        {
                            graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                            graphics.DrawImageUnscaled(backup, 0, 0);
                        }

                        BitmapData bitmapData = null;
                        try
                        {
                            bitmapData = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), ImageLockMode.ReadOnly, temp.PixelFormat);

                            var bytes = bitmapData.Stride * bitmap.Height;
                            var rgbValues = new byte[bytes];
                            Marshal.Copy(bitmapData.Scan0, rgbValues, 0, bytes);

                            for (var y = 0; y < bitmapData.Height; y++)
                                for (var x = 0; x < bitmapData.Width; x++)
                                {
                                    var color = GetPixel(rgbValues, x, y, bitmapData.Stride);
                                    var a = (int)Math.Round(i * color.A);
                                    SetPixel(rgbValues, x, y, Color.FromArgb(a, color.R, color.G, color.B), bitmapData.Stride);
                                }

                            Marshal.Copy(rgbValues, 0, bitmapData.Scan0, bytes);
                        }
                        finally
                        {
                            if (bitmapData != null)
                                temp.UnlockBits(bitmapData);
                        }

                        return temp;
                    });
                else
                {
                    using (var graphics = CreateGraphics(temp))
				    {
					    graphics.Clear(Color.FromArgb(0, 0, 0, 0));
					    graphics.DrawImageUnscaled(bitmap, 0, 0);
				    }

			        Draw(bitmap, i => temp);
			    }
		}

        private void Draw(Bitmap bitmap, Func<float, Image> temp)
	    {
	        var w = (float)bitmap.Width / 2;
	        var h = (float)bitmap.Height / 2;
	        var r = Math.Sqrt(w * w + h * h);
	        var уголЦентраРад = Math.Atan2(h, w);

            using (var graphics = CreateGraphics(bitmap))
            {
	            graphics.Clear(Color.FromArgb(0, 0, 0, 0));

	            for (var i = 0; i < КоличествоКопий; i++) {

					GaussBlur.Smooth(bitmap, Сглаживание);

	                var angle = (float)i / КоличествоКопий;
	                var a = НачальныйУгол + angle * (КонечныйУгол - НачальныйУгол);
	                var уголРад = (float)(2 * Math.PI * a);
	                var уголГрад = 360 * a;

	                OnMessage(Math.Round(уголГрад) + "°");

	                var dx = r * Math.Cos(уголРад + уголЦентраРад);
	                var dy = r * Math.Sin(уголРад + уголЦентраРад);
	                graphics.TranslateTransform(w - (float)dx, h - (float)dy);

	                graphics.RotateTransform(уголГрад);

                    graphics.DrawImageUnscaled(temp(angle), 0, 0);
	                graphics.ResetTransform();

	                OnPreview();

	                var progressEventArgs = new ProgressEventArgs(i, КоличествоКопий);
	                OnProgress(progressEventArgs);
	                if (progressEventArgs.Stop)
	                    break;
	            }
	        }
	    }

	    public override System.Windows.Controls.UserControl SettingsControl
		{
			get
			{
			    return _settingsControl ?? (_settingsControl = new НастройкиКлонирования { DataContext = this });
			}
		}
	}
}
