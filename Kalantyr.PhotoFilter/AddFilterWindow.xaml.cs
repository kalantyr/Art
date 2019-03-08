using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Kalantyr.PhotoFilter.Filters;

namespace Kalantyr.PhotoFilter
{
	public partial class AddFilterWindow
	{
		public Type SelectedFilterType
		{
			get
			{
				return _listBox.SelectedItem as Type;
			}
		}

		public AddFilterWindow()
		{
			InitializeComponent();

			_listBox.ItemsSource = GetAllFilterTypes().OrderBy(TypeToNameConverter.GetEffectName);

			TuneControls();
		}

		public IEnumerable<Type> GetAllFilterTypes()
		{
			return new[]
			{
			    typeof(Файл), 
				typeof(Плющ), 
				typeof(Клонирование), 
				typeof(Растекание),
			    typeof(Фигуры),
			    typeof(Акварель),
			    typeof(АкварельнаяЗвезда),
			    typeof(НачальныеТочки)
			};
		}

		private void _listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			TuneControls();
		}

		private void TuneControls()
		{
			OkButton.IsEnabled = SelectedFilterType != null;
		}

		private void _listBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DialogResult = true;
		}

		private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}

	public class TypeToNameConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return GetEffectName((Type)value);
		}

		internal static string GetEffectName(Type type)
		{
			if (type != null)
			{
				var attributes = (FilterAttribute[]) type.GetCustomAttributes(typeof (FilterAttribute), true);
				return attributes[0].Name;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class TypeToDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var type = value as Type;
			if (type != null)
			{
				var attributes = (FilterAttribute[])type.GetCustomAttributes(typeof(FilterAttribute), true);
				return attributes[0].Description;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
