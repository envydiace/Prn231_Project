namespace ClientApp.Models
{
    public class CartItemView
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
