using System;

namespace Kalantyr.PhotoFilter.Filters
{
    /// <summary>
    /// Interaction logic for НастройкиФигур.xaml
    /// </summary>
    public partial class НастройкиФигур
    {
        public НастройкиФигур()
        {
            InitializeComponent();

            _типФигур.ItemsSource = Enum.GetValues(typeof(ТипФигур));
        }
    }
}
