namespace ClientApp.Models
{
    public class OrderSearchView
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? page { get; set; } = 1;
    }
}
