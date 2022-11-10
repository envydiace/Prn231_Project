namespace ClientApp.Models
{
    public class RefreshTokenRequest
    {
        public int AccountId { get; set; }
        public string RefreshToken { get; set; }
    }
}
