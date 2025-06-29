namespace GraphQL.Data
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public string Role { get; set; } = "User";
        
        public ICollection<TaskItem> CreatedByTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> AssignedToTasks { get; set; } = new List<TaskItem>();

    }
}
