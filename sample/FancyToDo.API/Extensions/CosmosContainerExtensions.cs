using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace FancyToDo.API.Extensions;

public static class CosmosContainerExtensions
{
    public static async Task<IEnumerable<T>> Get<T>(this Container container)
    {
        IOrderedQueryable<T> queryable = container.GetItemLinqQueryable<T>();

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