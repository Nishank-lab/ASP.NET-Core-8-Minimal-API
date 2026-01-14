using Xunit;
using MinimalAPIProject.Mock_Data;
using MinimalAPIProject.Model;

namespace MinimalAPIProject.Tests.Mock_Data;

public class StudentMockTests
{
    [Fact]
    public void Students_ShouldNotBeNull()
    {
        // Act
        var students = StudentMock.students;

        // Assert
        Assert.NotNull(students);
    }

    [Fact]
    public void Students_ShouldContainFiveElements()
    {
        // Act
        var students = StudentMock.students;

        // Assert
        Assert.Equal(5, students.Count());
    }

    [Fact]
    public void Students_ShouldAllHaveNonEmptyIds()
    {
        // Act
        var students = StudentMock.students;

        // Assert
        Assert.All(students, student => Assert.NotEqual(Guid.Empty, student.Id));
    }

    [Fact]
    public void Students_ShouldAllHaveUniqueIds()
    {
        // Act
        var students = StudentMock.students;
        var ids = students.Select(s => s.Id).ToList();

        // Assert
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void Students_ShouldAllHaveNames()
    {
        // Act
        var students = StudentMock.students;

        // Assert
        Assert.All(students, student => Assert.False(string.IsNullOrWhiteSpace(student.Name)));
    }

    [Fact]
    public void Students_ShouldHaveNamesStartingWithTest()
    {
        // Act
        var students = StudentMock.students;

        // Assert
        Assert.All(students, student => Assert.StartsWith("Test", student.Name));
    }

    [Fact]
    public void Students_ShouldAllHaveAgeInRange()
    {
        // Act
        var students = StudentMock.students;

        // Assert
        Assert.All(students, student =>
        {
            Assert.True(student.Age >= 18);
            Assert.True(student.Age < 60);
        });
    }

    [Fact]
    public void Students_ShouldBeEnumerable()
    {
        // Act
        var students = StudentMock.students;

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Student>>(students);
    }

    [Fact]
    public void Students_FirstStudent_ShouldHaveNameTest1()
    {
        // Act
        var students = StudentMock.students;
        var firstStudent = students.First();

        // Assert
        Assert.Equal("Test 1", firstStudent.Name);
    }

    [Fact]
    public void Students_LastStudent_ShouldHaveNameTest5()
    {
        // Act
        var students = StudentMock.students;
        var lastStudent = students.Last();

        // Assert
        Assert.Equal("Test 5", lastStudent.Name);
    }

    [Fact]
    public void Students_ShouldBeReadable()
    {
        // Act & Assert
        Assert.NotNull(StudentMock.students);
        var count = 0;
        foreach (var student in StudentMock.students)
        {
            Assert.NotNull(student);
            count++;
        }
        Assert.Equal(5, count);
    }
}
