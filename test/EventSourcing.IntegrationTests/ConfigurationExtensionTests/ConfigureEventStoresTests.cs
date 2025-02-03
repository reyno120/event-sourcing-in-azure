using EventSourcing.Core;
using FancyToDo.Core.ToDoList;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EventSourcing.IntegrationTests.ConfigurationExtensionTests;

[Collection("CosmosDb collection")]
public class ConfigureEventStoresTests(CosmosDbTestContainerFixture fixture)
{
    [Fact]
    public void MissingConfiguration_ThrowsException()
    {
        // Arrange
        var hostBuilder = new HostApplicationBuilder();
        hostBuilder.Configuration.AddJsonFile("testconfig.json");
        hostBuilder.ConfigureEventStores(hostBuilder.Configuration.GetSection("MissingEventStore"));
        var app = hostBuilder.Build();
        
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => app.Services.GetRequiredService<IEventStore<ToDoList>>());
        
        
        // Assert
        var expectedExceptionMessage = "ToDoList event store is missing from configuration.";
        Assert.Equal(expectedExceptionMessage, exception.Message);
    }

    [Fact]
    public async Task CosmosContainerHasInvalidPartitionKey_ThrowsException()
    {
        // Arrange
        var hostBuilder = new HostApplicationBuilder();
        hostBuilder.Configuration.AddJsonFile("testconfig.json");
        hostBuilder.ConfigureEventStores(hostBuilder.Configuration.GetSection("InvalidPartitionKey"));
        var app = hostBuilder.Build();
        
        var container = await fixture.TestDatabase.DefineContainer("InvalidParitionKeyTestContainer", "/dumbPk")
            .WithUniqueKey()
            .Path("/version")
            .Attach()
            .CreateIfNotExistsAsync();
        
        
        // Act
        var exception = Assert.Throws<InvalidResourceException>(() => app.Services.GetRequiredService<IEventStore<ToDoList>>());
        
        
        // Assert
        var expectedExceptionMessage = "Partition Key is invalid. Container should be configured with a Partition Key Path of \"/streamId\"";
        Assert.Equal(expectedExceptionMessage, exception.Message);

        await container.Container.DeleteContainerAsync();
    }

    [Fact]
    public async Task CosmosContainerDoesNotHaveUniqueKeyPolicy_ThrowsException()
    {
        // Arrange
        var hostBuilder = new HostApplicationBuilder();
        hostBuilder.Configuration.AddJsonFile("testconfig.json");
        hostBuilder.ConfigureEventStores(hostBuilder.Configuration.GetSection("MissingEventStore"));
        var app = hostBuilder.Build();
        
        var container = await fixture.TestDatabase.DefineContainer("NoUniqueKeyTestContainer", "/streamId")
            .CreateIfNotExistsAsync();
        
        
        // Act
        var exception = Assert.Throws<InvalidResourceException>(() => app.Services.GetRequiredService<IEventStore<ToDoList>>());
        
        
        // Assert
        var expectedExceptionMessage =
            "Container is invalid. Container requires a Unique Key Path of \"/version\" to be configured";
        Assert.Equal(expectedExceptionMessage, exception.Message);

        await container.Container.DeleteContainerAsync();
    }

    [Fact]
    public void ConfigureEventStores_ReturnsEventStores()
    {
        
    }
}