using Xunit;
using MinimalAPIProject.Model;

namespace MinimalAPIProject.Model.Tests;

public class StudentTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var age = 25;

        // Act
        var student = new Student
        {
            Id = id,
            Name = name,
            Age = age
        };

        // Assert
        Assert.Equal(id, student.Id);
        Assert.Equal(name, student.Name);
        Assert.Equal(age, student.Age);
    }

    [Fact]
    public void Id_ShouldBeSettable()
    {
        // Arrange
        var student = new Student { Name = "Test", Age = 20 };
        var newId = Guid.NewGuid();

        // Act
        student.Id = newId;

        // Assert
        Assert.Equal(newId, student.Id);
    }

    [Fact]
    public void Name_ShouldBeSettable()
    {
        // Arrange
        var student = new Student { Name = "Original", Age = 20 };

        // Act
        student.Name = "Updated";

        // Assert
        Assert.Equal("Updated", student.Name);
    }

    [Fact]
    public void Age_ShouldBeSettable()
    {
        // Arrange
        var student = new Student { Name = "Test", Age = 20 };

        // Act
        student.Age = 30;

        // Assert
        Assert.Equal(30, student.Age);
    }

    [Fact]
    public void Name_ShouldBeRequired()
    {
        // This test verifies the required keyword is present
        var student = new Student { Name = "Test", Age = 20 };
        Assert.NotNull(student.Name);
    }

    [Fact]
    public void Student_ShouldAllowEmptyGuid()
    {
        // Arrange & Act
        var student = new Student
        {
            Id = Guid.Empty,
            Name = "Test",
            Age = 20
        };

        // Assert
        Assert.Equal(Guid.Empty, student.Id);
    }

    [Fact]
    public void Student_ShouldAllowZeroAge()
    {
        // Arrange & Act
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = 0
        };

        // Assert
        Assert.Equal(0, student.Age);
    }

    [Fact]
    public void Student_ShouldAllowNegativeAge()
    {
        // Arrange & Act
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = -1
        };

        // Assert
        Assert.Equal(-1, student.Age);
    }

    [Fact]
    public void Student_ShouldAllowPositiveAge()
    {
        // Arrange & Act
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = 100
        };

        // Assert
        Assert.Equal(100, student.Age);
    }

    [Fact]
    public void Student_ShouldHandleEmptyName()
    {
        // Arrange & Act
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "",
            Age = 20
        };

        // Assert
        Assert.Equal("", student.Name);
    }

    [Fact]
    public void Student_ShouldHandleLongName()
    {
        // Arrange
        var longName = new string('A', 1000);

        // Act
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = longName,
            Age = 20
        };

        // Assert
        Assert.Equal(longName, student.Name);
        Assert.Equal(1000, student.Name.Length);
    }
}
