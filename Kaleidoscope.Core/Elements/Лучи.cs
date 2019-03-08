using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Kaleidoscope.Core.Elements
{
    internal class Лучи : IДублируемыйЭлемент
    {
        private class Ray
        {
            public SizeF Vector { get; set; }

            public Color Color { get; set; }
        }

        private IEnumerable<Ray> _rays;
        private float _penWidth;
        private Color _color;
    	private readonly Parameters _parameters;
    	private PointF _center;

        public Лучи(Color color, Parameters parameters)
        {
        	_color = color;
        	_parameters = parameters;
        }

    	public int MaxRadius
        {
            get
            {
                return (int)_rays.Max(r => Math.Sqrt(r.Vector.Width * r.Vector.Width + r.Vector.Height * r.Vector.Height));
            }
        }

        public void BeforeDraw()
        {
            foreach (var ray in _rays)
                ray.Color = Parameters.NextRandomColor(ray.Color, _parameters);
        }

        public void AfterDraw()
        {
        }

        public void Draw(Graphics gr)
        {
            foreach (var ray in _rays)
                using (var pen = new Pen(ray.Color, _penWidth))
                    gr.DrawLine(pen, _center, _center + ray.Vector);
        }

        public void Init(int rayCount)
        {
            _penWidth = (float)(Parameters.R.NextDouble() * _parameters.PenWidth) / 4;

			var radius = Parameters.R.Next(2 * _parameters.MinSplineRadius, _parameters.MaxSplineRadius) / 4;

            var angle = Parameters.R.NextDouble() * 2 * Math.PI;
            _center = new PointF((float)(radius * Math.Cos(angle)), (float)(radius * Math.Sin(angle)));

			var count = 16 * Parameters.R.Next(_parameters.MinPointCount, _parameters.MaxPointCount);
            var rays = new Ray[count];

            for (var i = 0; i < rays.Length; i++)
            {
                angle = Parameters.R.NextDouble() * 2 * Math.PI;
                var r = Parameters.R.Next(radius, 2 * radius);
                var x = r * Math.Cos(angle);
                var y = r * Math.Sin(angle);

                rays[i] = new Ray
                {
                    Vector = new SizeF((float)x, (float)y),
                    Color = _color
                };

                _color = Parameters.NextRandomColor(_color, _parameters);
            }
            _rays = rays;
        }

        public void Done()
        {
        }

        public void Change()
        {
            //_color = Parameters.NextRandomColor(_color);
        }
    }
}
