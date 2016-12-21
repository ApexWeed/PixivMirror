using System.ComponentModel.DataAnnotations;

namespace Pixiv.Models
{
    public class PixivImage
    {
        [Required]
        [RegularExpression(@"[0-9]+", ErrorMessage = "ID must be a number")]
        [Display(Name = "Pixiv ID")]
        public string ID { get; set; }

        [Required]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Index must be a number")]
        [Display(Name = "Image Index")]
        public string Index { get; set; }
    }
}
