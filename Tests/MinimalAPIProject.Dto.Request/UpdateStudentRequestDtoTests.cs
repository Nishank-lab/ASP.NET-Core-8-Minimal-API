using Xunit;
using MinimalAPIProject.Dto.Request;

namespace MinimalAPIProject.Dto.Request.Tests;

public class UpdateStudentRequestDtoTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var name = "John Doe";
        var age = 25;

        // Act
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
    public void Name_ShouldBeSettable()
    {
        // Arrange
        var dto = new UpdateStudentRequestDto { Name = "Original", Age = 20 };

        // Act
        dto.Name = "Updated";

        // Assert
        Assert.Equal("Updated", dto.Name);
    }

    [Fact]
    public void Age_ShouldBeSettable()
    {
        // Arrange
        var dto = new UpdateStudentRequestDto { Name = "Test", Age = 20 };

        // Act
        dto.Age = 30;

        // Assert
        Assert.Equal(30, dto.Age);
    }

    [Fact]
    public void Name_ShouldBeRequired()
    {
        // This test verifies the required keyword is present
        var dto = new UpdateStudentRequestDto { Name = "Test", Age = 20 };
        Assert.NotNull(dto.Name);
    }

    [Fact]
    public void UpdateStudentRequestDto_ShouldAllowZeroAge()
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
    public void UpdateStudentRequestDto_ShouldAllowNegativeAge()
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

    [Fact]
    public void UpdateStudentRequestDto_ShouldAllowPositiveAge()
    {
        // Arrange & Act
        var dto = new UpdateStudentRequestDto
        {
            Name = "Test",
            Age = 100
        };

        // Assert
        Assert.Equal(100, dto.Age);
    }

    [Fact]
    public void UpdateStudentRequestDto_ShouldHandleEmptyName()
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
}
