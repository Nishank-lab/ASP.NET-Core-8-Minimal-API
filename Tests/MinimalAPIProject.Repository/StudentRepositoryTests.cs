using Xunit;
using Microsoft.EntityFrameworkCore;
using MinimalAPIProject.Repository;
using MinimalAPIProject.Data;
using MinimalAPIProject.Model;

namespace MinimalAPIProject.Tests.Repository;

public class StudentRepositoryTests
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
        using var context = new ApplicationDbContext(options);

        // Act
        var repository = new StudentRepository(context);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task CreateStudent_ShouldAddStudentAndReturnIt()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new StudentRepository(context);
        var student = new Student { Id = Guid.NewGuid(), Name = "Test Student", Age = 20 };

        // Act
        var result = await repository.CreateStudent(student);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.Id, result.Id);
        Assert.Equal(student.Name, result.Name);
        Assert.Equal(student.Age, result.Age);
        Assert.Single(context.Students);
    }

    [Fact]
    public async Task CreateStudent_ShouldPersistToDatabase()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new StudentRepository(context);
            var student = new Student { Id = studentId, Name = "Test Student", Age = 20 };
            await repository.CreateStudent(student);
        }

        // Act & Assert
        using (var context = new ApplicationDbContext(options))
        {
            var foundStudent = await context.Students.FindAsync(studentId);
            Assert.NotNull(foundStudent);
            Assert.Equal("Test Student", foundStudent.Name);
        }
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnTrueWhenStudentExists()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            context.Students.Add(new Student { Id = studentId, Name = "Test Student", Age = 20 });
            await context.SaveChangesAsync();
        }

        // Act
        bool result;
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new StudentRepository(context);
            result = await repository.DeleteStudent(studentId);
        }

        // Assert
        Assert.True(result);
        using (var context = new ApplicationDbContext(options))
        {
            var student = await context.Students.FindAsync(studentId);
            Assert.Null(student);
        }
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnFalseWhenStudentDoesNotExist()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();

        // Act
        using var context = new ApplicationDbContext(options);
        var repository = new StudentRepository(context);
        var result = await repository.DeleteStudent(studentId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnAllStudents()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using (var context = new ApplicationDbContext(options))
        {
            context.Students.AddRange(
                new Student { Id = Guid.NewGuid(), Name = "Student 1", Age = 20 },
                new Student { Id = Guid.NewGuid(), Name = "Student 2", Age = 25 },
                new Student { Id = Guid.NewGuid(), Name = "Student 3", Age = 30 }
            );
            await context.SaveChangesAsync();
        }

        // Act
        IEnumerable<Student> result;
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new StudentRepository(context);
            result = await repository.GetAllStudents();
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnEmptyListWhenNoStudents()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        var repository = new StudentRepository(context);
        var result = await repository.GetAllStudents();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnStudentWhenExists()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            context.Students.Add(new Student { Id = studentId, Name = "Test Student", Age = 20 });
            await context.SaveChangesAsync();
        }

        // Act
        Student? result;
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new StudentRepository(context);
            result = await repository.GetStudentById(studentId);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentId, result.Id);
        Assert.Equal("Test Student", result.Name);
        Assert.Equal(20, result.Age);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnNullWhenDoesNotExist()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();

        // Act
        using var context = new ApplicationDbContext(options);
        var repository = new StudentRepository(context);
        var result = await repository.GetStudentById(studentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnTrueAndUpdateWhenStudentExists()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            context.Students.Add(new Student { Id = studentId, Name = "Original Name", Age = 20 });
            await context.SaveChangesAsync();
        }

        // Act
        bool? result;
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new StudentRepository(context);
            var updatedStudent = new Student { Id = studentId, Name = "Updated Name", Age = 30 };
            result = await repository.UpdateStudent(updatedStudent);
        }

        // Assert
        Assert.True(result);
        using (var context = new ApplicationDbContext(options))
        {
            var student = await context.Students.FindAsync(studentId);
            Assert.Equal("Updated Name", student!.Name);
            Assert.Equal(30, student.Age);
        }
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNullWhenStudentDoesNotExist()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();

        // Act
        using var context = new ApplicationDbContext(options);
        var repository = new StudentRepository(context);
        var updatedStudent = new Student { Id = studentId, Name = "Updated Name", Age = 30 };
        var result = await repository.UpdateStudent(updatedStudent);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStudent_ShouldOnlyUpdateNameAndAge()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var studentId = Guid.NewGuid();
        using (var context = new ApplicationDbContext(options))
        {
            context.Students.Add(new Student { Id = studentId, Name = "Original Name", Age = 20 });
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new StudentRepository(context);
            var updatedStudent = new Student { Id = studentId, Name = "Updated Name", Age = 30 };
            await repository.UpdateStudent(updatedStudent);
        }

        // Assert
        using (var context = new ApplicationDbContext(options))
        {
            var student = await context.Students.FindAsync(studentId);
            Assert.Equal(studentId, student!.Id);
            Assert.Equal("Updated Name", student.Name);
            Assert.Equal(30, student.Age);
        }
    }
}
