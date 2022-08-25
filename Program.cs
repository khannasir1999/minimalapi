using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CmsDatabaseContext>(options =>
options.UseInMemoryDatabase("CmsDatabse"));
builder.Services.AddAutoMapper(typeof(CmsMapper));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/courses", async (CmsDatabaseContext db) =>
{
    try
    {
           var result = await db.Courses.ToListAsync();
    return Results.Ok(result);
    }
    catch (System.Exception ex)
    {
        return Results.Problem(ex.Message);
       
    }
 
});
app.MapPost("/courses", async ([FromBody] CourseDto courseDto,[FromServices] CmsDatabaseContext db,[FromServices] IMapper mapper) =>
{
    try
    {
            var newCourse = mapper.Map<Course>(courseDto);
    db.Courses.Add(newCourse);
    await db.SaveChangesAsync();
    var result = mapper.Map<CourseDto>(newCourse);
    return Results.Created($"/courses/{result.CourseId}", result);
    }
    catch (System.Exception ex)
    {
        
    return Results.Problem(ex.Message);
    }

});
app.MapGet("/courses/{courseId}" , async (int courseId , CmsDatabaseContext db, IMapper mapper) => {
var course = await db.Courses.FindAsync(courseId);
if(course == null){
    return Results.NotFound();

}


var result = mapper.Map<CourseDto>(course);
return Results.Ok(result);


});
app.MapPut("/courses/{courseId}" , async (int courseId ,CourseDto courseDto, CmsDatabaseContext db, IMapper mapper) => {
var course = await db.Courses.FindAsync(courseId);
if(course == null){
    return Results.NotFound();

}
course.CourseName = courseDto.CourseName;
course.CourseDuration = courseDto.CourseDuration;
course.CourseType = (int)courseDto.CourseType;
await db.SaveChangesAsync();
var result = mapper.Map<CourseDto>(course);
return Results.Ok(result);


});
app.Run();
public class CmsMapper: Profile{
    public CmsMapper(){
        CreateMap<Course , CourseDto>();
        CreateMap<CourseDto , Course>();
    }
}
public class Course
{
    public int CourseId { get; set; }
    public String CourseName { get; set; } = string.Empty;
    public int CourseDuration { get; set; }
    public int CourseType { get; set; }
}
public class CourseDto
{
    public int CourseId { get; set; }
    public String CourseName { get; set; } = string.Empty;
    public int CourseDuration { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Course_Type CourseType { get; set; }
}
public enum Course_Type{
    Ëngineering = 1,
    Managment = 2,
    Science = 2
}

public class CmsDatabaseContext : DbContext
{
    public DbSet<Course> Courses => Set<Course>();
    public CmsDatabaseContext(DbContextOptions options) : base(options)
    {
    }
}