using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Kalantyr.PhotoFilter.Properties;
using Microsoft.Win32;
using Size=System.Drawing.Size;

namespace Kalantyr.PhotoFilter
{
	public partial class MainWindow
	{
		private const string FileExt = "filter.xml";
		private const string FileFilter = "Проекты|*." + FileExt + "|Все файлы|*.*";

		#region Fields

		private readonly ObservableCollection<FilterBase> _filters = new ObservableCollection<FilterBase>();
		private Runner _runner;
		private bool _stopping;
		private IntPtr _previewHBitmap = IntPtr.Zero;

		public static readonly DependencyProperty CanEditProperty = DependencyProperty.Register("CanEdit", typeof(bool),
		                                                                                  typeof (MainWindow), null);

		#endregion

		#region Properties

		public FilterBase SelectedFilter
		{
			get { return FiltersListBox.SelectedItem as FilterBase; }
			private set { FiltersListBox.SelectedItem = value; }
		}

		public bool CanEdit
		{
			get
			{
				return (bool)GetValue(CanEditProperty);
			}
			set
			{
				SetValue(CanEditProperty, value);
			}
		}

		#endregion

		#region Constructors

		public MainWindow()
		{
			InitializeComponent();

			FiltersListBox.ItemsSource = _filters;

			var attributes = (AssemblyTitleAttribute[]) GetType().Assembly.GetCustomAttributes(typeof (AssemblyTitleAttribute), true);
			Title = attributes[0].Title;

			WidthTextBox.Text = Settings.Default.Width.ToString();
			HeightTextBox.Text = Settings.Default.Height.ToString();
			DestFileTextBox.Text = Settings.Default.DestFileName;
			AutoOpenPreviewCheckBox.IsChecked = Settings.Default.AutoOpenPreview;

			_filters.CollectionChanged += Filters_CollectionChanged;

			DataContext = this;
			CanEdit = true;

			PriorityComboBox.ItemsSource = Enum.GetValues(typeof(ThreadPriority));
			if (Settings.Default.Priority < 0)
				PriorityComboBox.SelectedItem = ThreadPriority.BelowNormal;
			else
				PriorityComboBox.SelectedItem = (ThreadPriority)Settings.Default.Priority;

			TuneControls();

			Loaded += (sender, e) =>
			{
			    foreach (var arg in Environment.GetCommandLineArgs()
					.Where(arg => new FileInfo(arg).Extension == FileExt.Substring(FileExt.IndexOf(".")))
					.Where(File.Exists))
			    {
			    	Open(arg);
			    	break;
			    }
			};
		}

		#endregion

		#region Utilities

		private void TuneControls()
		{
			DeleteFilterButton.IsEnabled = SelectedFilter != null;
			StartStopButton.IsEnabled = _filters.Count > 0;

			var i = _filters.IndexOf(SelectedFilter);
			downButton.IsEnabled = SelectedFilter != null && i < _filters.Count - 1;
			upButton.IsEnabled = SelectedFilter != null && i > 0;
		}

		private void OnFilterComplete(Bitmap result)
		{
			if (!Dispatcher.CheckAccess())
				Dispatcher.Invoke(new Action<Bitmap>(OnFilterComplete), result);
			else
			{
				FiltersProgressBar.Value++;
				MessageTextBlock.Text = null;
			}
		}

