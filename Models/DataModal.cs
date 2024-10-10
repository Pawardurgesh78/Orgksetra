using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrgkSetra.Models
{
    public class DataModal
    {
    }
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress, Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid mobile number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Mobile No")]
        public string MobileNo { get; set; }
        [Required]
        public string Passward { get; set; }
        public Customer()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            MobileNo = "";
            Passward = "";
        }
      
    }
    [NotMapped]
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string? ItemType { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal ItemQuantity { get; set; }
        public string? ItemUnit { get; set; } = string.Empty;
        public int ItemStatus { get; set; }
        public string ItemImgIds { get; set; }
        [NotMapped]
        public IFormFile? Img { get; set; }
        [NotMapped]
        public Image? images { get; set; }
        public Item()
        {
            ItemImgIds = string.Empty;
            images = new Image();
        }
    }
    [NotMapped]
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageName { get; set; }
        public string? ContentType { get; set; }
        public int? ItemId { get; set; }
    }
}
