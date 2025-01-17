using System.ComponentModel.DataAnnotations;
namespace Orgksetra.ViewModel
{
    public class ViewModel
    {
    }
    public class ItemDetails
    {
        [Key    ]
        public int? ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string? ItemType { get; set; }
        [RegularExpression(@"^[0-9.]+$", ErrorMessage = "Price must be in number")]
        public decimal ItemPrice { get; set; }
        [RegularExpression(@"^[0-9.]+$", ErrorMessage = "Quantity must be in number")]
        public decimal ItemQuantity { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Unit must be in alphabate")]
        public string? ItemUnit { get; set; } = string.Empty;
        public int ItemStatus { get; set; }
        public int? ImageId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageName { get; set; }
        public string? ContentType { get; set; }

    }
    
}
