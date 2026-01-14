using Xunit;
using Moq;
using MinimalAPIProject.Service;
using MinimalAPIProject.Repository;
using MinimalAPIProject.Model;
using MinimalAPIProject.Dto.Request;

namespace MinimalAPIProject.Tests.Service;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _mockRepository;
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        _mockRepository = new Mock<IStudentRepository>();
        _service = new StudentService(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockRepo = new Mock<IStudentRepository>();

        // Act
        var service = new StudentService(mockRepo.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task CreateStudent_ShouldCallRepositoryAndReturnStudent()
    {
        // Arrange
        var createDto = new CreateStudentRequestDto
        {
            Name = "Test Student",
            Age = 20
        };
        var expectedStudent = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Test Student",
            Age = 20
        };
        _mockRepository.Setup(r => r.CreateStudent(It.IsAny<Student>())).ReturnsAsync(expectedStudent);

        // Act
        var result = await _service.CreateStudent(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Age, result.Age);
        Assert.NotEqual(Guid.Empty, result.Id);
        _mockRepository.Verify(r => r.CreateStudent(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public async Task CreateStudent_ShouldGenerateNewGuid()
    {
        // Arrange
        var createDto = new CreateStudentRequestDto { Name = "Test", Age = 20 };
        _mockRepository.Setup(r => r.CreateStudent(It.IsAny<Student>())).ReturnsAsync((Student s) => s);

        // Act
        var result = await _service.CreateStudent(createDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task DeleteStudent_ShouldCallRepositoryAndReturnTrue()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteStudent(studentId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteStudent(studentId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteStudent(studentId), Times.Once);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnFalseWhenNotFound()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteStudent(studentId)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteStudent(studentId);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteStudent(studentId), Times.Once);
    }

    [Fact]
    public async Task GetAllStudents_ShouldCallRepositoryAndReturnStudents()
    {
        // Arrange
        var students = new List<Student>
        {
            new Student { Id = Guid.NewGuid(), Name = "Student 1", Age = 20 },
            new Student { Id = Guid.NewGuid(), Name = "Student 2", Age = 25 }
        };
        _mockRepository.Setup(r => r.GetAllStudents()).ReturnsAsync(students);

        // Act
        var result = await _service.GetAllStudents();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllStudents(), Times.Once);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnEmptyListWhenNoStudents()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllStudents()).ReturnsAsync(new List<Student>());

        // Act
        var result = await _service.GetAllStudents();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllStudents(), Times.Once);
    }

    [Fact]
    public async Task GetStudentById_ShouldCallRepositoryAndReturnStudent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = new Student { Id = studentId, Name = "Test Student", Age = 20 };
        _mockRepository.Setup(r => r.GetStudentById(studentId)).ReturnsAsync(student);

        // Act
        var result = await _service.GetStudentById(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentId, result.Id);
        Assert.Equal("Test Student", result.Name);
        _mockRepository.Verify(r => r.GetStudentById(studentId), Times.Once);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetStudentById(studentId)).ReturnsAsync((Student?)null);

        // Act
        var result = await _service.GetStudentById(studentId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetStudentById(studentId), Times.Once);
    }

    [Fact]
    public async Task UpdateStudent_ShouldCallRepositoryAndReturnTrue()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var updateDto = new UpdateStudentRequestDto { Name = "Updated Name", Age = 30 };
        _mockRepository.Setup(r => r.UpdateStudent(It.IsAny<Student>())).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateStudent(studentId, updateDto);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateStudent(It.Is<Student>(s =>
            s.Id == studentId && s.Name == updateDto.Name && s.Age == updateDto.Age)), Times.Once);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var updateDto = new UpdateStudentRequestDto { Name = "Updated Name", Age = 30 };
        _mockRepository.Setup(r => r.UpdateStudent(It.IsAny<Student>())).ReturnsAsync((bool?)null);

        // Act
        var result = await _service.UpdateStudent(studentId, updateDto);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateStudent(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnFalseWhenUpdateFails()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var updateDto = new UpdateStudentRequestDto { Name = "Updated Name", Age = 30 };
        _mockRepository.Setup(r => r.UpdateStudent(It.IsAny<Student>())).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateStudent(studentId, updateDto);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateStudent(It.IsAny<Student>()), Times.Once);
    }
}
