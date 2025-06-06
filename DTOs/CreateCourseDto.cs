using System.ComponentModel.DataAnnotations;

namespace LmsApi.DTOs
{
    public class CreateCourseDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}