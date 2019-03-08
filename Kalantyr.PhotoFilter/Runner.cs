using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Kalantyr.PhotoFilter
{
	public class Runner
	{
		private bool _stopping;
        private Bitmap _result;

		public Size Size
		{
			get; private set;
		}

		public IEnumerable<FilterBase> Filters
		{
			get; private set;
		}

		public Runner(Size size, IEnumerable<FilterBase> filters)
		{
			if (filters == null) throw new ArgumentNullException("filters");

			Size = size;
			Filters = filters;
		}

		public void Work()
		{
			try
			{
				_result = new Bitmap(Size.Width, Size.Height);
				using (var graphics = Graphics.FromImage(_result))
					graphics.Clear(Color.FromArgb(0, 0, 0, 0));

				foreach (var filter in Filters.Where(filter => filter.IsEnabled))
				{
					try
					{
						filter.Progress += Filter_Progress;
						filter.Preview += Filter_Preview;
						filter.Message += Filter_Message;

						filter.Work(_result);

						if (OnFilterComplete != null)
							OnFilterComplete(_result);

						if (_stopping)
							return;
					}
					finally
					{
						filter.Preview -= Filter_Preview;
						filter.Progress -= Filter_Progress;
						filter.Message -= Filter_Message;
					}
				}

                Filter_Preview();
			}
			catch (Exception exception)
			{
				if (OnError != null)
					OnError(exception);
				else
					throw;
			}
			finally
			{
				if (OnComplete != null)
					OnComplete(_result);

				if (_result != null)
                    _result.Dispose();
			}
		}

		private void Filter_Message(object sender, MessageEventArgs e)
		{
			if (OnMessage != null)
				OnMessage(e.Message);
		}

		private void Filter_Preview()
		{
			if (OnPreview != null)
                OnPreview(_result);
		}

		private void Filter_Progress(object sender, ProgressEventArgs e)
		{
			if (FilterProgress != null)
			{
				FilterProgress(this, e);
				_stopping = e.Stop;
			}
		}

		public Action<Bitmap> OnComplete;

		public Action<Bitmap> OnFilterComplete;

		public Action<Bitmap> OnPreview;
		public Action<string> OnMessage;
		
		public event EventHandler<ProgressEventArgs> FilterProgress;

		private static Bitmap LoadSourceImage(string imageFileName)
		{
			try
			{
				using (var sourceFile = new FileStream(imageFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					return (Bitmap)Image.FromStream(sourceFile);
			}
			catch (Exception exception)
			{
				throw new InvalidOperationException("Не удалось загрузить изображение.", exception);
			}
		}

		public Action<Exception> OnError;
	}
}
