using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace Kalantyr.PhotoFilter.Filters {
	[Filter("Начальные точки", "Создает опорные точки для эффекта растекания")]
	public class НачальныеТочки : FilterBase {

        private НастройкиНачальныхТочек _settingsControl;

		public float РадиусЗвезды { get; set; }

		public int КоличествоКолец { get; set; }

		private StartPointsType _type;
		public StartPointsType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				OnPropertyChanged("Type");
			}
		}

		public PointF PointPosition
        {
            get { return new PointF(0.5f, 0.5f); }
        }

		public int PointsCount { get; set; }

		[XmlIgnore]
		public Color ColorLeft { get; set; }

		[XmlIgnore]
		public Color ColorCenter { get; set; }

		[XmlIgnore]
		public Color ColorRight { get; set; }

		public string ColorLeftString {
			get { return ColorToString(ColorLeft); }
			set { ColorLeft = ColorFromString(value); }
		}

		public string ColorCenterString {
			get { return ColorToString(ColorCenter); }
			set { ColorCenter = ColorFromString(value); }
		}

		public string ColorRightString {
			get { return ColorToString(ColorRight); }
			set { ColorRight = ColorFromString(value); }
		}

		public int КоличествоЛучей { get; set; }

		public НачальныеТочки()
		{
			Type = StartPointsType.TopLine;
			ColorLeft = Color.Green;
			ColorCenter = Color.Yellow;
			ColorRight = Color.Red;
			PointsCount = 256;
			КоличествоЛучей = 12;
			КоличествоКолец = 3;
			РадиусЗвезды = 0.5f;
		}

		public override void Work(Bitmap bitmap)
		{
			var w2 = bitmap.Width / 2;

			switch (Type)
			{
				case StartPointsType.CenterPoint:
					bitmap.SetPixel((int)(bitmap.Width * PointPosition.X), (int)(bitmap.Height * PointPosition.Y), ColorCenter);
					break;

				case StartPointsType.TopLine:
					for (var x = 0; x < w2; x++)
						bitmap.SetPixel(x, 0, MixColors(ColorLeft, ColorCenter, (float)x / w2));
					for (var x = w2; x < bitmap.Width; x++)
						bitmap.SetPixel(x, 0, MixColors(ColorCenter, ColorRight, (float)(x - w2) / w2));
					break;

				case StartPointsType.RandomPoints:
					for (var i = 0; i < PointsCount; i++)
					{
						var color = Rand.Next(2) == 0
							? GetRandomColorFromRange(ColorLeft, ColorCenter)
							: GetRandomColorFromRange(ColorCenter, ColorRight);
						bitmap.SetPixel(Rand.Next(bitmap.Width), Rand.Next(bitmap.Height), color);
					}
					break;

				case StartPointsType.Star:
					var colors = new Dictionary<int, Color>();
					for (var ring = 0; ring < КоличествоКолец; ring++)
						colors.Add(ring, Rand.Next(2) == 0 ? GetRandomColorFromRange(ColorLeft, ColorCenter) : GetRandomColorFromRange(ColorCenter, ColorRight));

					foreach (var colorPoint in CreateStarPoints(bitmap.Size, КоличествоЛучей, КоличествоКолец, РадиусЗвезды, (ray, ring) => colors[ring]))
						bitmap.SetPixel(colorPoint.X, colorPoint.Y, colorPoint.Color);

					break;

				case StartPointsType.Swirl:
					const double dAngle = 0.02d;
					const double dRadius = 2.0d;
					foreach (var colorPoint in CreateSwirlPoints(bitmap.Size, dRadius, dAngle,
						(angle, radius) => Rand.Next(2) == 0
							? GetRandomColorFromRange(ColorLeft, ColorCenter)
							: GetRandomColorFromRange(ColorCenter, ColorRight)))
						bitmap.SetPixel(colorPoint.X, colorPoint.Y, colorPoint.Color);
					break;

				default:
					throw new NotImplementedException();
			}
			OnPreview();
		}

		internal static IEnumerable<ColorPoint> CreateSwirlPoints(Size bitmapSize, double dRadius, double dAngle, Func<float, float, Color> getColor)
		{
			var w2 = bitmapSize.Width / 2;
			var h2 = bitmapSize.Height / 2;

			var result = new List<ColorPoint>();

			var maxRadius = Math.Sqrt(w2 * w2 + h2 * h2);
			var radius = 0d;
			var angle = 0d;
			while (radius < maxRadius)
			{
				var x = (int)(w2 + radius * Math.Cos(angle));
				var y = (int)(h2 + radius * Math.Sin(angle));
				if (x >= 0 && x < bitmapSize.Width)
					if (y >= 0 && y < bitmapSize.Height)
						result.Add(new ColorPoint { X = x, Y = y, Color = getColor((float)angle, (float)radius) });

				radius += dRadius;
				angle += 2 * Math.PI * dAngle;
			}
			return result;
		}

		internal static IEnumerable<ColorPoint> CreateStarPoints(Size bitmapSize, int rays, int rings, float radiusRatio, Func<int, int, Color> getColor)
		{
			var w2 = bitmapSize.Width / 2;
			var h2 = bitmapSize.Height / 2;

			var maxRadius = Math.Min(w2, h2);

			var result = new List<ColorPoint>();
		
			for (var ring = 0; ring < rings; ring++)
			{
				var radius = (float)ring / rings;
				radius *= maxRadius * radiusRatio;
				for (var ray = 0; ray < rays; ray++)
				{
					var angle = ((float) ray/rays);
					var dx = radius * Math.Cos(2 * Math.PI * angle);
					var dy = radius * Math.Sin(2 * Math.PI * angle);
					var x = w2 + (int)dx;
					var y = h2 + (int)dy;
					if (x >= 0 && x < bitmapSize.Width)
						if (y >= 0 && y < bitmapSize.Height)
							result.Add(new ColorPoint { X = x, Y = y, Color = getColor(ray, ring) });
				}
			}

			return result;
		}

		private static Color GetRandomColorFromRange(Color color1, Color color2)
		{
			var a = GetRandomFromRange(color1.A, color2.A);
			var r = GetRandomFromRange(color1.R, color2.R);
			var g = GetRandomFromRange(color1.G, color2.G);
			var b = GetRandomFromRange(color1.B, color2.B);
			return Color.FromArgb(a, r, g, b);
		}

		private static byte GetRandomFromRange(int v1, int v2)
		{
			return v1 < v2
				? (byte) Rand.Next(v1, v2 + 1)
				: (byte) Rand.Next(v2, v1 + 1);
		}

		private static Color MixColors(Color color1, Color color2, float i)
		{
			var a = color1.A + i * (color2.A - color1.A);
			var r = color1.R + i * (color2.R - color1.R);
			var g = color1.G + i * (color2.G - color1.G);
			var b = color1.B + i * (color2.B - color1.B);
			return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
		}

		public override System.Windows.Controls.UserControl SettingsControl
        {
            get
            {
                return _settingsControl ?? (_settingsControl = new НастройкиНачальныхТочек { DataContext = this });
            }
        }
    }

	public enum StartPointsType
	{
		CenterPoint,
		TopLine,
		RandomPoints,
		Star,
		Swirl,
	}
}
