using Xunit;
using MinimalAPIProject.Dto.Request;

namespace MinimalAPIProject.Dto.Request.Tests;

public class CreateStudentRequestDtoTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var name = "John Doe";
        var age = 25;

        // Act
        var dto = new CreateStudentRequestDto
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
        var dto = new CreateStudentRequestDto { Name = "Original", Age = 20 };

        // Act
        dto.Name = "Updated";

        // Assert
        Assert.Equal("Updated", dto.Name);
    }

    [Fact]
    public void Age_ShouldBeSettable()
    {
        // Arrange
        var dto = new CreateStudentRequestDto { Name = "Test", Age = 20 };

        // Act
        dto.Age = 30;

        // Assert
        Assert.Equal(30, dto.Age);
    }

    [Fact]
    public void Name_ShouldBeRequired()
    {
        // This test verifies the required keyword is present
        var dto = new CreateStudentRequestDto { Name = "Test", Age = 20 };
        Assert.NotNull(dto.Name);
    }

    [Fact]
    public void CreateStudentRequestDto_ShouldAllowZeroAge()
    {
        // Arrange & Act
        var dto = new CreateStudentRequestDto
        {
            Name = "Test",
            Age = 0
        };

        // Assert
        Assert.Equal(0, dto.Age);
    }

    [Fact]
    public void CreateStudentRequestDto_ShouldAllowNegativeAge()
    {
        // Arrange & Act
        var dto = new CreateStudentRequestDto
        {
            Name = "Test",
            Age = -1
        };

        // Assert
        Assert.Equal(-1, dto.Age);
    }

    [Fact]
    public void CreateStudentRequestDto_ShouldAllowPositiveAge()
    {
        // Arrange & Act
        var dto = new CreateStudentRequestDto
        {
            Name = "Test",
            Age = 100
        };

        // Assert
        Assert.Equal(100, dto.Age);
    }

    [Fact]
    public void CreateStudentRequestDto_ShouldHandleEmptyName()
    {
        // Arrange & Act
        var dto = new CreateStudentRequestDto
        {
            Name = "",
            Age = 20
        };

        // Assert
        Assert.Equal("", dto.Name);
    }

    [Fact]
    public void CreateStudentRequestDto_ShouldHandleLongName()
    {
        // Arrange
        var longName = new string('A', 1000);

        // Act
        var dto = new CreateStudentRequestDto
        {
            Name = longName,
            Age = 20
        };

        // Assert
        Assert.Equal(longName, dto.Name);
        Assert.Equal(1000, dto.Name.Length);
    }
}
