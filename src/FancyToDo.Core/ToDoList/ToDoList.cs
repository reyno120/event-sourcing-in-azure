using Ardalis.GuardClauses;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using SharedKernel.EventSourcing.Core;

namespace FancyToDo.Core.ToDoList;

public partial class ToDoList : AggregateRoot 
{
    public string Name { get; private set; }

    private readonly List<ToDoItem> _items = []; 

    public IReadOnlyList<ToDoItem> Items => _items.AsReadOnly();

    public ToDoList(string name)
    {
        Guard.Against.NullOrEmpty(name);
        Guard.Against.LengthOutOfRange(name, 1, 50);

        Apply(new ToDoListCreatedEvent(this.Id, name));
    }

    public void AddToDo(string task)
    {
        if (_items.Any(a => a.Task.Equals(task)))
            throw new InvalidOperationException("Task already exists with that name.");

        var item = new ToDoItem(task);
        Apply(new ItemAddedEvent(this.Id, item.Id, item.Task, item.Status));
    }

    public void RenameTask(Guid taskId, string name)
    {
        var task = _items.SingleOrDefault(w => w.Id == taskId);
        if (task == null)
            throw new InvalidOperationException("Item Id does not exist.");
        
        task.RenameTask(name);
    }
}