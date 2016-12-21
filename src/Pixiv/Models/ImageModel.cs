using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixiv.Models
{
    public class ImageModel
    {
        public bool Success { get { return true; } }
        public string URL { get; set; }
        public string Image { get; set; }
        public string PixivURL { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
