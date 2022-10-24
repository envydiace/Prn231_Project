namespace Prm231_Project.DTO
{
    public class CartItemDTO
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; } 
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