		private void OnFilterPreview(Bitmap result)
		{
			if (!Dispatcher.CheckAccess())
				Dispatcher.Invoke(new Action<Bitmap>(OnFilterPreview), result);
			else
			{
				if (_previewHBitmap != IntPtr.Zero)
				{
					Image.Source = null;
					DeleteObject(_previewHBitmap);
				}

				_previewHBitmap = result.GetHbitmap();
				Image.Source = Imaging.CreateBitmapSourceFromHBitmap(_previewHBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
		}
		
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		private void OnFilterMessage(string message)
		{
			if (!Dispatcher.CheckAccess())
				Dispatcher.Invoke(new Action<string>(OnFilterMessage), message);
			else
				MessageTextBlock.Text = message;
		}

		private void OnComplete(Bitmap result)
		{
			if (!Dispatcher.CheckAccess())
				Dispatcher.Invoke(new Action<Bitmap>(OnComplete), result);
			else
				try
				{
					if (result != null)
                        if (!string.IsNullOrWhiteSpace(Settings.Default.DestFileName))
                            SaveImage(result, Settings.Default.DestFileName);
				}
				finally
				{
					FiltersProgressBar.Value = 0;
					MessageTextBlock.Text = null;
					FilterProgressBar.Value = 0;
					StartStopButton.Content = "Старт";
                    StartStopButton.IsEnabled = true;
					PriorityComboBox.IsEnabled = true;

					if (IsActive)
						ResetTaskbar();
					else
					{
						TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
						TaskbarItemInfo.ProgressValue = 1;
					}
					_runner = null;
					CanEdit = true;

					if (Settings.Default.AutoOpenPreview)
						Process.Start(DestFileTextBox.Text);
				}
		}

		private void ResetTaskbar()
		{
			TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
			TaskbarItemInfo.ProgressValue = 0;
		}

		private static void SaveImage(Image result, string destFileName)
		{
			try
			{
				var imageFormat = ImageFormat.Png;

				if (destFileName.ToLower().EndsWith("jpg"))
					imageFormat = ImageFormat.Jpeg;

				using (var destFile = new FileStream(destFileName, FileMode.Create, FileAccess.Write, FileShare.None))
					result.Save(destFile, imageFormat);
			}
			catch (Exception exception)
			{
				throw new InvalidOperationException("Не удалось сохранить изображение.", exception);
			}
		}

		private void OnError(Exception exception)
		{
			if (!Dispatcher.CheckAccess())
				Dispatcher.Invoke(new Action<Exception>(OnError), exception);
			else
				App.ShowError(exception);
		}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
                return;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.S)
                {
                    SaveProject();
                    e.Handled = true;
                }
                if (e.Key == Key.O)
                {
                    OpenProject();
                    e.Handled = true;
                }
            }
        }

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);

			if (TaskbarItemInfo.ProgressState == System.Windows.Shell.TaskbarItemProgressState.Paused)
				ResetTaskbar();
		}
	
		#endregion

		#region Handlers

		private void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			TuneControls();
		}

		private void DestFileButton_Click(object sender, RoutedEventArgs e)
		{
			var fileDialog = new SaveFileDialog
			{
				Filter = "PNG-файлы|*.png|JPEG-файлы|*.jpg|Все файлы|*.*"
			};
			var fileInfo = new FileInfo(DestFileTextBox.Text);
			if (fileInfo.Exists)
				fileDialog.FileName = DestFileTextBox.Text;
			if (fileInfo.Directory.Exists)
				fileDialog.InitialDirectory = fileInfo.Directory.FullName;
			if (fileDialog.ShowDialog() == true)
				DestFileTextBox.Text = fileDialog.FileName;
		}

		private void StartStopButton_Click(object sender, RoutedEventArgs e)
		{
			if (_runner != null)
			{
				_stopping = true;
				_runner = null;
                StartStopButton.IsEnabled = false;
                return;
			}

			var directory = new FileInfo(DestFileTextBox.Text).Directory;
			if (!directory.Exists)
				throw new MyException(string.Format("Папка \"{0}\" не существует.", directory.FullName));

			CanEdit = false;

			Image.Source = null;

			Settings.Default.Width = int.Parse(WidthTextBox.Text);
			Settings.Default.Height = int.Parse(HeightTextBox.Text);
			Settings.Default.DestFileName = DestFileTextBox.Text;
			Settings.Default.Priority = (int) PriorityComboBox.SelectedItem;
			Settings.Default.AutoOpenPreview = AutoOpenPreviewCheckBox.IsChecked == true;
			Settings.Default.Save();

			FiltersProgressBar.Maximum = _filters.Count;
			StartStopButton.Content = "Стоп";
			PriorityComboBox.IsEnabled = false;
			_stopping = false;
			TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;

			_runner = new Runner(new Size(Settings.Default.Width, Settings.Default.Height), _filters)
			{
			    OnComplete = OnComplete,
			    OnFilterComplete = OnFilterComplete,
			    OnError = OnError,
				OnPreview = OnFilterPreview,
				OnMessage = OnFilterMessage
			};
			_runner.FilterProgress += Runner_FilterProgress;

			var thread = new Thread(_runner.Work)
			{
			    Name = Title,
			    IsBackground = true,
			    Priority = (ThreadPriority) PriorityComboBox.SelectedItem
			};
			thread.Start();
		}

		private void Runner_FilterProgress(object sender, ProgressEventArgs e)
		{
			if (!Dispatcher.CheckAccess())
				Dispatcher.Invoke(new EventHandler<ProgressEventArgs>(Runner_FilterProgress), sender, e);
			else
			{
				FilterProgressBar.Maximum = e.FullCount;
				FilterProgressBar.Value = e.Count;
				TaskbarItemInfo.ProgressValue = (float)e.Count / e.FullCount;
				e.Stop = _stopping;
			}
		}

		private void AddFilterButton_Click(object sender, RoutedEventArgs e)
		{
			var addFilterWindow = new AddFilterWindow();
			if (addFilterWindow.ShowDialog() != true) 
				return;

			var filter = (FilterBase)Activator.CreateInstance(addFilterWindow.SelectedFilterType);
			_filters.Add(filter);
			FiltersListBox.SelectedItem = filter;
		}

		private void FiltersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			TuneControls();

			SettingsControlContainer.Content = SelectedFilter != null ? SelectedFilter.SettingsControl : null;
		}

		private void DeleteFilterButton_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Удалить фильтр?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
			if (result != MessageBoxResult.Yes)
				return;

			_filters.Remove(SelectedFilter);
		}

		private void Up_Click(object sender, RoutedEventArgs e)
		{
			var filter = SelectedFilter;
			var i = _filters.IndexOf(filter);
			_filters.Remove(filter);
			_filters.Insert(i - 1, filter);
			SelectedFilter = filter;
		}

		private void Dopwn_Click(object sender, RoutedEventArgs e)
		{
			var filter = SelectedFilter;
			var i = _filters.IndexOf(filter);
			_filters.Remove(filter);
			_filters.Insert(i + 1, filter);
			SelectedFilter = filter;
		}

	    private void SaveProject()
	    {
	        var saveFileDialog = new SaveFileDialog { DefaultExt = FileExt, Filter = FileFilter };
	        if (saveFileDialog.ShowDialog() == true)
	            using (var file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
	            {
	                var project = new Project { Filters = _filters.ToArray() };
	                var serializer = new XmlSerializer(project.GetType());
	                serializer.Serialize(file, project);
	            }
	    }

	    private void OpenProject()
	    {
	        var openFileDialog = new OpenFileDialog { DefaultExt = FileExt, Filter = FileFilter };
	        if (openFileDialog.ShowDialog() == true)
	            Open(openFileDialog.FileName);
	    }

	    private void Open(string fileName)
		{
			using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var serializer = new XmlSerializer(typeof(Project));
				var project = (Project)serializer.Deserialize(file);
				_filters.Clear();
				foreach (var filter in project.Filters)
					_filters.Add(filter);
			}
		}

		#endregion
	}

	public class FilterToNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var type = value.GetType();
			if (type != null)
			{
				var attributes = (FilterAttribute[])type.GetCustomAttributes(typeof(FilterAttribute), true);
				return attributes[0].Name;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class Project
	{
		public FilterBase[] Filters { get; set; }
	}
}
