using System.Drawing;

namespace Splines
{
	internal class Splines
	{
		#region Fields

		private int _rotateCount;

		#endregion

		#region Methods

		public void Start()
		{
			Spline.Start();
		}

		public void Stop()
		{
			Spline.Stop();
		}

		public void Draw(Graphics gr)
		{
			var oldMatrix = gr.Transform;
			for (var i = 0; i < _rotateCount; i++)
			{
				var angle = 360f * (i / (float)_rotateCount);
				gr.RotateTransform(angle);
				Spline.Draw(gr);
				gr.Transform = oldMatrix;
			}
		}

		public void GenerateRandom()
		{
			Spline = new Spline();
			Spline.GenerateRandom();
			_rotateCount = Parameters.Random.Next(Parameters.MinRotateCount, Parameters.MaxRotateCount);
		}

		#endregion

		#region Properties

		public Spline Spline { get; private set; }

		#endregion
	}
}
