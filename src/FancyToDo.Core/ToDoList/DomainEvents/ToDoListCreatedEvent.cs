﻿using SharedKernel;

namespace FancyToDo.Core.ToDoList.DomainEvents;

public record ToDoListCreatedEvent(Guid ToDoListId, string Name) : BaseDomainEvent;