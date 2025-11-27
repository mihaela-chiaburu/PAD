FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SyncNode/SyncNode.csproj", "SyncNode/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "SyncNode/SyncNode.csproj"

COPY ["SyncNode/", "SyncNode/"]
COPY ["Common/", "Common/"]

WORKDIR "/src/SyncNode"
RUN dotnet build "SyncNode.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SyncNode.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SyncNode.dll"]