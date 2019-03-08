using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Kaleidoscope.Core
{
    internal class Spline
    {
    	private readonly Parameters _parameters;

    	#region Fields

        PointF[] _points = new PointF[0];

        //private Pen _pen;
        //private Brush _brush;
        //private GraphicsPath _path;
        private float _xCenter;
        private float _yCenter;
        private float _angle;
        private float? _dAngle;
        private Color _brushColor;
        private bool _closedCurve;

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

        public Color PenColor { get; private set; }

        public bool Polygon { get; private set; }

        public bool Fill { get; private set; }

        public float PenWidth { get; private set; }

        #endregion

        #region Methods

        public void Draw(Graphics gr)
        {
            var angle = Math.PI * _angle / 180;
            var scale = (float)(1 + Math.Sin(5 * angle) / 5);

            try
            {
                gr.TranslateTransform(_xCenter, _yCenter);
                gr.RotateTransform(_angle);
                gr.ScaleTransform(scale, scale);

                using (var path = new GraphicsPath())
                {
                    if (Polygon)
                        path.AddPolygon(_points);
                    else
                    {
                        if (_closedCurve)
                            path.AddClosedCurve(_points);
                        else
                            path.AddCurve(_points);
                    }

                    if (Fill)
                    {
                        using (var brush = new SolidBrush(_brushColor))
                            gr.FillPath(brush, path);
                    }
                    else
                        using (var pen = new Pen(PenColor, PenWidth))
                            gr.DrawPath(pen, path);
                }
            }
            finally
            {
                gr.ScaleTransform(1f / scale, 1f / scale);
                gr.RotateTransform(-_angle);
                gr.TranslateTransform(-_xCenter, -_yCenter);
            }
        }

        public void GenerateRandom(Color color)
        {
			MaxRadius = 2 * Parameters.R.Next(_parameters.MinSplineRadius, _parameters.MaxSplineRadius) / 3;
			_points = new PointF[Parameters.R.Next(_parameters.MinPointCount, _parameters.MaxPointCount)];
            for (var i = 0; i < _points.Length; i++)
            {
                float x = Parameters.R.Next(-Parameters.R.Next(MaxRadius) / 2, Parameters.R.Next(MaxRadius));
                float y = Parameters.R.Next(-Parameters.R.Next(MaxRadius) / 2, Parameters.R.Next(MaxRadius));
                _points[i] = new PointF(x, y);
            }

            PenColor = color;
            Polygon = Parameters.R.Next(2) == 0;
            Fill = Polygon && Parameters.R.Next(4) == 0;
			PenWidth = (float)(Parameters.R.NextDouble() * _parameters.PenWidth);

            var minX = _points.Min(p => p.X);
            var minY = _points.Min(p => p.Y);
            var maxX = _points.Max(p => p.X);
            var maxY = _points.Max(p => p.Y);
            _xCenter = (minX + maxX) / 2;
            _yCenter = (minY + maxY) / 2;

            _closedCurve = Parameters.R.Next(2) == 0;
        }

        #endregion

		public Spline(Parameters parameters)
		{
			_parameters = parameters;
		}

    	public void Start()
        {
            PenColor = Parameters.NextRandomColor(PenColor, _parameters);
            _brushColor = Color.FromArgb(Parameters.R.Next(PenColor.A), PenColor.R, PenColor.G, PenColor.B);

            //_path = new GraphicsPath();
            //if (Polygon)
            //    _path.AddPolygon(_points);
            //else
            //{
            //    if (Parameters.R.Next(2) == 0)
            //        _path.AddClosedCurve(_points);
            //    else
            //        _path.AddCurve(_points);
            //}

            //if (!Fill)
            //    _pen = new Pen(PenColor, PenWidth);
            //else
            //    _brush = new SolidBrush(Color.FromArgb(Parameters.R.Next(PenColor.A), PenColor.R, PenColor.G, PenColor.B));
        }

        public void Stop()
        {
            //if (_pen != null)
            //{
            //    _pen.Dispose();
            //    _pen = null;
            //}

            //_path.Dispose();

            //if (_brush != null)
            //{
            //    _brush.Dispose();
            //    _brush = null;
            //}
        }

        public void Change()
        {
            if (!_dAngle.HasValue)
            {
                var r = (1 + Parameters.R.NextDouble()) / 2;
				_dAngle = 5 * (float)(r * _parameters.MovieChangeSpeed);
                if (Parameters.R.Next(2) == 0)
                    _dAngle = -_dAngle;
            }

            _angle += _dAngle.Value;
        }
    }
}
