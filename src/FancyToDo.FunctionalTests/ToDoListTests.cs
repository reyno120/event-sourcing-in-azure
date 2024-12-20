using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using FancyToDo.API.ToDoItemEndpoints;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FancyToDo.FunctionalTests;

// https://microsoft.github.io/AzureTipsAndTricks/blog/tip196.html

public class ToDoListTests : IDisposable
{
    // Instructions:
    // 1. Add a project reference to the target AppHost project, e.g.:
    //
    //    <ItemGroup>
    //        <ProjectReference Include="../MyAspireApp.AppHost/MyAspireApp.AppHost.csproj" />
    //    </ItemGroup>
    //
    // 2. Uncomment the following example test and update 'Projects.MyAspireApp_AppHost' to match your AppHost project:

    private Process _process;
    
    [Fact]
    public async Task CreateToDoListItem()
    {
        
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FancyToDo_API>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
    
        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();
        

        _process = new Process
        {
            StartInfo = new ProcessStartInfo(@"C:\Users\00009775\.AzureToolsForIntelliJ\AzureFunctionsCoreTools\v4\4.104.0\func.exe", 
                "host start --pause-on-error --dotnet-isolated-debug")
            {
                WorkingDirectory = @"C:\TemporaryWork\FancyToDoApp\The-Fanciest-ToDo-App-You-Will-Ever-See\src\FancyToDo.Functions\bin\Debug\net8.0"
                // CreateNoWindow = false,
                // RedirectStandardOutput = true,
                // UseShellExecute = false
            }
        };


        // TODO: Move to fixture
        // TODO: Grab logs and exceptions and redirect to output
        // TODO: Make sure process is successfully disposed and not using port (Func.exe has stopped)
        // TODO: Figure out how to attach debugger
        var processStarted = false;
        try
        {
            processStarted = _process.Start();
            Thread.Sleep(5000);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);            
        }

        // var functionHost = new WebHostBuilder().UseStartup<FancyToDo.Functions.Program>();
        // await functionHost.Build().StartAsync();

        // Act
        using var httpClient = app.CreateHttpClient("fancy-api");
        await resourceNotificationService
            .WaitForResourceAsync("fancy-api", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));

        var postObject =
            new CreateToDoItemRequest(Guid.Parse("381cafbf-9126-43ff-bbd4-eda0eef17e97"), "Functional Test");
        
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(postObject),
            Encoding.UTF8,
            "application/json");

        try
        {
            await httpClient.PostAsync("/todoitems/", jsonContent);
            Thread.Sleep(5000);
        }
        catch (Exception e)
        {
            var test = 1;
        }

        //
        // var response = await httpClient.GetAsync("/todolists");
        // var content = JsonSerializer.Deserialize<GetToDoListResponse>(
        //     await response.Content.ReadAsStreamAsync(),
        //     new JsonSerializerOptions()
        //     {
        //        PropertyNameCaseInsensitive = true
        //     });
        //
        //
        // // Assert
        // Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Assert.NotNull(content);
        // Assert.NotEqual(Guid.Empty, content.Id);
    }

    public void Dispose()
    {
        if (!_process.HasExited)
            _process.Kill(entireProcessTree: true);
 
        _process.Dispose();
    }
}