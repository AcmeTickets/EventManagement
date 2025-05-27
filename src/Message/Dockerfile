FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Domains/EventManagement/Message/Message.csproj", "src/Domains/EventManagement/Message/"]
COPY ["src/Domains/EventManagement/Shared/Shared.csproj", "src/Domains/EventManagement/Shared/"]
RUN dotnet restore "src/Domains/EventManagement/Message/Message.csproj"
COPY src/Domains/EventManagement/Message/. /src/Domains/EventManagement/Message/
COPY src/Domains/EventManagement/Shared/. /src/Domains/EventManagement/Shared/  
WORKDIR /src/Domains/EventManagement/Message
RUN ls -l /src/Domains/EventManagement/Message
RUN dotnet build "Message.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Message.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Message.dll"]
