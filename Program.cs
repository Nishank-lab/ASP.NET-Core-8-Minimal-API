using MinimalAPIProject.Endpoint;

var builder = WebApplication.CreateBuilder(args);

// Add explicit environment variable support for containerization
builder.Configuration.AddEnvironmentVariables();

//add services and repositories
builder.Services.AddStudentApi();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks for container orchestration
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map health check endpoints for Kubernetes liveness and readiness probes
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

//all api's endpoints
app.MapStudentApiRoutes();


app.Run();