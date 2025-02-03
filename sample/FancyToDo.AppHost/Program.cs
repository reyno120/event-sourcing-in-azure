
var builder = DistributedApplication.CreateBuilder(args);

var useLocalEmulator = builder.AddParameter("UseLocalEmulator").Resource.Value.Equals("true");

// TODO: Can't get Azure function built from dockerfile in FunctionalTests to work with CosmosEmulator from linux container
var cosmos = useLocalEmulator
    ? builder.AddConnectionString("fancy-cosmos")
    : builder.AddAzureCosmosDB("fancy-cosmos").RunAsEmulator();


var api = builder.AddProject<Projects.FancyToDo_API>("fancy-api")
    .WithReference(cosmos);

// var funcApp = builder.AddAzureFunctionsProject<Projects.AzureFunctionsEndToEnd_Functions>("funcapp")

if (!useLocalEmulator)
    api.WaitFor(cosmos);
        // .WaitFor(funcApp);


builder.Build().Run();