﻿using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem.DomainEvents;
using SharedKernel.EventSourcing.Core;

namespace FancyToDo.Core.ToDoList.Entities.ToDoItem;

public class ToDoItem : Entity
{
    public string Task { get; private set; }
    public string Status { get; private set; } = "To Do";
    
    internal ToDoItem(ItemAddedEvent e)
    {
        this.Id = e.ItemId;
        this.Task = e.Task;
        this.Status = e.Status;
    }

    internal ToDoItem(string task)
    {
        Guard.Against.NullOrEmpty(task);
        Guard.Against.LengthOutOfRange(task, 1, 50);
        
        // We don't care that an item was created, just when it's been added to the aggregate
        this.Task = task;
    }

    internal void RenameTask(string newTaskName)
    {
        Guard.Against.NullOrEmpty(newTaskName);
        Guard.Against.LengthOutOfRange(newTaskName, 1, 50);

        Apply(new TaskRenamedEvent(this.Id, newTaskName));
    }

    internal void SetStatus(string newStatus)
    {
        // TODO
        var isStatusValid = newStatus.Equals("To Do") || newStatus.Equals("In Progress") || newStatus.Equals("Done");
        if (!isStatusValid)
            throw new InvalidOperationException("Status is invalid.");

        this.Status = newStatus;
    }


    #region Event Sourcing

    private void Handle(TaskRenamedEvent e)
    {
        this.Task = e.Name;
    }
    
    #endregion
}