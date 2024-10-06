FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

COPY banbet.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build "banbet.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish 
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "banbet.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "banbet.dll"]
