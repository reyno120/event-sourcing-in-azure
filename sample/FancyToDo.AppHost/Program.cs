
var builder = DistributedApplication.CreateBuilder(args);

var useLocalEmulator = builder.AddParameter("UseLocalEmulator").Resource.Value.Equals("true");

// TODO: Can't get Azure function built from dockerfile in FunctionalTests to work with CosmosEmulator from linux container
var cosmos = useLocalEmulator
    ? builder.AddConnectionString("fancy-cosmos")
    : builder.AddAzureCosmosDB("fancy-cosmos").RunAsEmulator(
        emulator =>
        {
            emulator.WithLifetime(ContainerLifetime.Persistent);
        });

// AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;AccountEndpoint=https://127.0.0.1:60654;DisableServerCertificateValidation=True;
var api = builder.AddProject<Projects.FancyToDo_API>("fancy-api")
    .WithReference(cosmos);

// var funcApp = builder.AddAzureFunctionsProject<Projects.FancyToDo_Functions>("funcapp")
//     .WithReference(cosmos)
//     .WaitFor(api);

if (!useLocalEmulator)
    api.WaitFor(cosmos);


builder.Build().Run();