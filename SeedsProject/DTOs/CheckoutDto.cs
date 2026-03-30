namespace SeedsProject.DTOs
{
    public class CheckoutDto
    {
        public List<CartItemDto> Items { get; set; }
    }
    public class CartItemDto
    {
        public int SeedId { get; set; }
        public int Quantity { get; set; }
    }
}
