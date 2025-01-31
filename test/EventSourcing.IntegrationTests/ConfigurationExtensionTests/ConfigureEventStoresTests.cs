using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EventSourcing.IntegrationTests.ConfigurationExtensionTests;

public class ConfigureEventStoresTests
{
    [Fact]
    public void ConfigureEventStores_DefaultConfigurationSection()
    {
        // Arrange
        var hostBuilder = new HostApplicationBuilder();
        hostBuilder.Configuration.AddJsonFile("Extensions/TestConfig.json");

        hostBuilder.ConfigureEventStores();

        var app = hostBuilder.Build();
        
        
        // Act
        var namedOptions = app.Services.GetRequiredService<IOptionsMonitor<EventStoreOptions>>();
        EventStoreOptions eventStoreAOptions = namedOptions.Get("EventStoreA");
        EventStoreOptions eventStoreBOptions = namedOptions.Get("EventStoreB");
        EventStoreOptions eventStoreCOptions = namedOptions.Get("EventStoreC");
        
        
        // Assert
        Assert.Equal("DatabaseNameA", eventStoreAOptions.DatabaseName);
        Assert.Equal("ContainerNameA", eventStoreAOptions.ContainerName);
        Assert.Equal("DatabaseNameB", eventStoreBOptions.DatabaseName);
        Assert.Equal("ContainerNameB", eventStoreBOptions.ContainerName);
        
        Assert.Null(eventStoreCOptions.DatabaseName);
        Assert.Null(eventStoreCOptions.ContainerName);
    }
    
    [Fact]
    public void ConfigureEventStores_OverrideDefaultConfigurationSection()
    {
        // Arrange
        var hostBuilder = new HostApplicationBuilder();
        hostBuilder.Configuration.AddJsonFile("Extensions/TestConfig.json");

        hostBuilder.ConfigureEventStores(hostBuilder.Configuration.GetSection("OverrideEventStoresSection"));

        var app = hostBuilder.Build();
        
        
        // Act
        var namedOptions = app.Services.GetRequiredService<IOptionsMonitor<EventStoreOptions>>();
        EventStoreOptions eventStoreAOptions = namedOptions.Get("EventStoreA");
        EventStoreOptions eventStoreCOptions = namedOptions.Get("EventStoreC");
        EventStoreOptions eventStoreDOptions = namedOptions.Get("EventStoreD");
        
        
        // Assert
        Assert.Null(eventStoreAOptions.DatabaseName);
        Assert.Null(eventStoreAOptions.ContainerName);
        
        Assert.Equal("DatabaseNameC", eventStoreCOptions.DatabaseName);
        Assert.Equal("ContainerNameC", eventStoreCOptions.ContainerName);
        Assert.Equal("DatabaseNameD", eventStoreDOptions.DatabaseName);
        Assert.Equal("ContainerNameD", eventStoreDOptions.ContainerName);
    }
    
    [Fact]
    public void ConfigureEventStoresTests_MissingConfigurations()
    {
        // Arrange
        var hostBuilder = new HostApplicationBuilder();
        hostBuilder.Configuration.AddJsonFile("Extensions/TestConfig.json");

        
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            hostBuilder.ConfigureEventStores(hostBuilder.Configuration.GetSection("MissingConfigurations")));
        
        
        // Assert
        var expectedExceptionMessage = "ContainerName is missing from EventStoreE event store configuration.";
        Assert.Equal(expectedExceptionMessage, exception.Message);
    }
}