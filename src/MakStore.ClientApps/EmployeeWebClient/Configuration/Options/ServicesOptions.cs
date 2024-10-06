namespace EmployeeWebClient.Configuration.Options;

public class ServicesOptions
{
    public ProductsServiceOptions ProductsService { get; set; }

    #region Classes

    public class ProductsServiceOptions
    {
        public string BaseUrl { get; set; }
    }

    #endregion
}