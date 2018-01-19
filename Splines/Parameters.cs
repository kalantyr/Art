using System;
using System.Drawing;

namespace Splines
{
	internal static class Parameters
	{
		private static readonly Random Rand = new Random();

		/// <summary>
		/// Генератор случайных чисел
		/// </summary>
		public static Random Random
		{
			get
			{
				return Rand;
			}
		}

		/// <summary>
		/// Ширина (и высота) растра
		/// </summary>
		public static int PictureSide = 4096;

		/// <summary>
		/// Минимальное колическтво узлов сплайна
		/// </summary>
		public static int MinPointCount = 3;

		/// <summary>
		/// Максимальное количество узлов сплайна
		/// </summary>
		public static int MaxPointCount = 7;

		/// <summary>
		/// Минимальное количество поворотов сплайна
		/// </summary>
		public static int MinRotateCount = 5;

		/// <summary>
		/// Максимальное количество поворотов сплайна
		/// </summary>
		public static int MaxRotateCount = 32;

		/// <summary>
		/// Минимальное количество лучей звезды
		/// </summary>
		public static int MinRayCount = 5;

		/// <summary>
		/// Максимальное количество лучей звезды
		/// </summary>
		public static int MaxRayCount = 64;

		/// <summary>
		/// Минимальный радиус сплайна
		/// </summary>
		public static int MinSplineRadius = PictureSide / 20;

		/// <summary>
		/// Максимальный радиус сплайна
		/// </summary>
		public static int MaxSplineRadius = PictureSide / 4;

		/// <summary>
		/// Минимальное количество звезд
		/// </summary>
		public static int MinStarCount = 2;

		/// <summary>
		/// Максимальное количество звезд
		/// </summary>
		public static int MaxStarCount = 16;

		/// <summary>
		/// Текущий цвет
		/// </summary>
		public static Color Color = Color.White;

		/// <summary>
		/// Скорость изменения альфа-канала
		/// </summary>
		public static int DAlpha = 1;

		/// <summary>
		/// Скорость изменения цвета
		/// </summary>
		public static int DColor = 1;

		/// <summary>
		/// Толщина пера
		/// </summary>
		public static int PenWidth = PictureSide / 1024;

		#region Methods

		private static int NextColor(int v, int dv)
		{
			var result = v + Random.Next(1 + 2 * dv) - dv;
			if (result < 0)
				result = 0;
			if (result > 255)
				result = 255;
			return result;
		}

		/// <summary>
		/// Сдвиг цвета
		/// </summary>
		public static void NextColor()
		{
			var a = NextColor(Color.A, DAlpha);
			var r = NextColor(Color.R, DColor);
			var g = NextColor(Color.G, DColor);
			var b = NextColor(Color.B, DColor);
			Color = Color.FromArgb(a, r, g, b);
		}

		#endregion
	}
}
