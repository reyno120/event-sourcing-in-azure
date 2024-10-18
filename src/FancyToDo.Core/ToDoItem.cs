using Ardalis.GuardClauses;
using SharedKernel;

namespace FancyToDo.Core;

public class ToDoItem : Entity
{
    public Guid Id { get; init; }
    public string Task { get; private set; }
    public string Status { get; private set; } = "To Do";

    public ToDoItem(string task)
    {
        Guard.Against.NullOrEmpty(task);
        Guard.Against.LengthOutOfRange(task, 1, 50);

        this.Task = task;
        this.Id = Guid.NewGuid();
    }

    public void RenameTask(string newTaskName)
    {
        Guard.Against.NullOrEmpty(newTaskName);
        Guard.Against.LengthOutOfRange(newTaskName, 1, 50);

        this.Task = newTaskName;
    }

    public void SetStatus(string newStatus)
    {
        var isStatusValid = newStatus.Equals("To Do") || newStatus.Equals("In Progress") || newStatus.Equals("Done");
        if (!isStatusValid)
            throw new InvalidOperationException("Status is invalid.");

        this.Status = newStatus;
    }
}