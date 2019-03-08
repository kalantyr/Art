using System.Drawing;

namespace Kaleidoscope.Core
{
    public interface IДублируемыйЭлемент
    {
        int MaxRadius { get; }

        void BeforeDraw();

        void AfterDraw();

        void Draw(Graphics gr);

        void Init(int rayCount);

        void Done();

        void Change();
    }
}
