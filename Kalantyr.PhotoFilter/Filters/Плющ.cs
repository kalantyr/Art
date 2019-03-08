using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Kalantyr.PhotoFilter.Filters
{
	[Filter("Плющ", "Завивающиеся ползущие линии")]
	public class Плющ: FilterBase
	{
		#region Fields

		private НастройкиПлюща _settingsControl;
		private int _шаг;
		private int _количествоШагов;

		#endregion

		#region Properties

		public override System.Windows.Controls.UserControl SettingsControl
		{
			get
			{
				return _settingsControl ?? (_settingsControl = new НастройкиПлюща {DataContext = this});
			}
		}

		public double НачальнаяПозицияX
		{
			get; set;
		}

		public double НачальнаяПозицияY
		{
			get;
			set;
		}

		public double НачальнаяТолщина
		{
			get;
			set;
		}

		public double МинимальнаяТолщина
		{
			get;
			set;
		}

		public double УбываниеТолщины
		{
			get;
			set;
		}

		public double Длина
		{
			get;
			set;
		}

		public double ВероятностьНовойВетви
		{
			get;
			set;
		}

		public double ВероятностьРезкогоПоворота {
			get;
			set;
		}

		public bool Трава {
			get;
			set;
		}

		public double ОтклонениеНовойВетви
		{
			get;
			set;
		}

		public double Поворот
		{
			get;
			set;
		}

		public double УскорениеПоворота
		{
			get;
			set;
		}

		public int КоличествоГлавныхВетвей
		{
			get;
			set;
		}

		public double УскорениеУскоренияПоворота
		{
			get;
			set;
		}

		public double УскорениеУскоренияУскоренияПоворота
		{
			get;
			set;
		}

		public double ПредельныйУголСозданияВетви
		{
			get;
			set;
		}

		public double ПредельныйПоворот
		{
			get;
			set;
		}

		public bool ИнвертироватьПрозрачность
		{
			get;
			set;
		}

		public int ПредельнаяГлубина
		{
			get;
			set;
		}

		public int ИзменениеЦвета
		{
			get;
			set;
		}

		public bool РисоватьКругами {
			get;
			set;
		}

		public double ОтступКругов {
			get;
			set;
		}

		public double ТолщинаКонтура { get; set; }

		public bool УменьшатьШаг { get; set; }

		#endregion

		public Плющ()
		{
			НачальнаяПозицияX = 0.5f;
            НачальнаяПозицияY = 0.5f;
			НачальнаяТолщина = 5;
			МинимальнаяТолщина = 0.01;
			УбываниеТолщины = 0.01;
			Длина = 3;
			ВероятностьНовойВетви = 0.01;
            ВероятностьРезкогоПоворота = 0.01;
			Трава = false;
            ОтклонениеНовойВетви = 0.01;
			КоличествоГлавныхВетвей = 3;
			ИнвертироватьПрозрачность = false;
            ПредельныйУголСозданияВетви = 0.01;
			ПредельнаяГлубина = 4;
			ИзменениеЦвета = 4;

            Поворот = 0.001;
            УскорениеПоворота = 0.0001;
            УскорениеУскоренияПоворота = 0.00001;
            УскорениеУскоренияУскоренияПоворота = 0.000001;
            ПредельныйПоворот = 0.1;

			ОтступКругов = 0.5;
			ТолщинаКонтура = 1 / 4d;
		}

		#region Methos

		public override void Work(Bitmap bitmap)
		{
            using (var graphics = CreateGraphics(bitmap))
			{
				_количествоШагов = (int)(КоличествоГлавныхВетвей * (НачальнаяТолщина - МинимальнаяТолщина) / УбываниеТолщины);
				_шаг = 0;
				var цвет = Color.FromArgb(Rand.Next(128), Rand.Next(128), Rand.Next(128));

				if (Трава)
					for (var i = 0; i < КоличествоГлавныхВетвей; i++)
					{
						if (НарисоватьВетвь(НачальнаяТолщина, -Math.PI / 2, graphics, Rand.Next(bitmap.Width), bitmap.Height - 1, 0, цвет, bitmap))
							break;
					}
				else
				{
					var x = bitmap.Width * НачальнаяПозицияX;
					var y = bitmap.Height * НачальнаяПозицияY;

					for (var i = 0; i < КоличествоГлавныхВетвей; i++)
					{
						var угол = 2 * Math.PI * i / КоличествоГлавныхВетвей - Math.PI / 2;
						if (НарисоватьВетвь(НачальнаяТолщина, угол, graphics, x, y, 0, цвет, bitmap))
							break;
					}
				}
			}
		}

		private bool НарисоватьВетвь(double начальнаяТолщина, double начальныйУгол, Graphics graphics, double началоX, double началоY, int уровень, Color начальныйЦвет, Bitmap bitmap)
		{
			if (уровень > ПредельнаяГлубина)
				return false;

			//OnMessage(string.Format("Уровень: {0}", уровень));

			var x = началоX;
			var y = началоY;
			var угол = начальныйУгол;
			var толщина = начальнаяТолщина;
			var цвет = начальныйЦвет;
			
			var поворот = (Rand.NextDouble() * 2 - 1) * Math.PI * Поворот;
			var ускорениеПоворота = (Rand.NextDouble() * 2 - 1) * Math.PI * УскорениеПоворота;
			var ускорениеУскоренияПоворота = (Rand.NextDouble() * 2 - 1) * Math.PI * УскорениеУскоренияПоворота;
			var ускорениеУскоренияУскоренияПоворота = (Rand.NextDouble() * 2 - 1) * Math.PI * УскорениеУскоренияУскоренияПоворота;

			while (толщина >= МинимальнаяТолщина && толщина > 0)
			{
				var a = угол;

				var dx = Длина * Math.Cos(a);
				var dy = Длина * Math.Sin(a);

				var alpha = (толщина - МинимальнаяТолщина) / (НачальнаяТолщина - МинимальнаяТолщина);
				if (ИнвертироватьПрозрачность)
					alpha = 1 - alpha;

				if (РисоватьКругами)
				{
					var r = толщина / 2;
					var rect = new RectangleF((float)(x - r), (float)(y - r), (float)толщина, (float)толщина);
					using (var brush = new SolidBrush(цвет))
						graphics.FillEllipse(brush, rect);
					using (var pen = new Pen(Color.Black, (int)(r * ТолщинаКонтура)))
						graphics.DrawEllipse(pen, rect);

					if (УменьшатьШаг)
					{
						dx = (толщина * ОтступКругов) * Math.Cos(a);
						dy = (толщина * ОтступКругов) * Math.Sin(a);
					}
				}
				else
					using (var pen = new Pen(Color.FromArgb((int)(alpha * 255), цвет), (float)толщина))
					{
						pen.StartCap = LineCap.Round;
						pen.EndCap = LineCap.Round;
						graphics.DrawLine(pen, (float)x, (float)y, (float)(x + dx), (float)(y + dy));
					}

				x += dx;
				y += dy;

				if (Math.Abs(поворот) <= ПредельныйУголСозданияВетви * Math.PI)
					if (Rand.NextDouble() < ВероятностьНовойВетви)
					{
						var отклонение = (2 * Rand.NextDouble() - 1) * Math.PI * ОтклонениеНовойВетви;
                        if (поворот > Math.PI / 16 && Rand.Next(2) == 0)
                        {
                            if (отклонение < 0)
                                отклонение += Math.PI / 2;
                            else
                                отклонение -= Math.PI / 2;
                        }
						НарисоватьВетвь(толщина, угол + отклонение, graphics, x, y, уровень + 1, цвет, bitmap);
					}

				толщина -= УбываниеТолщины;
				цвет = GetNextColor(цвет, ИзменениеЦвета);

				угол += поворот;
				поворот += ускорениеПоворота;
				ускорениеПоворота += ускорениеУскоренияПоворота;
				ускорениеУскоренияПоворота += ускорениеУскоренияУскоренияПоворота;

				if (Rand.NextDouble() < ВероятностьРезкогоПоворота)
					угол += (Rand.NextDouble() * 2 - 1) * Math.PI;

				if (Math.Abs(поворот) > ПредельныйПоворот * Math.PI)
					return false;

				if (уровень == 0)
					_шаг++;

				if (уровень <= 2)
				{
					var progressEventArgs = new ProgressEventArgs(_шаг, _количествоШагов);
					OnProgress(progressEventArgs);
					if (progressEventArgs.Stop)
						return true;
				}
			}

			if (уровень == 0)
				OnPreview();

			return false;
		}

		#endregion
	}
}
