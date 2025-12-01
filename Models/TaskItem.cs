namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PriorityLevel Priority { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }

    public enum PriorityLevel
    {
        Baja = 1,
        Media = 2,
        Alta = 3
    }
}
