using System;
using System.Collections.Generic;
using System.Drawing;
using Kaleidoscope.Core.Elements;

namespace Kaleidoscope.Core
{
    public class Layer
    {
    	private readonly Parameters _parameters;

    	public enum RotationType
        {
            Simple,
            Ray,
            Gluk
        }

        private const int PolarLimit = 32;
        private const int MaxPolarRayCount = 8;
        private const int MaxRotations = 1;

        private float _maxRadius;
        private float _a0;
        private float _a1;
        private float _r0;
        private RotationType _rotationType;
        private int _polarRayCount;

        public IДублируемыйЭлемент ДублируемыйЭлемент { get; private set; }

        public int RayCount { get; private set; }

        public Layer(Parameters parameters)
        {
        	_parameters = parameters;
        	ДублируемыйЭлемент = null;
        }

    	#region Methods

        public void Draw(Graphics gr)
        {
            try
            {
                ДублируемыйЭлемент.BeforeDraw();

                var polarRayCount = _rotationType == RotationType.Ray ? _polarRayCount : 1;
                for (var j = 0; j < polarRayCount; j++)
                {
                    var a = 360f * j / polarRayCount;

                    for (var i = 0; i < RayCount; i++)
                    {
                        var t = GetPolarCoords(i);
                        var angle = t.Key;
                        var radius = t.Value;

                        var oldMatrix = gr.Transform;
                        try
                        {
                            gr.RotateTransform(a + angle);
                            gr.TranslateTransform(radius, 0);

                            var r = radius / _maxRadius;
                            r = 0.1f + 0.9f * r;
                            gr.ScaleTransform(r, r);

                            ДублируемыйЭлемент.Draw(gr);
                        }
                        finally
                        {
                            gr.Transform = oldMatrix;
                        }
                    }
                }
            }
            finally
            {
                ДублируемыйЭлемент.AfterDraw();
            }
        }

        public void Init(int rayCount, Type elementType, bool allowGluk)
        {
            RayCount = rayCount > 0
                ? GetNewRayCount(rayCount)
				: Parameters.R.Next(_parameters.MinRayCount, _parameters.MaxRayCount);

            if (Parameters.R.Next(2) == 0 && RayCount % 2 == 0)
                RayCount = RayCount / 2;
            if (Parameters.R.Next(2) == 0 && RayCount % 2 == 0)
                RayCount = RayCount / 2;
            if (Parameters.R.Next(2) == 0 && RayCount % 2 == 0)
                RayCount = RayCount / 2;
            if (Parameters.R.Next(2) == 0 && RayCount % 2 == 0)
                RayCount = RayCount / 2;
            if (Parameters.R.Next(2) == 0 && RayCount % 2 == 0)
                RayCount = RayCount / 2;

            ДублируемыйЭлемент = СоздатьДублируемыйЭлемент(elementType, _parameters);
            ДублируемыйЭлемент.Init(RayCount);

            var minRadius = ДублируемыйЭлемент.MaxRadius + 2 * rayCount;
			var maxRadius = _parameters.PictureSide / 2 - minRadius;
            _maxRadius = 3 * Parameters.R.Next(minRadius, Math.Max(minRadius, maxRadius)) / 4f;

            if (allowGluk)
                _rotationType = RotationType.Gluk;	// Ну это пипец
            else
            {
                _rotationType = RotationType.Simple;
                if (RayCount > PolarLimit)
                    if (Parameters.R.Next(4) == 0)
                        _rotationType = RotationType.Ray;
            }

            if (_rotationType == RotationType.Ray || _rotationType == RotationType.Gluk)
            {
                _a0 = (float)(360 * Parameters.R.NextDouble());
                _a1 = MaxRotations * 2 * (float)(Parameters.R.NextDouble() - 0.5d);
                _r0 = (float)(_maxRadius * Parameters.R.NextDouble()) / 2;
                _polarRayCount = 4 + 2 * Parameters.R.Next(0, MaxPolarRayCount / 2);
            }
        }

        private static IДублируемыйЭлемент СоздатьДублируемыйЭлемент(Type elementType, Parameters parameters)
        {
            if (elementType == typeof(Звездочка))
				return new Звездочка(parameters.RandomColor(), parameters);
            if (elementType == typeof(Штрихи))
				return new Штрихи(parameters.RandomColor(), parameters);
            if (elementType == typeof(Лучи))
				return new Лучи(parameters.RandomColor(), parameters);

            throw new NotImplementedException();
        }

        private static int GetNewRayCount(int rayCount)
        {
            var count = rayCount;

            if (Parameters.R.Next(2) == 0)
                if (count % 2 == 0)
                    count = count / 2;

            if (Parameters.R.Next(2) == 0)
                if (count % 2 == 0)
                    count = count / 2;

            if (Parameters.R.Next(2) == 0)
                if (count % 2 == 0)
                    count = count / 2;

            return count;
        }

        public void Done()
        {
            ДублируемыйЭлемент.Done();
        }

        public void Change()
        {
            ДублируемыйЭлемент.Change();
        }

        private KeyValuePair<float, float> GetPolarCoords(int rayNumber)
        {
            var x = (float)rayNumber / RayCount;

            if (_rotationType == RotationType.Ray)
            {
                var angle = _a0 + _a1 * x * 360;
                var radius = _r0 + x * (_maxRadius - _r0);
				return new KeyValuePair<float, float>(angle, radius);
            }

            if (_rotationType == RotationType.Gluk)
            {
                var angle = _a0 + _a1 * x * 360;
                var sin = Math.Sin(_polarRayCount * x * 2 * Math.PI);	// Пора к психотерапевту?
                var radius = _r0 + Math.Abs(sin) * (_maxRadius - _r0);
				return new KeyValuePair<float, float>(angle, (float)radius);
            }

			return new KeyValuePair<float, float>(x * 360, _maxRadius);
        }

        #endregion
    }
}
