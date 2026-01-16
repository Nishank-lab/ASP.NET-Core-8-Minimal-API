using Xunit;
using Moq;
using MinimalAPIProject.Service;
using MinimalAPIProject.Repository;
using MinimalAPIProject.Model;
using MinimalAPIProject.Dto.Request;

namespace MinimalAPIProject.Service.Tests;

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
    public void Constructor_ShouldInitializeWithRepository()
    {
        // Arrange & Act
        var service = new StudentService(_mockRepository.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task CreateStudent_ShouldReturnStudent()
    {
        // Arrange
        var dto = new CreateStudentRequestDto { Name = "John", Age = 25 };
        var expectedStudent = new Student { Id = Guid.NewGuid(), Name = "John", Age = 25 };
        _mockRepository.Setup(r => r.CreateStudent(It.IsAny<Student>())).ReturnsAsync(expectedStudent);

        // Act
        var result = await _service.CreateStudent(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Age, result.Age);
        _mockRepository.Verify(r => r.CreateStudent(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public async Task CreateStudent_ShouldGenerateNewGuid()
    {
        // Arrange
        var dto = new CreateStudentRequestDto { Name = "Jane", Age = 30 };
        var expectedStudent = new Student { Id = Guid.NewGuid(), Name = "Jane", Age = 30 };
        _mockRepository.Setup(r => r.CreateStudent(It.IsAny<Student>())).ReturnsAsync(expectedStudent);

        // Act
        var result = await _service.CreateStudent(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnTrue_WhenStudentExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteStudent(id)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteStudent(id);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteStudent(id), Times.Once);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnFalse_WhenStudentDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteStudent(id)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteStudent(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnListOfStudents()
    {
        // Arrange
        var students = new List<Student>
        {
            new Student { Id = Guid.NewGuid(), Name = "John", Age = 25 },
            new Student { Id = Guid.NewGuid(), Name = "Jane", Age = 30 }
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
    public async Task GetAllStudents_ShouldReturnEmptyList_WhenNoStudents()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllStudents()).ReturnsAsync(new List<Student>());

        // Act
        var result = await _service.GetAllStudents();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnStudent_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var student = new Student { Id = id, Name = "John", Age = 25 };
        _mockRepository.Setup(r => r.GetStudentById(id)).ReturnsAsync(student);

        // Act
        var result = await _service.GetStudentById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("John", result.Name);
    }

    [Fact]
    public async Task GetStudentById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetStudentById(id)).ReturnsAsync((Student?)null);

        // Act
        var result = await _service.GetStudentById(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnTrue_WhenUpdateSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateStudentRequestDto { Name = "Updated", Age = 35 };
        _mockRepository.Setup(r => r.UpdateStudent(It.IsAny<Student>())).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateStudent(id, dto);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateStudent(It.Is<Student>(s => s.Id == id && s.Name == dto.Name && s.Age == dto.Age)), Times.Once);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNull_WhenStudentNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateStudentRequestDto { Name = "Updated", Age = 35 };
        _mockRepository.Setup(r => r.UpdateStudent(It.IsAny<Student>())).ReturnsAsync((bool?)null);

        // Act
        var result = await _service.UpdateStudent(id, dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnFalse_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateStudentRequestDto { Name = "Updated", Age = 35 };
        _mockRepository.Setup(r => r.UpdateStudent(It.IsAny<Student>())).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateStudent(id, dto);

        // Assert
        Assert.False(result);
    }
}
