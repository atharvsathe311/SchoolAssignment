FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5050
ENV ASPNETCORE_ENVIRONMENT=Development


USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["src/Users/UserAPI/UserAPI.csproj", "Users/UserAPI/"]
RUN dotnet restore "/src/Users/UserAPI/UserAPI.csproj"
COPY . .
WORKDIR "/src/src/Users/UserAPI"
RUN dotnet build "UserAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "UserAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

EXPOSE 5050

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserAPI.dll"]
