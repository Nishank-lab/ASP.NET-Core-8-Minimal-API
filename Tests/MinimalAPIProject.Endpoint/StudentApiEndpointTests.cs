using Xunit;
using Microsoft.Extensions.DependencyInjection;
using MinimalAPIProject.Endpoint;
using MinimalAPIProject.Service;
using MinimalAPIProject.Repository;

namespace MinimalAPIProject.Tests.Endpoint;

public class StudentApiEndpointTests
{
    [Fact]
    public void AddStudentApi_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddStudentApi();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(services, s => s.ServiceType == typeof(TimeProvider));
        Assert.Contains(services, s => s.ServiceType == typeof(IStudentRepository));
        Assert.Contains(services, s => s.ServiceType == typeof(IStudentService));
    }

    [Fact]
    public void AddStudentApi_ShouldRegisterTimeProviderAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddStudentApi();

        // Assert
        var timeProviderDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TimeProvider));
        Assert.NotNull(timeProviderDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, timeProviderDescriptor.Lifetime);
    }

    [Fact]
    public void AddStudentApi_ShouldRegisterStudentRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddStudentApi();

        // Assert
        var repositoryDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IStudentRepository));
        Assert.NotNull(repositoryDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, repositoryDescriptor.Lifetime);
        Assert.Equal(typeof(StudentRepository), repositoryDescriptor.ImplementationType);
    }

    [Fact]
    public void AddStudentApi_ShouldRegisterStudentServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddStudentApi();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IStudentService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(StudentService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddStudentApi_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddStudentApi();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddStudentApi_ShouldAllowChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var result = services.AddStudentApi().AddStudentApi();
        Assert.NotNull(result);
    }

    [Fact]
    public void AddStudentApi_ShouldRegisterExactlyThreeServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddStudentApi();

        // Assert
        Assert.Equal(3, services.Count);
    }

    [Fact]
    public void AddStudentApi_ShouldRegisterServicesInCorrectOrder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddStudentApi();

        // Assert
        Assert.Equal(typeof(TimeProvider), services[0].ServiceType);
        Assert.Equal(typeof(IStudentRepository), services[1].ServiceType);
        Assert.Equal(typeof(IStudentService), services[2].ServiceType);
    }
}
