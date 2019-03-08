using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kalantyr.PhotoFilter {
	class GaussBlur {
		protected class ConvMatrix {
			public int TopLeft, TopMid, TopRight;
			public int MidLeft, Pixel = 1, MidRight;
			public int BottomLeft, BottomMid, BottomRight;
			public int Factor = 1;
			public int Offset;
			public void SetAll(int nVal) {
				TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight =
						  BottomLeft = BottomMid = BottomRight = nVal;
			}
		}

		protected static void Conv3X3(Bitmap b, ConvMatrix m) {
			if (0 == m.Factor)
				return;

			using (var bSrc = (Bitmap)b.Clone())
			{
				BitmapData bmData = null;
				try
				{
					BitmapData bmSrc = null;
					try
					{
						bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
						bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, b.PixelFormat);
						Blur(bmSrc, bmData, m, b.PixelFormat);
					}
					finally
					{
						if (bmSrc != null)
							bSrc.UnlockBits(bmSrc);
					}
				}
				finally
				{
					if (bmData != null)
						b.UnlockBits(bmData);
				}
			}
		}

		private static unsafe void Blur(BitmapData bmSrc, BitmapData bmData, ConvMatrix m, PixelFormat pixelFormat)
		{
			int bits;
			switch (pixelFormat)
			{
				case PixelFormat.Format32bppArgb:
					bits = 4;
					break;
				case PixelFormat.Format24bppRgb:
					bits = 3;
					break;
				default:
					throw new NotImplementedException();
			}

			var stride = bmData.Stride;
			var stride2 = stride * 2;

			var scan0 = bmData.Scan0;
			var srcScan0 = bmSrc.Scan0;

			var p = (byte*)(void*)scan0;
			var pSrc = (byte*)(void*)srcScan0;
			var nOffset = stride - bmData.Width * bits;

			for (var y = 0; y < bmData.Height - 2; ++y) {
				for (var x = 0; x < bmData.Width - 2; ++x) {

					for (var bit = 0; bit < bits; bit++) {
						var color = (((
							(pSrc[bit + 0 * bits] * m.TopLeft) +
						    (pSrc[bit + 1 * bits] * m.TopMid) +
						    (pSrc[bit + 2 * bits] * m.TopRight) +
						    (pSrc[bit + 0 * bits + stride] * m.MidLeft) +
						    (pSrc[bit + 1 * bits + stride] * m.Pixel) +
						    (pSrc[bit + 2 * bits + stride] * m.MidRight) +
						    (pSrc[bit + 0 * bits + stride2] * m.BottomLeft) +
						    (pSrc[bit + 1 * bits + stride2] * m.BottomMid) +
						    (pSrc[bit + 2 * bits + stride2] * m.BottomRight))
						    / m.Factor) + m.Offset);

						if (color < 0) color = 0;
						if (color > 255) color = 255;
						p[bits + bit + stride] = (byte)color;
					}

					p += bits;
					pSrc += bits;
				}

				p += nOffset;
				pSrc += nOffset;
			}
		}

		public static void Smooth(Bitmap b, int nWeight)
		{
			if (nWeight == 0)
				return;

			var m = new ConvMatrix();
			m.SetAll(1);
			m.Pixel = nWeight;
			m.Factor = nWeight + 8;

			Conv3X3(b, m);
		}
	}
}
