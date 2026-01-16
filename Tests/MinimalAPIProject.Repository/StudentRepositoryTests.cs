using Xunit;
using MinimalAPIProject.Repository;
using MinimalAPIProject.Model;
using MinimalAPIProject.Mock_Data;

namespace MinimalAPIProject.Repository.Tests;

public class StudentRepositoryTests
{
    private readonly StudentRepository _repository;

    public StudentRepositoryTests()
    {
        _repository = new StudentRepository();
    }

    [Fact]
    public void Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var repository = new StudentRepository();

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task CreateStudent_ShouldAddStudentToCollection()
    {
        // Arrange
        var initialCount = StudentMock.students.Count();
        var student = new Student { Id = Guid.NewGuid(), Name = "New Student", Age = 25 };

        // Act
        var result = await _repository.CreateStudent(student);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.Id, result.Id);
        Assert.Equal(student.Name, result.Name);
        Assert.Equal(student.Age, result.Age);
    }

    [Fact]
    public async Task CreateStudent_ShouldReturnCreatedStudent()
    {
        // Arrange
        var student = new Student { Id = Guid.NewGuid(), Name = "Test Student", Age = 30 };

        // Act
        var result = await _repository.CreateStudent(student);

        // Assert
        Assert.Same(student, result);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnTrue_WhenStudentExists()
    {
        // Arrange
        var student = new Student { Id = Guid.NewGuid(), Name = "To Delete", Age = 25 };
        await _repository.CreateStudent(student);

        // Act
        var result = await _repository.DeleteStudent(student.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnFalse_WhenStudentDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteStudent(nonExistentId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnAllStudents()
    {
        // Act
        var result = await _repository.GetAllStudents();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Student>>(result);
    }

    [Fact]
    public async Task GetAllStudents_ShouldNotReturnNull()
    {
        // Act
        var result = await _repository.GetAllStudents();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnStudent_WhenExists()
    {
        // Arrange
        var student = new Student { Id = Guid.NewGuid(), Name = "Find Me", Age = 28 };
        await _repository.CreateStudent(student);

        // Act
        var result = await _repository.GetStudentById(student.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.Id, result.Id);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetStudentById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnTrue_WhenStudentExists()
    {
        // Arrange
        var student = new Student { Id = Guid.NewGuid(), Name = "Original", Age = 25 };
        await _repository.CreateStudent(student);
        var updatedStudent = new Student { Id = student.Id, Name = "Updated", Age = 30 };

        // Act
        var result = await _repository.UpdateStudent(updatedStudent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNull_WhenStudentDoesNotExist()
    {
        // Arrange
        var nonExistentStudent = new Student { Id = Guid.NewGuid(), Name = "Non Existent", Age = 25 };

        // Act
        var result = await _repository.UpdateStudent(nonExistentStudent);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStudent_ShouldModifyStudentProperties()
    {
        // Arrange
        var student = new Student { Id = Guid.NewGuid(), Name = "Original", Age = 25 };
        await _repository.CreateStudent(student);
        var updatedStudent = new Student { Id = student.Id, Name = "Modified", Age = 35 };

        // Act
        await _repository.UpdateStudent(updatedStudent);
        var result = await _repository.GetStudentById(student.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Modified", result.Name);
        Assert.Equal(35, result.Age);
    }
}
