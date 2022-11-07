namespace ClientApp.Models
{
    public class OrderCartView
    {
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public string? Address { get; set; }
        public DateTime? RequiredDate { get; set; }
        public List<CartItemView> Items { get; set; }
    }
}
