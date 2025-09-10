# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyDotNetProject.csproj", "."]
RUN dotnet restore "./MyDotNetProject.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MyDotNetProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyDotNetProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyDotNetProject.dll"]
