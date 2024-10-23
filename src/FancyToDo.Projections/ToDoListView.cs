namespace FancyToDo.Projections;

public class ToDoListView
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ToDoListItemView> Items { get; set; } 
    
    public class ToDoListItemView
    {
        public Guid Id { get; set; }
        public string Task { get; set; }
        public string Status { get; set; }
    }
}
