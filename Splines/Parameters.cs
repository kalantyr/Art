using System;
using System.Drawing;

namespace Splines
{
	internal static class Parameters
	{
		private static readonly Random Rand = new Random();

		/// <summary>
		/// ��������� ��������� �����
		/// </summary>
		public static Random Random
		{
			get
			{
				return Rand;
			}
		}

		/// <summary>
		/// ������ (� ������) ������
		/// </summary>
		public static int PictureSide = 4096;

		/// <summary>
		/// ����������� ����������� ����� �������
		/// </summary>
		public static int MinPointCount = 3;

		/// <summary>
		/// ������������ ���������� ����� �������
		/// </summary>
		public static int MaxPointCount = 7;

		/// <summary>
		/// ����������� ���������� ��������� �������
		/// </summary>
		public static int MinRotateCount = 5;

		/// <summary>
		/// ������������ ���������� ��������� �������
		/// </summary>
		public static int MaxRotateCount = 32;

		/// <summary>
		/// ����������� ���������� ����� ������
		/// </summary>
		public static int MinRayCount = 5;

		/// <summary>
		/// ������������ ���������� ����� ������
		/// </summary>
		public static int MaxRayCount = 64;

		/// <summary>
		/// ����������� ������ �������
		/// </summary>
		public static int MinSplineRadius = PictureSide / 20;

		/// <summary>
		/// ������������ ������ �������
		/// </summary>
		public static int MaxSplineRadius = PictureSide / 4;

		/// <summary>
		/// ����������� ���������� �����
		/// </summary>
		public static int MinStarCount = 2;

		/// <summary>
		/// ������������ ���������� �����
		/// </summary>
		public static int MaxStarCount = 16;

		/// <summary>
		/// ������� ����
		/// </summary>
		public static Color Color = Color.White;

		/// <summary>
		/// �������� ��������� �����-������
		/// </summary>
		public static int DAlpha = 1;

		/// <summary>
		/// �������� ��������� �����
		/// </summary>
		public static int DColor = 1;

		/// <summary>
		/// ������� ����
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
		/// ����� �����
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
