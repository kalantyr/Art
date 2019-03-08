using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Kaleidoscope.Core.Elements
{
    internal class Штрихи : IДублируемыйЭлемент
    {
        private class PointTuple
        {
            public PointF P1 { get; set; }
            public PointF P2 { get; set; }
        }

        private IEnumerable<PointTuple> _points;
        private float _penWidth;
        private Color _color;
    	private readonly Parameters _parameters;
    	private float _angle = Parameters.R.Next(360);
        private float? _dAngle;

        public Штрихи(Color color, Parameters parameters)
        {
        	_color = color;
        	_parameters = parameters;
        }

    	public int MaxRadius { get; private set; }

        public void BeforeDraw()
        {
        }

        public void AfterDraw()
        {
        }

        public void Draw(Graphics gr)
        {
            var minX = _points.Min(p => Math.Min(p.P1.X, p.P2.X));
            var minY = _points.Min(p => Math.Min(p.P1.Y, p.P2.Y));
            var maxX = _points.Max(p => Math.Max(p.P1.X, p.P2.X));
            var maxY = _points.Max(p => Math.Max(p.P1.Y, p.P2.Y));
            var xCenter = (minX + maxX) / 2;
            var yCenter = (minY + maxY) / 2;

            var angle = Math.PI * _angle / 180;
            var scale = (float)(1 + Math.Sin(5 * angle) / 5);

            try
            {
                gr.TranslateTransform(xCenter, yCenter);
                gr.RotateTransform(_angle);
                gr.ScaleTransform(scale, scale);

                using (var pen = new Pen(_color, _penWidth))
                    foreach (var tuple in _points)
                        gr.DrawLine(pen, tuple.P1, tuple.P2);
            }
            finally
            {
                gr.ScaleTransform(1f / scale, 1f / scale);
                gr.RotateTransform(-_angle);
                gr.TranslateTransform(-xCenter, -yCenter);
            }
        }

        public void Init(int rayCount)
        {
            _penWidth = (float)(Parameters.R.NextDouble() * _parameters.PenWidth) / 4;

			MaxRadius = Parameters.R.Next(_parameters.MinSplineRadius, _parameters.MaxSplineRadius) / 4;

			var count = 8 * Parameters.R.Next(_parameters.MinPointCount, 4 * _parameters.MaxPointCount);
            var points = new PointTuple[count];

            var x1 = Parameters.R.Next(-MaxRadius, MaxRadius);
            var y1 = Parameters.R.Next(-MaxRadius, MaxRadius);
            var x2 = Parameters.R.Next(-MaxRadius, MaxRadius);
            var y2 = Parameters.R.Next(-MaxRadius, MaxRadius);

            var maxDelta = Parameters.R.Next(MaxRadius / 4);

            for (var i = 0; i < points.Length; i++)
            {
                x1 += Parameters.R.Next(-maxDelta, maxDelta);
                y1 += Parameters.R.Next(-maxDelta, maxDelta);
                x2 += Parameters.R.Next(-maxDelta, maxDelta);
                y2 += Parameters.R.Next(-maxDelta, maxDelta);
                points[i] = new PointTuple { P1 = new Point(x1, y1), P2 = new Point(x2, y2) };
            }
            _points = points;

            _color = Parameters.NextRandomColor(_color, _parameters);
        }

        public void Done()
        {
        }

        public void Change()
        {
            //_color = Parameters.NextRandomColor(_color);

            if (!_dAngle.HasValue)
            {
                _dAngle = (1 + (float)Parameters.R.NextDouble()) / 2;
                if (Parameters.R.Next(2) == 0)
                    _dAngle = -_dAngle;
            }

            _angle += _dAngle.Value;
        }
    }
}
