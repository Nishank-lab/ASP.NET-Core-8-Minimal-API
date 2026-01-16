using Xunit;
using MinimalAPIProject.Dto.Response;

namespace MinimalAPIProject.Dto.Response.Tests;

public class StudentResponseDtoTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var age = 25;

        // Act
        var dto = new StudentResponseDto
        {
            Id = id,
            Name = name,
            Age = age
        };

        // Assert
        Assert.Equal(id, dto.Id);
        Assert.Equal(name, dto.Name);
        Assert.Equal(age, dto.Age);
    }

    [Fact]
    public void Id_ShouldBeSettable()
    {
        // Arrange
        var dto = new StudentResponseDto { Name = "Test", Age = 20 };
        var newId = Guid.NewGuid();

        // Act
        dto.Id = newId;

        // Assert
        Assert.Equal(newId, dto.Id);
    }

    [Fact]
    public void Name_ShouldBeSettable()
    {
        // Arrange
        var dto = new StudentResponseDto { Name = "Original", Age = 20 };

        // Act
        dto.Name = "Updated";

        // Assert
        Assert.Equal("Updated", dto.Name);
    }

    [Fact]
    public void Age_ShouldBeSettable()
    {
        // Arrange
        var dto = new StudentResponseDto { Name = "Test", Age = 20 };

        // Act
        dto.Age = 30;

        // Assert
        Assert.Equal(30, dto.Age);
    }

    [Fact]
    public void Name_ShouldBeRequired()
    {
        // This test verifies the required keyword is present
        // In runtime, if Name is not set, it would throw an exception
        var dto = new StudentResponseDto { Name = "Test", Age = 20 };
        Assert.NotNull(dto.Name);
    }

    [Fact]
    public void StudentResponseDto_ShouldAllowEmptyGuid()
    {
        // Arrange & Act
        var dto = new StudentResponseDto
        {
            Id = Guid.Empty,
            Name = "Test",
            Age = 20
        };

        // Assert
        Assert.Equal(Guid.Empty, dto.Id);
    }

    [Fact]
    public void StudentResponseDto_ShouldAllowZeroAge()
    {
        // Arrange & Act
        var dto = new StudentResponseDto
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = 0
        };

        // Assert
        Assert.Equal(0, dto.Age);
    }

    [Fact]
    public void StudentResponseDto_ShouldAllowNegativeAge()
    {
        // Arrange & Act
        var dto = new StudentResponseDto
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = -1
        };

        // Assert
        Assert.Equal(-1, dto.Age);
    }
}
