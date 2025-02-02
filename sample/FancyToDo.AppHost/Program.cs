
var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzureCosmosDB("fancy-cosmos")
    .RunAsEmulator();

builder.AddProject<Projects.FancyToDo_API>("fancy-api")
    .WaitFor(db)
    .WithReference(db);

builder.Build().Run();