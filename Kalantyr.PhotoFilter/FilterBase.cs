using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Xml.Serialization;
using Kalantyr.PhotoFilter.Filters;

namespace Kalantyr.PhotoFilter
{
	[XmlInclude(typeof(Клонирование))]
	[XmlInclude(typeof(Плющ))]
	[XmlInclude(typeof(Растекание))]
	[XmlInclude(typeof(Файл))]
	[XmlInclude(typeof(Фигуры))]
	[XmlInclude(typeof(НачальныеТочки))]
    [XmlInclude(typeof(Акварель))]
	[XmlInclude(typeof(АкварельнаяЗвезда))]
	public abstract class FilterBase : INotifyPropertyChanged
	{
	    protected static DateTime LastPreviewTime = DateTime.MinValue;
	    private static readonly Random R = new Random();
	    protected readonly TimeSpan PreviewTimeSpan = TimeSpan.FromSeconds(5);

	    protected static Random Rand
		{
			get
			{
				return R;
			}
		}

		public bool IsEnabled { get; set; }

		protected FilterBase()
		{
			IsEnabled = true;
		}

		public abstract void Work(Bitmap bitmap);

		public virtual UserControl SettingsControl
		{
			get { return null; }
		}

		protected virtual void OnProgress(ProgressEventArgs eventArgs)
		{
			if (Progress != null)
				Progress(this, eventArgs);
		}

		protected virtual void OnPreview()
		{
            if (Preview != null)
            {
                if (DateTime.Now - LastPreviewTime > PreviewTimeSpan)
                {
                    Preview();
                    LastPreviewTime = DateTime.Now;
                }
            }
		}

		protected virtual void OnMessage(string message)
		{
			if (Message != null)
				Message(this, new MessageEventArgs(message));
		}

		public event EventHandler<ProgressEventArgs> Progress;
		public event Action Preview;
		public event EventHandler<MessageEventArgs> Message;

		protected static int RandomStep(int value, int maxStep, int maxValue)
		{
			var v = value + Rand.Next(1 + 2 * maxStep) - maxStep;
			if (v < 0)
				v = 0;
			if (v >= maxValue)
				v = maxValue - 1;
			return v;
		}

		protected static Color GetNextColor(Color color, int maxStep)
		{
			var r = RandomStep(color.R, maxStep, 256);
			var g = RandomStep(color.G, maxStep, 256);
			var b = RandomStep(color.B, maxStep, 256);
		    return Color.FromArgb(color.A, r, g, b);
		}

		protected static Color GetPixel(IList<byte> rgbValues, int x, int y, int stride) {
			var offset = y * stride + x * 4;
			var a = rgbValues[offset + 3];
			var r = rgbValues[offset + 2];
			var g = rgbValues[offset + 1];
			var b = rgbValues[offset + 0];
			return Color.FromArgb(a, r, g, b);
		}

		protected static void SetPixel(IList<byte> rgbValues, int x, int y, Color color, int stride) {
			var offset = y * stride + x * 4;
			rgbValues[offset + 3] = color.A;
			rgbValues[offset + 2] = color.R;
			rgbValues[offset + 1] = color.G;
			rgbValues[offset + 0] = color.B;
		}

		protected static string ColorToString(Color firstColor) {
			return firstColor.A.ToString("X2") + firstColor.R.ToString("X2") + firstColor.G.ToString("X2") + firstColor.B.ToString("X2");
		}

		protected static Color ColorFromString(string s) {
			try
			{
				var a = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber);
				var r = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber);
				var g = byte.Parse(s.Substring(4, 2), NumberStyles.HexNumber);
				var b = byte.Parse(s.Substring(6, 2), NumberStyles.HexNumber);
				return Color.FromArgb(a, r, g, b);
			}
			catch (Exception)
			{
				return Color.Transparent;
			}
		}

        protected static Graphics CreateGraphics(Bitmap bitmap)
        {
            var gr = Graphics.FromImage(bitmap);
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.SmoothingMode = SmoothingMode.HighQuality;
            return gr;
        }

        protected static Bitmap LoadImage(string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                return (Bitmap)System.Drawing.Image.FromStream(file);
        }

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class FilterAttribute: Attribute
	{
		public string Name
		{
			get; private set;
		}

		public string Description
		{
			get; private set;
		}

		public FilterAttribute(string name, string description)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

			Name = name;
			Description = description;
		}
	}

	public class ProgressEventArgs : EventArgs
	{
		public int Count
		{
			get;
			private set;
		}

		public int FullCount
		{
			get;
			private set;
		}

		public bool Stop
		{
			get;
			set;
		}

		public ProgressEventArgs(int count, int fullCount)
		{
			Count = count;
			FullCount = fullCount;
		}
	}

	public class MessageEventArgs : EventArgs
	{
		public string Message
		{
			get; private set;
		}

		public MessageEventArgs(string message)
		{
			Message = message;
		}
	}
}
