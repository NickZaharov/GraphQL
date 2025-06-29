using System.ComponentModel.DataAnnotations;

namespace GraphQL.Data
{
    public class TaskItem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedById { get; set; }
        public required User CreatedBy { get; set; }

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }
    }
}
