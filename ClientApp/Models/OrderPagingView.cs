namespace ClientApp.Models
{
    public class OrderPagingView
    {
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public ICollection<OrderAdminView>? Values { get; set; }
    }
}
