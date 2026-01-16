# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy project file and restore dependencies (layer caching optimization)
COPY MinimalAPIProject.csproj ./
RUN dotnet restore "MinimalAPIProject.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "MinimalAPIProject.csproj" -c Release -o /app/build

# Publish application
RUN dotnet publish "MinimalAPIProject.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application from builder stage
COPY --from=builder /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Configure ASP.NET Core environment
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose application port
EXPOSE 8080

# Set entry point
ENTRYPOINT ["dotnet", "MinimalAPIProject.dll"]