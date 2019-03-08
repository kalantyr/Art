using System.Drawing;

namespace Kaleidoscope.Core.Elements
{
    internal class Звездочка : IДублируемыйЭлемент
    {
        #region Fields

        private int _rotateCount;
        private readonly Color _color;
    	private readonly Parameters _parameters;

    	#endregion

        public Звездочка(Color color, Parameters parameters)
        {
        	_color = color;
        	_parameters = parameters;
        }

    	#region Methods

        public void BeforeDraw()
        {
        }

        public void AfterDraw()
        {
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

        public void Init(int rayCount)
        {
            Spline = new Spline(_parameters);
            Spline.GenerateRandom(_color);
			_rotateCount = Parameters.R.Next(_parameters.MinRotateCount, _parameters.MaxRotateCount);

            Spline.Start();
        }

        public void Done()
        {
            Spline.Stop();
        }

        public void Change()
        {
            Spline.Change();
        }

        #endregion

        #region Properties

        private Spline Spline { get; set; }

        public int MaxRadius
        {
            get { return Spline.MaxRadius; }
        }

        #endregion
    }
}
