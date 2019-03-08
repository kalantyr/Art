using System.Drawing;

namespace Kalantyr.PhotoFilter {
	public class ColorPoint {
		public int X { get; set; }
		public int Y { get; set; }
		public Point Point
		{
			get { return new Point(X, Y); }
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}
		public Color Color { get; set; }
	}
}
