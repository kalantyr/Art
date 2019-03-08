using System;
using System.Drawing;
using Kaleidoscope.Core.Elements;

namespace Kaleidoscope.Core
{
    public class Parameters
    {
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Генератор случайных чисел
        /// </summary>
        public static Random R
        {
            get
            {
                return Rand;
            }
        }

        /// <summary>
        /// Минимальное колическтво узлов сплайна
        /// </summary>
        public int MinPointCount = 3;

        /// <summary>
        /// Максимальное количество узлов сплайна
        /// </summary>
        public int MaxPointCount = 7;

        /// <summary>
        /// Минимальное количество поворотов сплайна
        /// </summary>
        public int MinRotateCount = 5;

        /// <summary>
        /// Максимальное количество поворотов сплайна
        /// </summary>
        public int MaxRotateCount = 16;

        /// <summary>
        /// Минимальное количество лучей звезды
        /// </summary>
        public int MinRayCount = 5;

        /// <summary>
        /// Максимальное количество лучей звезды
        /// </summary>
        public int MaxRayCount = 64;

        /// <summary>
        /// Минимальный радиус сплайна
        /// </summary>
        public int MinSplineRadius;

        /// <summary>
        /// Максимальный радиус сплайна
        /// </summary>
        public int MaxSplineRadius;

        /// <summary>
        /// Минимальное количество звезд
        /// </summary>
        public int MinLayerCount = 16;

        /// <summary>
        /// Максимальное количество звезд
        /// </summary>
        public int MaxLayerCount = 64;

        /// <summary>
        /// Текущий цвет
        /// </summary>
        public Color Color;

        /// <summary>
        /// Скорость изменения альфа-канала
        /// </summary>
        public int DAlpha = 16;

        /// <summary>
        /// Скорость изменения цвета
        /// </summary>
        public int DColor = 16;

        /// <summary>
        /// Толщина пера
        /// </summary>
        public float PenWidth
        {
            get { return PenWidthRatio * PictureSide / 4096; }
        }

        /// <summary>
        /// Коэффициент толщины пера
        /// </summary>
        public float PenWidthRatio
        {
            get { return 1.1f; }
        }

        /// <summary>
        /// Диапазон случайных цветов (и альфа-канала)
        /// </summary>
        public int ColorRange
        {
            get { return 192; }
        }

        /// <summary>
        /// Относительная скорость изменений при анимации
        /// </summary>
        public double MovieChangeSpeed = 1d / 10d;

    	/// <summary>
    	/// Ширина (и высота) растра
    	/// </summary>
    	public int PictureSide;

		public Parameters()
		{
			PictureSide = 1024;
			MinSplineRadius = PictureSide / 20;
			MaxSplineRadius = PictureSide / 4;
			Color = RandomColor();
		}

    	#region Methods

        private static int NextColor(int v, int dv, Parameters parameters)
        {
			var min = (256 - parameters.ColorRange) / 2;
            var max = 256 - min;

            var result = v + R.Next(1 + 2 * dv) - dv;
            if (result < min)
                result = min;
            if (result > max)
                result = max;
            return result;
        }

        /// <summary>
        /// Сдвиг цвета
        /// </summary>
        public static Color NextRandomColor(Color color, Parameters parameters)
        {
			var a = NextColor(color.A, parameters.DAlpha, parameters);
			var r = NextColor(color.R, parameters.DColor, parameters);
			var g = NextColor(color.G, parameters.DColor, parameters);
			var b = NextColor(color.B, parameters.DColor, parameters);
            return Color.FromArgb(a, r, g, b);
        }

        public Color RandomColor()
        {
            var min = (256 - ColorRange) / 2;
            var max = 256 - min;
            return Color.FromArgb(R.Next(min, max), R.Next(min, max), R.Next(min, max), R.Next(min, max));
        }

        public static Type[] GetElementTypes()
        {
            return new[] { typeof(Звездочка), typeof(Штрихи), typeof(Лучи) };
        }

        #endregion
    }
}
