# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/NotificationsAPI.Api/NotificationsAPI.Api.csproj", "NotificationsAPI.Api/"]
COPY ["src/NotificationsAPI.Application/NotificationsAPI.Application.csproj", "NotificationsAPI.Application/"]
COPY ["src/NotificationsAPI.Domain/NotificationsAPI.Domain.csproj", "NotificationsAPI.Domain/"]
COPY ["src/NotificationsAPI.Infrastructure/NotificationsAPI.Infrastructure.csproj", "NotificationsAPI.Infrastructure/"]

RUN dotnet restore "NotificationsAPI.Api/NotificationsAPI.Api.csproj"

# Copy all source code
COPY src/ .

# Build and publish
WORKDIR "/src/NotificationsAPI.Api"
RUN dotnet build "NotificationsAPI.Api.csproj" -c Release -o /app/build
RUN dotnet publish "NotificationsAPI.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "NotificationsAPI.Api.dll"]
