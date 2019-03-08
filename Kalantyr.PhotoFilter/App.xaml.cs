using System;
using System.Windows;
using System.Windows.Threading;

namespace Kalantyr.PhotoFilter
{
	public partial class App
	{
		public App()
		{
			DispatcherUnhandledException += delegate (object sender, DispatcherUnhandledExceptionEventArgs e)
			{
				ShowError(e.Exception);
				e.Handled = true;
			};
		}

		public static void ShowError(Exception error)
		{
			if (error == null) throw new ArgumentNullException("error");

			string message;
			if (error is MyException)
				message = error.Message;
			else
				message = error.ToString();

			//var message = error.Message;
			//if (error.InnerException != null)
			//{
			//    message += Environment.NewLine + Environment.NewLine + error.InnerException.Message;
			//    if (error.InnerException.InnerException != null)
			//    {
			//        message += Environment.NewLine + Environment.NewLine + error.InnerException.InnerException.Message;
			//        if (error.InnerException.InnerException.InnerException != null)
			//            message += Environment.NewLine + Environment.NewLine + error.InnerException.InnerException.InnerException.Message;
			//    }
			//}

			MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
