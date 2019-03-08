namespace Kalantyr.PhotoFilter.Filters
{
    public partial class НастройкиНачальныхТочек
    {
        public НастройкиНачальныхТочек()
        {
            InitializeComponent();

			typeSelector.ItemsSource = new[]
			{
			    StartPointsType.CenterPoint,
				StartPointsType.TopLine,
				StartPointsType.RandomPoints,
				StartPointsType.Star,
				StartPointsType.Swirl,
			};
        }
    }
}
