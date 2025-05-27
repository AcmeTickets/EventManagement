FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Message/Message.csproj", "src/Message/"]
COPY ["src/InternalContracts/InternalContracts.csproj", "src/InternalContracts/"]
RUN dotnet restore "src/Message/Message.csproj"
COPY src/Message/. /src/Message/
COPY src/InternalContracts/. /src/InternalContracts/
WORKDIR /src/Message
RUN ls -l /src/Message
RUN dotnet build "Message.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Message.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Message.dll"]
