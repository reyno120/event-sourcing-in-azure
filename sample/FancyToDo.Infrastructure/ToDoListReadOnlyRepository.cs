using FancyToDo.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;

namespace FancyToDo.Infrastructure;

public class ToDoListReadOnlyRepository(CosmosClient cosmosClient, IOptions<ProjectionOptions> options)
{
    private readonly Container _container = cosmosClient
        .GetContainer(options.Value.DatabaseName, options.Value.ContainerName);

    public async Task<IEnumerable<T>> Get<T>()
    {
        IOrderedQueryable<T> queryable = _container.GetItemLinqQueryable<T>();

        using FeedIterator<T> linqFeed = queryable.ToFeedIterator();

        var records = new List<T>();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<T> response = await linqFeed.ReadNextAsync();
            records.AddRange(response);
        }

        return records;
    }
}