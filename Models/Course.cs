namespace LmsApi.Models
{
    public class Course
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? InstructorId { get; set; }
        public ApplicationUser? Instructor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
