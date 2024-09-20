# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the project file and restore the dependencies
COPY *.sln ./
COPY IMS.Presentation/*.csproj ./IMS.Presentation/
COPY IMS.Application/*.csproj ./IMS.Application/
COPY IMS.Core/*.csproj ./IMS.Core/
COPY IMS.Infrastructure/*.csproj ./IMS.Infrastructure/
COPY IMS.Tests/*.csproj ./IMS.Tests/

# Restore the dependencies
RUN dotnet restore 

# Copy the remaining files
COPY . .

# Build the project
RUN dotnet build

# Stage 2: Publish stage
FROM build AS publish
WORKDIR /src/IMS.Presentation
RUN dotnet publish -c Release -o /app

# Stage 3: Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app .
COPY ["web.config", "."]
ENTRYPOINT ["dotnet", "IMS.Presentation.dll"]
