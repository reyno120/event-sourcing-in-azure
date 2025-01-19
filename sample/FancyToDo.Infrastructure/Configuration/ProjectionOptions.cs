namespace FancyToDo.Infrastructure.Configuration;

public class ProjectionOptions
{
    public const string Projection = "Projection";
    
    public string? DatabaseName { get; init; } 
    public string? ContainerName { get; init; }
}