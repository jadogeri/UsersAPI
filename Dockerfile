# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy ONLY the project files first (Bypass the .slnx)
COPY ["UsersAPI/UsersAPI.csproj", "UsersAPI/"]
COPY ["UsersAPI.Tests/UsersAPI.Tests.csproj", "UsersAPI.Tests/"]

# 2. Restore the projects directly (This avoids .slnx parsing errors)
RUN dotnet restore "UsersAPI/UsersAPI.csproj"
RUN dotnet restore "UsersAPI.Tests/UsersAPI.Tests.csproj"

# 3. Now copy the rest of the source code
COPY . .

# 4. Build and Publish the API project
WORKDIR "/src/UsersAPI"
RUN dotnet publish "UsersAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Environment setup
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "UsersAPI.dll"]
