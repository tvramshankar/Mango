using Mango.Web.Utility;

namespace Mango.Web.Models.DTO
{
	public class ProductPostDTO
	{
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ImageLocalPath { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        [MaxFileSize(10)]
        [AllowedExtension(new string[] {".jpg",".png"})]
        public IFormFile? Image { get; set; }
    }
}