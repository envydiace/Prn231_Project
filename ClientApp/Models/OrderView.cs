namespace ClientApp.Models
{
    public class OrderView
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? ShipAddress { get; set; }
        public virtual ICollection<CartItemView> OrderDetails { get; set; }
    }
}
