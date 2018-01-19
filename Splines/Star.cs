using System.Drawing;

namespace Splines
{
	internal class Star
	{
		#region Fields

		public Star()
		{
			Splines = null;
		}

		#endregion

		#region Methods

		public void Draw(Graphics gr)
		{
			var rad = Parameters.PictureSide / 2;
			float radius = Parameters.Random.Next(Splines.Spline.MaxRadius, rad - Splines.Spline.MaxRadius);

			try
			{
				Splines.Start();

				float phase = Parameters.Random.Next(360);
				var oldMatrix = gr.Transform;
				for (var i = 0; i < RayCount; i++)
				{
					var angle = 360f * (i / (float)RayCount);
					gr.RotateTransform(phase + angle);
					gr.TranslateTransform(radius, 0);
					Splines.Draw(gr);
					gr.Transform = oldMatrix;
				}
			}
			finally
			{
				Splines.Stop();
			}
		}

		public void GenerateRandom()
		{
			Splines = new Splines();
			Splines.GenerateRandom();
			RayCount = Parameters.Random.Next(Parameters.MinRayCount, Parameters.MaxRayCount);
		}

		#endregion

		#region Properties

		public Splines Splines { get; private set; }

		public int RayCount { get; set; }

		#endregion
	}
}
