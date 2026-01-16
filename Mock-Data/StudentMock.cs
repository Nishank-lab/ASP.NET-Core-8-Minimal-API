using MinimalAPIProject.Model;

namespace MinimalAPIProject.Mock_Data;
public class StudentMock
{
    // WARNING: In-memory storage - not suitable for containerized production environments
    // Data will be lost on container restart/redeploy. For production, replace with external database
    // Configure connection via environment variable: ConnectionStrings__DefaultConnection
    // Example: "Server=${DB_HOST};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};"
    public static IEnumerable<Student> students = new List<Student>(){
        new Student() { Name = "Test 1", Age = new Random().Next(18, 60), Id = Guid.NewGuid() },
        new Student() { Name = "Test 2", Age = new Random().Next(18, 60), Id = Guid.NewGuid() },
        new Student() { Name = "Test 3", Age = new Random().Next(18, 60), Id = Guid.NewGuid() },
        new Student() { Name = "Test 4", Age = new Random().Next(18, 60), Id = Guid.NewGuid() },
        new Student() { Name = "Test 5", Age = new Random().Next(18, 60), Id = Guid.NewGuid() }
    };
}