# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only solution and project files to restore dependencies
COPY BankDirectoryAPI.sln ./
COPY BankDirectoryAPI/BankDirectoryApi.API.csproj BankDirectoryAPI/
COPY BankDirectoryApi.Application/BankDirectoryApi.Application.csproj BankDirectoryApi.Application/
COPY BankDirectoryApi.Domain/BankDirectoryApi.Domain.csproj BankDirectoryApi.Domain/
COPY BankDirectoryApi.Infrastructure/BankDirectoryApi.Infrastructure.csproj BankDirectoryApi.Infrastructure/
COPY BankDirectoryApi.Common/BankDirectoryApi.Common.csproj BankDirectoryApi.Common/
COPY BankDirectoryApi.UnitTests/BankDirectoryApi.UnitTests.csproj BankDirectoryApi.UnitTests/
COPY BankDirectoryApi.IntegrationTestss/BankDirectoryApi.IntegrationTestss.csproj BankDirectoryApi.IntegrationTestss/

# Restore NuGet packages (cached if project files haven't changed)
RUN dotnet restore "BankDirectoryAPI.sln"

# Copy the rest of the source code
COPY BankDirectoryAPI ./BankDirectoryAPI
COPY BankDirectoryApi.Application ./BankDirectoryApi.Application
COPY BankDirectoryApi.Domain ./BankDirectoryApi.Domain
COPY BankDirectoryApi.Infrastructure ./BankDirectoryApi.Infrastructure
COPY BankDirectoryApi.Common ./BankDirectoryApi.Common
COPY BankDirectoryApi.UnitTests ./BankDirectoryApi.UnitTests
COPY BankDirectoryApi.IntegrationTestss ./BankDirectoryApi.IntegrationTestss

# Build the application in Release mode (faster and optimized) or Debug
RUN dotnet publish "BankDirectoryAPI/BankDirectoryApi.API.csproj" -c Release -o /out

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /out .

# Set ASP.NET Core to listen on all interfaces for Docker access
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

# Expose the HTTP port
EXPOSE 5000

# Run the application
EXPOSE 5000
ENTRYPOINT ["dotnet", "BankDirectoryApi.API.dll"]

#docker run -d -p 5000:5000 --name bankapi -e ASPNETCORE_URLS=http://0.0.0.0:5000 bankdirectoryapi
#docker build -t bankdirectoryapi .