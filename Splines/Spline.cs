using System.Drawing;
using System.Drawing.Drawing2D;

namespace Splines
{
	internal class Spline
	{
		#region Fields

		PointF[] _points = new PointF[0];

		private Pen _pen;
		private GraphicsPath _path;

		#endregion

		#region Properties

		public PointF[] Points
		{
			get
			{
				return _points;
			}
		}

		/// <summary>
		/// Максимальный радиус сплайна
		/// </summary>
		public int MaxRadius { get; private set; }

		public Color Color { get; private set; }

		public bool Polygon { get; private set; }

		public float PenWidth { get; private set; }

		#endregion

		#region Methods

		public void Draw(Graphics gr)
		{
			gr.DrawPath(_pen, _path);
		}

		public void GenerateRandom()
		{
			MaxRadius = Parameters.Random.Next(Parameters.MinSplineRadius, Parameters.MaxSplineRadius);
			_points = new PointF[Parameters.Random.Next(Parameters.MinPointCount, Parameters.MaxPointCount)];
			for (var i = 0; i < _points.Length; i++)
			{
				float x = Parameters.Random.Next(-Parameters.Random.Next(MaxRadius) / 2, Parameters.Random.Next(MaxRadius));
				float y = Parameters.Random.Next(-Parameters.Random.Next(MaxRadius) / 2, Parameters.Random.Next(MaxRadius));
				_points[i] = new PointF(x, y);
			}
			Color = Color.FromArgb(Parameters.Random.Next(255), Parameters.Random.Next(255), Parameters.Random.Next(255), Parameters.Random.Next(255));
			Polygon = Parameters.Random.Next(2) == 0;
			PenWidth = (float) (Parameters.Random.NextDouble() * Parameters.PenWidth);
		}

		#endregion

		public void Start()
		{
			_pen = new Pen(Color, PenWidth);

			_path = new GraphicsPath();
			if (Polygon)
				_path.AddPolygon(_points);
			else
				_path.AddClosedCurve(_points);
		}

		public void Stop()
		{
			_pen.Dispose();
			_path.Dispose();
		}
	}
}
