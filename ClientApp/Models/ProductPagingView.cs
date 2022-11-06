namespace ClientApp.Models
{
    public class ProductPagingView
    {
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public ICollection<ProductView>? Values { get; set; }
    }
}
