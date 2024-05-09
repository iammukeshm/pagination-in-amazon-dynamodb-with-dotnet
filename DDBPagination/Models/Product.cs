using Amazon.DynamoDBv2.DataModel;

namespace DDBPagination.Models;

[DynamoDBTable("products")]
public class Product
{
    [DynamoDBHashKey("category")]
    public string? Category { get; set; }
    [DynamoDBRangeKey("id")]
    public string? Id { get; set; }
    [DynamoDBProperty("name")]
    public string? Name { get; set; }
    [DynamoDBProperty("price")]
    public float? Price { get; set; }
}