using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DDBPagination.Models;
using DDBPagination.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapPost("/", async (IAmazonDynamoDB client, IDynamoDBContext context, [FromBody] ProductSearchRequest request) =>
{
    var partitionKey = "category";
    var query = new QueryRequest
    {
        //ScanIndexForward = false,
        TableName = "products",
        Limit = 3,
        KeyConditionExpression = $"{partitionKey} = :value",
        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            {":value", new AttributeValue { S = request.Category }}
        }
    };
    if (!string.IsNullOrEmpty(request.PaginationToken))
    {
        query.ExclusiveStartKey = JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(Convert.FromBase64String(request.PaginationToken!));
    }

    var response = await client.QueryAsync(query);
    var lastEvaluatedKey = response.LastEvaluatedKey.Count == 0 ? null : Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(response.LastEvaluatedKey, new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
    }));
    var data = response.Items
         .Select(Document.FromAttributeMap)
         .Select(context.FromDocument<Product>);
    var pagedResult = new PagedResult<Product>
    {
        Items = data.ToList(),
        PaginationToken = lastEvaluatedKey,
    };
    return Results.Ok(pagedResult);
});

app.UseHttpsRedirection();
app.Run();