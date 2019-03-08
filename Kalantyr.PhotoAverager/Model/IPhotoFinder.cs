using System.Collections.Generic;
using System.Drawing;
using Kalantyr.PhotoFilter;

namespace Kalantyr.PhotoAverager.Model
{
	public interface IPhotoFinder
	{
        IEnumerable<Bitmap> GetPhotos();

		IEnumerable<Raster> GetRasters();
	}
}


