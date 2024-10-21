namespace FancyToDo.Functions;

public interface IEventHandler
{
    Task Handle(object e);
}