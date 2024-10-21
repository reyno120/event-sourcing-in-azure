using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace FancyToDo.Infrastructure;

public class ToDoListReadRepository(CosmosClient cosmosClient, string dbName, string containerName)
{
    private readonly Container _container = cosmosClient.GetContainer(dbName, containerName);

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