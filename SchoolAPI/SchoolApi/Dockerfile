FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5039
ENV ASPNETCORE_ENVIRONMENT=Development

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["SchoolAPI/SchoolApi/SchoolApi.csproj", "SchoolAPI/SchoolApi/"]
RUN dotnet restore "SchoolAPI/SchoolApi/SchoolApi.csproj"
COPY . .
WORKDIR "/src/SchoolAPI/SchoolApi"
RUN dotnet build "SchoolApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "SchoolApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

EXPOSE 80
EXPOSE 5039
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchoolApi.dll"]
