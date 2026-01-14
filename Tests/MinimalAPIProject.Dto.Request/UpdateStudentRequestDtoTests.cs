using Xunit;
using MinimalAPIProject.Dto.Request;

namespace MinimalAPIProject.Tests.Dto.Request;

public class UpdateStudentRequestDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var dto = new UpdateStudentRequestDto
        {
            Name = "Test Student",
            Age = 20
        };

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("Test Student", dto.Name);
        Assert.Equal(20, dto.Age);
    }

    [Fact]
    public void Name_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var dto = new UpdateStudentRequestDto { Name = "Initial Name", Age = 25 };

        // Act
        dto.Name = "Updated Name";

        // Assert
        Assert.Equal("Updated Name", dto.Name);
    }

    [Fact]
    public void Age_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var dto = new UpdateStudentRequestDto { Name = "Test", Age = 20 };

        // Act
        dto.Age = 30;

        // Assert
        Assert.Equal(30, dto.Age);
    }

    [Theory]
    [InlineData("John Doe", 25)]
    [InlineData("Jane Smith", 30)]
    [InlineData("Bob Johnson", 18)]
    public void Constructor_ShouldAcceptVariousValues(string name, int age)
    {
        // Arrange & Act
        var dto = new UpdateStudentRequestDto
        {
            Name = name,
            Age = age
        };

        // Assert
        Assert.Equal(name, dto.Name);
        Assert.Equal(age, dto.Age);
    }

    [Fact]
    public void Age_ShouldAllowZero()
    {
        // Arrange & Act
        var dto = new UpdateStudentRequestDto
        {
            Name = "Test",
            Age = 0
        };

        // Assert
        Assert.Equal(0, dto.Age);
    }

    [Fact]
    public void Age_ShouldAllowNegativeValues()
    {
        // Arrange & Act
        var dto = new UpdateStudentRequestDto
        {
            Name = "Test",
            Age = -1
        };

        // Assert
        Assert.Equal(-1, dto.Age);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(18)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Age_ShouldAcceptBoundaryValues(int age)
    {
        // Arrange & Act
        var dto = new UpdateStudentRequestDto
        {
            Name = "Test",
            Age = age
        };

        // Assert
        Assert.Equal(age, dto.Age);
    }

    [Fact]
    public void Name_ShouldAllowEmptyString()
    {
        // Arrange & Act
        var dto = new UpdateStudentRequestDto
        {
            Name = "",
            Age = 20
        };

        // Assert
        Assert.Equal("", dto.Name);
    }

    [Fact]
    public void Properties_ShouldBeIndependent()
    {
        // Arrange
        var dto1 = new UpdateStudentRequestDto { Name = "Student 1", Age = 20 };
        var dto2 = new UpdateStudentRequestDto { Name = "Student 2", Age = 25 };

        // Assert
        Assert.NotEqual(dto1.Name, dto2.Name);
        Assert.NotEqual(dto1.Age, dto2.Age);
    }
}
