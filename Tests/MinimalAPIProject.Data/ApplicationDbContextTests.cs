using Xunit;
using Microsoft.EntityFrameworkCore;
using MinimalAPIProject.Data;
using MinimalAPIProject.Model;

namespace MinimalAPIProject.Tests.Data;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Students_ShouldBeDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Students);
        Assert.IsAssignableFrom<DbSet<Student>>(context.Students);
    }

    [Fact]
    public void Students_ShouldBeEmptyInitially()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.Empty(context.Students);
    }

    [Fact]
    public void Context_ShouldAddStudent()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var student = new Student { Id = Guid.NewGuid(), Name = "Test Student", Age = 20 };

        // Act
        context.Students.Add(student);
        context.SaveChanges();

        // Assert
        Assert.Single(context.Students);
        Assert.Contains(context.Students, s => s.Id == student.Id);
    }

    [Fact]
    public void Context_ShouldFindStudentById()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            var student = new Student { Id = studentId, Name = "Test Student", Age = 20 };
            context.Students.Add(student);
            context.SaveChanges();
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var foundStudent = context.Students.Find(studentId);

            // Assert
            Assert.NotNull(foundStudent);
            Assert.Equal(studentId, foundStudent.Id);
            Assert.Equal("Test Student", foundStudent.Name);
        }
    }

    [Fact]
    public void Context_ShouldUpdateStudent()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            var student = new Student { Id = studentId, Name = "Original Name", Age = 20 };
            context.Students.Add(student);
            context.SaveChanges();
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var student = context.Students.Find(studentId);
            student!.Name = "Updated Name";
            context.SaveChanges();
        }

        // Assert
        using (var context = new ApplicationDbContext(options))
        {
            var student = context.Students.Find(studentId);
            Assert.Equal("Updated Name", student!.Name);
        }
    }

    [Fact]
    public void Context_ShouldDeleteStudent()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            var student = new Student { Id = studentId, Name = "Test Student", Age = 20 };
            context.Students.Add(student);
            context.SaveChanges();
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var student = context.Students.Find(studentId);
            context.Students.Remove(student!);
            context.SaveChanges();
        }

        // Assert
        using (var context = new ApplicationDbContext(options))
        {
            var student = context.Students.Find(studentId);
            Assert.Null(student);
        }
    }

    [Fact]
    public void Context_ShouldHandleMultipleStudents()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        context.Students.AddRange(
            new Student { Id = Guid.NewGuid(), Name = "Student 1", Age = 20 },
            new Student { Id = Guid.NewGuid(), Name = "Student 2", Age = 25 },
            new Student { Id = Guid.NewGuid(), Name = "Student 3", Age = 30 }
        );
        context.SaveChanges();

        // Assert
        Assert.Equal(3, context.Students.Count());
    }

    [Fact]
    public async Task Context_ShouldAddStudentAsync()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var student = new Student { Id = Guid.NewGuid(), Name = "Test Student", Age = 20 };

        // Act
        await context.Students.AddAsync(student);
        await context.SaveChangesAsync();

        // Assert
        Assert.Single(context.Students);
    }

    [Fact]
    public async Task Context_ShouldQueryStudentsAsync()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using (var context = new ApplicationDbContext(options))
        {
            context.Students.AddRange(
                new Student { Id = Guid.NewGuid(), Name = "Student 1", Age = 20 },
                new Student { Id = Guid.NewGuid(), Name = "Student 2", Age = 25 }
            );
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var students = await context.Students.ToListAsync();

            // Assert
            Assert.Equal(2, students.Count);
        }
    }
}
