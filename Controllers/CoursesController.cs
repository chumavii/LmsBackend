using System.Security.Claims;
using LmsApi.Data;
using LmsApi.DTOs;
using LmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseListDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Select(c => new CourseListDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    InstructorName = c.Instructor != null ? c.Instructor.FullName : null,
                    InstructorEmail = c.Instructor != null ? c.Instructor.Email : null
                }).ToListAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseListDto>> GetCourse(int id)
        {
            var course = await _context.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) 
                return NotFound();

            var courseDto = new CourseListDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorName = course.Instructor?.FullName,
                InstructorEmail = course.Instructor?.Email
            };
            return Ok(courseDto);
        }

        /// <summary>
        /// Creates a new course for the authenticated instructor.
        /// </summary>
        /// <param name="model">Course creation data.</param>
        /// <returns>Created course details.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(typeof(Course), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)
                return Unauthorized();

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                InstructorId = userId
            };
            
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course); ;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, Course updated)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) 
                return NotFound();

            course.Title = updated.Title;
            course.Description = updated.Description;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) 
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
