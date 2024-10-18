using AutoMapper;
using Core;
using WebAPI.ToDoListEndpoints;

namespace FancyToDo.API.MappingProfiles;

public class ToDoProfiles : Profile
{
    public ToDoProfiles()
    {
        CreateMap<ToDoList, GetToDoListsResponse>();
        CreateMap<ToDoItem, GetToDoListsResponseToDoItem>();
    }
}