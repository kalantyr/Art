using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Kalantyr.PhotoFilter
{
    public class Raster: IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly bool _autoCopyToBitmap;
        private readonly BitmapData _bitmapData;
        private readonly byte[] _data;

        public Raster(Bitmap bitmap, bool copyDataFromBitmap = true, bool autoCopyToBitmap = true)
        {
            _bitmap = bitmap;
            _autoCopyToBitmap = autoCopyToBitmap;
            _bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            _data = new byte[_bitmapData.Stride * bitmap.Height];

            if (copyDataFromBitmap)
                CopyDataFromBitmap();
        }

    	public Bitmap Bitmap
    	{
    		get { return _bitmap; }
    	}

    	public int Width
        {
            get { return _bitmap.Width; }
        }

		public int Height {
			get { return _bitmap.Height; }
		}

		public Size Size {
			get { return new Size(Width, Height); }
		}

        public void CopyDataFromBitmap()
        {
            Marshal.Copy(_bitmapData.Scan0, _data, 0, _data.Length);
        }

        public void CopyDataToBitmap()
        {
            Marshal.Copy(_data, 0, _bitmapData.Scan0, _data.Length);
        }

        public void Dispose()
        {
            if (_bitmapData != null)
            {
                if (_autoCopyToBitmap)
                    CopyDataToBitmap();

                _bitmap.UnlockBits(_bitmapData);
            }
        }

        public Color GetPixel(Point point)
        {
            return GetPixel(point.X, point.Y);
        }

        public Color GetPixel(int x, int y)
        {
            if (_bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                var offset = y * _bitmapData.Stride + x * 4;
                var a = _data[offset + 3];
                var r = _data[offset + 2];
                var g = _data[offset + 1];
                var b = _data[offset + 0];
                return Color.FromArgb(a, r, g, b);
            }

            if (_bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                var offset = y * _bitmapData.Stride + x * 3;
                var r = _data[offset + 2];
                var g = _data[offset + 1];
                var b = _data[offset + 0];
                return Color.FromArgb(r, g, b);
            }

            throw new NotImplementedException();
        }

        public byte GetAlpha(Point point)
        {
            return GetAlpha(point.X, point.Y);
        }

        public byte GetAlpha(int x, int y)
        {
            if (_bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                var offset = y * _bitmapData.Stride + x * 4;
                return _data[offset + 3];
            }

            if (_bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                return 255;

            throw new NotImplementedException();
        }

        public void SetPixel(Point point, Color color)
        {
            SetPixel(point.X, point.Y, color);
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (_bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                var offset = y * _bitmapData.Stride + x * 4;
                _data[offset + 3] = color.A;
                _data[offset + 2] = color.R;
                _data[offset + 1] = color.G;
                _data[offset + 0] = color.B;
                return;
            }

            if (_bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                var offset = y * _bitmapData.Stride + x * 3;
                _data[offset + 2] = color.R;
                _data[offset + 1] = color.G;
                _data[offset + 0] = color.B;
                return;
            }

            throw new NotImplementedException();
        }

        public void Clear()
        {
            Array.Clear(_data, 0, _data.Length);
        }
    }
}
