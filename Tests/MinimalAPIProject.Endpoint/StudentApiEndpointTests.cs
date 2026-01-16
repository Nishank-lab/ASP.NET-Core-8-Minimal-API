using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MinimalAPIProject.Endpoint;
using MinimalAPIProject.Service;
using MinimalAPIProject.Repository;
using MinimalAPIProject.Dto.Request;
using MinimalAPIProject.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MinimalAPIProject.Endpoint.Tests;

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
    public void AddStudentApi_ShouldRegisterRepositoryAsScoped()
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
    public void AddStudentApi_ShouldRegisterServiceAsScoped()
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
    public void MapStudentApiRoutes_ShouldReturnIEndpointRouteBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act
        var result = app.MapStudentApiRoutes();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEndpointRouteBuilder>(result);
    }

    [Fact]
    public void MapStudentApiRoutes_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act
        var result = app.MapStudentApiRoutes();

        // Assert
        Assert.Same(app, result);
    }
}
