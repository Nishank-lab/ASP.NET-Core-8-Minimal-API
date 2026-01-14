using Xunit;
using MinimalAPIProject.Dto.Response;

namespace MinimalAPIProject.Tests.Dto.Response;

public class StudentResponseDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var dto = new StudentResponseDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Student",
            Age = 20
        };

        // Assert
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Test Student", dto.Name);
        Assert.Equal(20, dto.Age);
    }

    [Fact]
    public void Id_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var dto = new StudentResponseDto { Name = "Test", Age = 25 };
        var expectedId = Guid.NewGuid();

        // Act
        dto.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, dto.Id);
    }

    [Fact]
    public void Name_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var dto = new StudentResponseDto { Name = "Initial Name", Age = 25 };

        // Act
        dto.Name = "Updated Name";

        // Assert
        Assert.Equal("Updated Name", dto.Name);
    }

    [Fact]
    public void Age_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var dto = new StudentResponseDto { Name = "Test", Age = 20 };

        // Act
        dto.Age = 30;

        // Assert
        Assert.Equal(30, dto.Age);
    }

    [Fact]
    public void Age_ShouldAllowZero()
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
    public void Age_ShouldAllowNegativeValues()
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

    [Theory]
    [InlineData(0)]
    [InlineData(18)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Age_ShouldAcceptVariousValues(int age)
    {
        // Arrange & Act
        var dto = new StudentResponseDto
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = age
        };

        // Assert
        Assert.Equal(age, dto.Age);
    }

    [Fact]
    public void Properties_ShouldBeIndependent()
    {
        // Arrange
        var dto1 = new StudentResponseDto { Id = Guid.NewGuid(), Name = "Student 1", Age = 20 };
        var dto2 = new StudentResponseDto { Id = Guid.NewGuid(), Name = "Student 2", Age = 25 };

        // Assert
        Assert.NotEqual(dto1.Id, dto2.Id);
        Assert.NotEqual(dto1.Name, dto2.Name);
        Assert.NotEqual(dto1.Age, dto2.Age);
    }
}
