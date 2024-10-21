var builder = DistributedApplication.CreateBuilder(args);

var connectionString = builder.AddConnectionString("local-fancy-cosmos");
var db = builder.AddAzureCosmosDB("fancy-cosmos")
    .RunAsEmulator()
    .AddDatabase("fancy-db");

builder.AddProject<Projects.FancyToDo_API>("fancy-api")
    .WithReference(connectionString)
    .WithReference(db);

// builder.AddProject<Projects.FancyToDo_Functions>("fancytodo-functions");

builder.Build().Run();