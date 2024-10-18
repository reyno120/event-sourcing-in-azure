var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzureCosmosDB("fancy-cosmos").AddDatabase("fancy-db")
    .RunAsEmulator();

builder.AddProject<Projects.FancyToDo_API>("fancy-api")
    .WithReference(db);

builder.Build().Run();