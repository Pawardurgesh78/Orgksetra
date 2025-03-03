using Orgksetra.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrgkSetra.Models
{
    public class Items_dto
    {
        [Key]
        public int CartId { get; set; }
        public int? SessionId { get; set; }
        public string ImgSrc { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public int? OrderStatus { get; set; }
        [NotMapped]
        public ItemDetails? ItemDetails { get; set; } = new ItemDetails();
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
       
    }
}
