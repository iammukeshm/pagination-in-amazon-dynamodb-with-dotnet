namespace DDBPagination.Requests;

public class ProductSearchRequest
{
    public string? Category { get; set; }
    public string? PaginationToken { get; set; }
}
