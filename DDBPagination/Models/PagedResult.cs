namespace DDBPagination.Models;

public class PagedResult<T> where T : class
{
    public List<T> Items { get; set; } = new List<T>();
    public string? PaginationToken { get; set; }
}
