using Xunit;
using MinimalAPIProject.Model;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPIProject.Tests.Model;

public class StudentTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Test Student",
            Age = 20
        };

        // Assert
        Assert.NotNull(student);
        Assert.NotEqual(Guid.Empty, student.Id);
        Assert.Equal("Test Student", student.Name);
        Assert.Equal(20, student.Age);
    }

    [Fact]
    public void Id_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var student = new Student { Name = "Test", Age = 20 };
        var expectedId = Guid.NewGuid();

        // Act
        student.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, student.Id);
    }

    [Fact]
    public void Name_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var student = new Student { Name = "Original Name", Age = 20 };

        // Act
        student.Name = "Updated Name";

        // Assert
        Assert.Equal("Updated Name", student.Name);
    }

    [Fact]
    public void Age_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var student = new Student { Name = "Test", Age = 20 };

        // Act
        student.Age = 30;

        // Assert
        Assert.Equal(30, student.Age);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(18)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Age_ShouldAcceptVariousValues(int age)
    {
        // Arrange & Act
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = age
        };

        // Assert
        Assert.Equal(age, student.Age);
    }

    [Fact]
    public void TableAttribute_ShouldBeApplied()
    {
        // Act
        var tableAttribute = typeof(Student).GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), false)
            .FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.TableAttribute;

        // Assert
        Assert.NotNull(tableAttribute);
        Assert.Equal("students", tableAttribute.Name);
    }

    [Fact]
    public void Id_ShouldHaveKeyAttribute()
    {
        // Act
        var property = typeof(Student).GetProperty(nameof(Student.Id));
        var keyAttribute = property!.GetCustomAttributes(typeof(KeyAttribute), false)
            .FirstOrDefault() as KeyAttribute;

        // Assert
        Assert.NotNull(keyAttribute);
    }

    [Fact]
    public void Id_ShouldHaveColumnAttribute()
    {
        // Act
        var property = typeof(Student).GetProperty(nameof(Student.Id));
        var columnAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute), false)
            .FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;

        // Assert
        Assert.NotNull(columnAttribute);
        Assert.Equal("id", columnAttribute.Name);
    }

    [Fact]
    public void Name_ShouldHaveRequiredAttribute()
    {
        // Act
        var property = typeof(Student).GetProperty(nameof(Student.Name));
        var requiredAttribute = property!.GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;

        // Assert
        Assert.NotNull(requiredAttribute);
    }

    [Fact]
    public void Name_ShouldHaveMaxLengthAttribute()
    {
        // Act
        var property = typeof(Student).GetProperty(nameof(Student.Name));
        var maxLengthAttribute = property!.GetCustomAttributes(typeof(MaxLengthAttribute), false)
            .FirstOrDefault() as MaxLengthAttribute;

        // Assert
        Assert.NotNull(maxLengthAttribute);
        Assert.Equal(200, maxLengthAttribute.Length);
    }

    [Fact]
    public void Name_ShouldHaveColumnAttribute()
    {
        // Act
        var property = typeof(Student).GetProperty(nameof(Student.Name));
        var columnAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute), false)
            .FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;

        // Assert
        Assert.NotNull(columnAttribute);
        Assert.Equal("name", columnAttribute.Name);
    }

    [Fact]
    public void Age_ShouldHaveRequiredAttribute()
    {
        // Act
        var property = typeof(Student).GetProperty(nameof(Student.Age));
        var requiredAttribute = property!.GetCustomAttributes(typeof(RequiredAttribute), false)
            .FirstOrDefault() as RequiredAttribute;

        // Assert
        Assert.NotNull(requiredAttribute);
    }

    [Fact]
    public void Age_ShouldHaveColumnAttribute()
    {
        // Act
        var property = typeof(Student).GetProperty(nameof(Student.Age));
        var columnAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute), false)
            .FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;

        // Assert
        Assert.NotNull(columnAttribute);
        Assert.Equal("age", columnAttribute.Name);
    }

    [Fact]
    public void Properties_ShouldBeIndependent()
    {
        // Arrange
        var student1 = new Student { Id = Guid.NewGuid(), Name = "Student 1", Age = 20 };
        var student2 = new Student { Id = Guid.NewGuid(), Name = "Student 2", Age = 25 };

        // Assert
        Assert.NotEqual(student1.Id, student2.Id);
        Assert.NotEqual(student1.Name, student2.Name);
        Assert.NotEqual(student1.Age, student2.Age);
    }
}
