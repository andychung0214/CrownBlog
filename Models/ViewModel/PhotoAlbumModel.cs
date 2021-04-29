using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownBlog.Models.ViewModel
{
    public class PhotoAlbumModel
    {
        public Guid Id { get; set; }
        public List<GalleryItem> Gallerys { get; set; }
        public List<PhotoAlbumModel> AlbumList { get; set; }
        public string GalleryUrl { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsShow { get; set; }
    }

    public class GalleryItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public string ThumbImgUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsShow { get; set; }
    }
}
