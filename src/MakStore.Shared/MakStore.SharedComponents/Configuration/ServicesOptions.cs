namespace MakStore.SharedComponents.Configuration;

public class ServicesOptions
{
    public ProductsServiceOptions ProductsService { get; set; }
    public OrdersServiceOptions OrdersService { get; set; }

    #region Classes

    public class ProductsServiceOptions
    {
        public string BaseUrl { get; set; }
    }
    
    public class OrdersServiceOptions
    {
        public string BaseUrl { get; set; }
    }

    #endregion
}