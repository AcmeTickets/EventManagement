FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5271

ENV ASPNETCORE_URLS=http://+:5271

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Domains/EventManagement/Api/Api.csproj", "src/Domains/EventManagement/Api/"]
COPY ["src/Domains/EventManagement/Shared/Shared.csproj", "src/Domains/EventManagement/Shared/"]
RUN dotnet restore "src/Domains/EventManagement/Api/Api.csproj"
COPY src/Domains/EventManagement/Api/. /src/Domains/EventManagement/Api/
COPY src/Domains/EventManagement/Shared/. /src/Domains/EventManagement/Shared/
WORKDIR /src/Domains/EventManagement/Api
RUN ls -l /src/Domains/EventManagement/Api
RUN dotnet build "Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]