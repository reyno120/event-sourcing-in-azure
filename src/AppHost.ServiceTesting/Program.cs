using Aspire.Hosting.Azure;
using Microsoft.Azure.Cosmos;

var builder = DistributedApplication.CreateBuilder(args);

// https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/custom-resource-commands
var cosmos = builder.AddAzureCosmosDB("cosmosemulator")
    .RunAsEmulator(emulator =>
    {
        emulator.WithGatewayPort(51395)
            // TODO: Figure out how to redirector 51395 to below url, instead of setting enviornment variable
            // won't see this with test anyway
            .WithEnvironment("Data Explorer", "https://localhost:51395/_explorer/index.html");
    });

// TODO: https://learn.microsoft.com/en-us/dotnet/aspire/database/seed-database-data?tabs=sql-server
// Figure out how to configure databases/containers once container has been started. Seeding data will be responsibility of the test


var api = builder.AddProject<Projects.FancyToDo_API>("fancy-api")
    .WithReference(cosmos, "CosmosDBConnectionString")
    .WaitFor(cosmos);

// TODO: Make this and cosmos emulator persistent for faster dev/debugging
// var functionDockerfileLocation = builder.AddParameter("FunctionDockerfileLocation");
var functionContainer = builder
    .AddDockerfile("fancy-function",
        "..", $"{Directory.GetCurrentDirectory()}/../../../../FancyToDo.Functions/Dockerfile")
    .WithEnvironment("DatabaseName", "fancy-db")
    .WithEnvironment("ContainerName", "ToDoListEventStream")
    .WithReference(cosmos, "CosmosDBConnectionString")
    .WaitFor(api);

// https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=docker-linux%2Ccsharp&pivots=api-nosql#import-the-emulators-tlsssl-certificate





builder.Build().Run();

// https://learn.microsoft.com/en-us/dotnet/aspire/database/azure-cosmos-db-integration?tabs=dotnet-cli
