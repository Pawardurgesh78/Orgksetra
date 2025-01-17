using OrgkSetra.Models;

namespace OrgkSetra.Repository
{
    public interface ICartItemRepository
    {
        public List<CartItem>? GetCartItemListByCustomerId(int customerId);
    }
}
