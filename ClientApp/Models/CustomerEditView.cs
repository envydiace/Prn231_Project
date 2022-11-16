namespace ClientApp.Models
{
    public class CustomerEditView
    {
        public string CompanyName { get; set; } = null!;
        public string ContactName { get; set; } = null!;
        public string? ContactTitle { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; } = null!;
    }
}
