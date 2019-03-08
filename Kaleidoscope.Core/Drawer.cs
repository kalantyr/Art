using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Kaleidoscope.Core
{
    public static class Drawer
    {
        private static Color GetRandomColor()
        {
            var r = Parameters.R.Next(256);
            var g = Parameters.R.Next(256);
            var b = Parameters.R.Next(256);
            return Color.FromArgb(r, g, b);
        }

        public static Bitmap Draw(Type elementType, Parameters parameters)
        {
            Layer[] layers = null;

            try
            {
				parameters.Color = GetRandomColor();

				layers = new Layer[Parameters.R.Next(parameters.MinLayerCount, parameters.MaxLayerCount)];
                for (var i = 0; i < layers.Length; i++)
                    layers[i] = new Layer(parameters);

				var rayCount = Parameters.R.Next(parameters.MinRayCount, parameters.MaxRayCount);
                var allowGluk = Parameters.R.Next(4) == 0;
                foreach (var layer in layers)
                    layer.Init(rayCount, elementType ?? Parameters.GetElementTypes()[Parameters.R.Next(Parameters.GetElementTypes().Length)], allowGluk);

				var bitmap = new Bitmap(parameters.PictureSide, parameters.PictureSide);

                using (var gr = Graphics.FromImage(bitmap))
                {
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.Clear(Color.Empty);
                    gr.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2);
                    foreach (var layer in layers)
						layer.Draw(gr);
                }

                return bitmap;
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException("Ошибка при формировании изображения.", exc);
            }
            finally
            {
                if (layers != null)
                    foreach (var layer in layers)
                        layer.Done();
            }
        }
    }
}
