using Ardalis.GuardClauses;
using FancyToDo.Core.ToDoList.DomainEvents;
using SharedKernel;

namespace FancyToDo.Core.ToDoList;

public class ToDoItem : Entity
{
    private readonly Action<BaseDomainEvent> _applier;
    public Guid Id { get; init; }
    public string Task { get; private set; }
    public string Status { get; private set; } = "To Do";

    internal ToDoItem(string task, Action<BaseDomainEvent> applier)
    {
        _applier = applier;
        
        Guard.Against.NullOrEmpty(task);
        Guard.Against.LengthOutOfRange(task, 1, 50);

        this.Task = task;
        this.Id = Guid.NewGuid();
    }

    internal void RenameTask(string newTaskName)
    {
        Guard.Against.NullOrEmpty(newTaskName);
        Guard.Against.LengthOutOfRange(newTaskName, 1, 50);

        _applier(new TaskRenamedEvent(this.Id, newTaskName));
    }

    internal void SetStatus(string newStatus)
    {
        var isStatusValid = newStatus.Equals("To Do") || newStatus.Equals("In Progress") || newStatus.Equals("Done");
        if (!isStatusValid)
            throw new InvalidOperationException("Status is invalid.");

        this.Status = newStatus;
    }
    
    internal void Mutate(BaseDomainEvent @event)
    {
        ((dynamic)this).When((dynamic)@event);
    }

    private void When(TaskRenamedEvent e)
    {
        this.Task = e.Name;
    }
}