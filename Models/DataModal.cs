using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Orgksetra.ViewModel;

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
        public int ImageId { get; set; }
        [NotMapped]
        public IFormFile? Img { get; set; }
        [NotMapped]
        public virtual Image? images { get; set; }
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
    public class Cart_Session
    {
        [Key]
        public int SessionId { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public Cart_Session()
        {
            CustomerId = 0;
            Total = 0;
        }
        public ICollection<CartItem>? cartItems { get; set; }
    }
    public class DeliveryAddress
    {
        [Key]
        public int DeliveryId { get; set; }
        public int SessionId { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public string Pincode { get; set; }
        public DeliveryAddress()
        {
            Address = string.Empty;
            MobileNo = string.Empty;
            Pincode = string.Empty;
        }

    }
    public class CartItem
    {
        [Key]
        public int CartId { get; set; }
        public int? SessionId { get; set; }
        public virtual Cart_Session? Session { get; set; }
        public int ItemId { get; set; } 
        public decimal Quantity { get; set; }
        [NotMapped]
        public virtual ItemDetails? ItemDetails { get; set; } = new ItemDetails();
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
      public CartItem()
        {
            SessionId = 0;
            ItemId = 0;
            Quantity = 1;
        }
    }
    public class Orders
    {
        [Key]
        public long OrderId { get; set; }
        public int CustomerId { get; set; }
        public int SessionId { get; set; }
        public int DeliveryId { get; set; }
        public int OrderStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
    public static class OrderStatus
    {
        public static int CartIsEmpty  = 0;
        public static int OrderPending  = 1;
        public static int OrderConfirmed  = 2;
        public static int OrderCompleted  = 3;
        public static int OrderCanceled  = 4;
        public static int OrderReturned  = 5;

    }
    public static class CustomerMode
    {
        public static int Guest = -1;
       public static int Customer = 1;
    }
}

