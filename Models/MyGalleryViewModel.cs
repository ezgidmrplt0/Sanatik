using System.Collections.Generic;

namespace Sanatik.Models
{
    public class MyGalleryViewModel
    {
        public List<Photo> UploadedPhotos { get; set; } = new List<Photo>();
        public List<Photo> SavedPhotos { get; set; } = new List<Photo>();
    }
}
