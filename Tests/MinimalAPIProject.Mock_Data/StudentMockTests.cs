using Xunit;
using MinimalAPIProject.Mock_Data;
using MinimalAPIProject.Model;

namespace MinimalAPIProject.Mock_Data.Tests;

public class StudentMockTests
{
    [Fact]
    public void Students_ShouldNotBeNull()
    {
        // Act & Assert
        Assert.NotNull(StudentMock.students);
    }

    [Fact]
    public void Students_ShouldContainFiveStudents()
    {
        // Act
        var count = StudentMock.students.Count();

        // Assert
        Assert.True(count >= 5);
    }

    [Fact]
    public void Students_ShouldHaveValidIds()
    {
        // Act & Assert
        foreach (var student in StudentMock.students)
        {
            Assert.NotEqual(Guid.Empty, student.Id);
        }
    }

    [Fact]
    public void Students_ShouldHaveValidNames()
    {
        // Act & Assert
        foreach (var student in StudentMock.students)
        {
            Assert.NotNull(student.Name);
            Assert.NotEmpty(student.Name);
        }
    }

    [Fact]
    public void Students_ShouldHaveAgeInValidRange()
    {
        // Act & Assert
        foreach (var student in StudentMock.students)
        {
            Assert.InRange(student.Age, 18, 60);
        }
    }

    [Fact]
    public void Students_ShouldBeEnumerable()
    {
        // Act
        var enumerable = StudentMock.students as IEnumerable<Student>;

        // Assert
        Assert.NotNull(enumerable);
        Assert.IsAssignableFrom<IEnumerable<Student>>(StudentMock.students);
    }

    [Fact]
    public void Students_ShouldHaveUniqueIds()
    {
        // Act
        var ids = StudentMock.students.Select(s => s.Id).ToList();
        var uniqueIds = ids.Distinct().ToList();

        // Assert
        Assert.Equal(ids.Count, uniqueIds.Count);
    }
}
