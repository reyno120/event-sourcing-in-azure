using AutoMapper;
using FancyToDo.API.ToDoListEndpoints;
using FancyToDo.Core.ToDoList;

namespace FancyToDo.API.MappingProfiles;

public class ToDoProfiles : Profile
{
    public ToDoProfiles()
    {
        CreateMap<ToDoList, GetToDoListResponse>();
        CreateMap<ToDoItem, GetToDoListsResponseToDoItem>();
    }
}