# inspired by https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-7.0
# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Directory.Build.props .
COPY src/IPinPoint.Api/*.csproj ./IPinPoint.Api/
RUN dotnet restore IPinPoint.Api/IPinPoint.Api.csproj

# copy everything else and build app
COPY src ./
RUN dotnet build IPinPoint.Api/IPinPoint.Api.csproj -c Release

RUN dotnet publish IPinPoint.Api/IPinPoint.Api.csproj --no-build -c Release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "IPinPoint.Api.dll"]