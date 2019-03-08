using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Kalantyr.PhotoFilter.Filters
{
    public abstract class АкварельBase : FilterBase
    {
        public int ПорогТаяния { get; set; }

        public int СкоростьТаяния { get; set; }

		public int МаксНепрозрачность {
			get;
			set;
		}

		public float ВероятностьУвеличенияПрозрачности {
			get;
			set;
		}

        protected АкварельBase()
        {
            СкоростьТаяния = 1;
            ПорогТаяния = 4;
            МаксНепрозрачность = 255;
        	ВероятностьУвеличенияПрозрачности = 1f / 16;
        }

        protected void НарисоватьАкварельноеПятно(ПараметрыАкварельногоПятна параметры, Raster layerRaster, Raster targetRaster, Func<int, int, bool> advancedCondition = null)
        {
            var s = layerRaster.Width * layerRaster.Height;

            var nextPoints = new List<Point>(s);
            var addedPoints = new List<Point>(nextPoints.Capacity);

            AddPoint(параметры.НачальнаяТочка, параметры.НачальныйЦвет, layerRaster, addedPoints, nextPoints);

            var count = 0;
            while (true)
            {
                if (count > 0 && count % (s / 16) == 0)
                    InitNextPoints(layerRaster, nextPoints, СкоростьТаяния);
                count++;

                if (addedPoints.Count >= s || nextPoints.Count == 0)
                    break;

                var basePoint = nextPoints[Rand.Next(nextPoints.Count)];
                var newPoint = new Point(basePoint.X + Rand.Next(3) - 1, basePoint.Y + Rand.Next(3) - 1);

                if (newPoint.X < 0 || newPoint.Y < 0 || newPoint.X >= layerRaster.Width || newPoint.Y >= layerRaster.Height)
                    continue;
                if (layerRaster.GetAlpha(newPoint.X, newPoint.Y) > 0)
                    continue;
                if (advancedCondition != null && !advancedCondition(newPoint.X, newPoint.Y))
                    continue;

                var color = layerRaster.GetPixel(basePoint);
                if (color.A < СкоростьТаяния)
                    continue;
                var alpha = GetNewAlpha(color.A);
                if (alpha <= 0)
                    continue;

                AddPoint(newPoint, Color.FromArgb(alpha, color), layerRaster, addedPoints, nextPoints);
            }

            СкопироватьПятноНаОсновнойХолст(layerRaster, targetRaster, addedPoints);
        }

        private void AddPoint(Point newPoint, Color color, Raster layerRaster, ICollection<Point> addedPoints, IList<Point> nextPoints)
        {
            layerRaster.SetPixel(newPoint, color);
            addedPoints.Add(newPoint);
                    
			if (НужнаОбработка(newPoint, layerRaster, СкоростьТаяния))
                nextPoints.Add(newPoint);
        }

        private int GetNewAlpha(byte a)
        {
            if (a < СкоростьТаяния)
                return 1;

            var alpha = (int)a;

//            if (Rand.Next(ПорогТаяния + 1) == 0)
            {
                alpha -= Rand.Next(СкоростьТаяния + 1);
				if (Rand.NextDouble() < ВероятностьУвеличенияПрозрачности)
					alpha += Rand.Next(СкоростьТаяния + 1);
				if (Rand.NextDouble() < ВероятностьУвеличенияПрозрачности)
					alpha -= Rand.Next(2 * СкоростьТаяния + 1);
				if (alpha < 1)
                    alpha = 1;
                if (alpha > 255)
                    alpha = 255;
            }

            return alpha;
        }

        private static void СкопироватьПятноНаОсновнойХолст(Raster layerRaster, Raster targetRaster, IEnumerable<Point> points)
        {
            foreach (var p in points)
            {
                var newColor = layerRaster.GetPixel(p);
                if (newColor.A <= 0)
                    continue;

                var alpha = newColor.A / 255f;

                var oldColor = targetRaster.GetPixel(p);

                var a = oldColor.A + (255 - oldColor.A) * alpha;
                var r = (1 - alpha) * oldColor.R + alpha * newColor.R;
                var g = (1 - alpha) * oldColor.G + alpha * newColor.G;
                var b = (1 - alpha) * oldColor.B + alpha * newColor.B;
                targetRaster.SetPixel(p, Color.FromArgb((int)a, (int)r, (int)g, (int)b));
            }
        }

        private static bool НужнаОбработка(Point point, Raster raster, int скоростьТаяния)
        {
            if (raster.GetAlpha(point) < скоростьТаяния)
                return false;

            var nearPoints = GetNearPoints(point, raster.Width, raster.Height);
            return nearPoints.Any(p => raster.GetAlpha(p) == 0);
        }

        public static IEnumerable<Point> GetNearPoints(Point point, int width, int height, bool excludeCenter = true)
        {
            var list = new List<Point>();
            for (var y1 = -1; y1 <= 1; y1++)
            {
                var newY = point.Y + y1;
                if (newY < 0 || newY >= height)
                    continue;
                for (var x1 = -1; x1 <= 1; x1++)
                {
                    if (excludeCenter)
                        if (x1 == 0 && y1 == 0)
                            continue;

                    var newX = point.X + x1;
                    if (newX < 0 || newX >= width)
                        continue;

                    list.Add(new Point(newX, newY));
                }
            }
            return list;
        }

        private static void InitNextPoints(Raster layerRaster, ICollection<Point> nextPoints, int скоростьТаяния)
        {
            nextPoints.Clear();
            for (var y = 0; y < layerRaster.Height; y++)
                for (var x = 0; x < layerRaster.Width; x++)
                {
                    var point = new Point(x, y);
                    if (НужнаОбработка(point, layerRaster, скоростьТаяния))
                        nextPoints.Add(point);
                }

        }
    }

    public class ПараметрыАкварельногоПятна
    {
        public Point НачальнаяТочка { get; set; }

        public Color НачальныйЦвет { get; set; }
    }
}
