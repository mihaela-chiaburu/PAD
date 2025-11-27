FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Path-urile sunt relative la SmartProxyInstance2/ (build context)
COPY ["MovieAPI/MovieAPI.csproj", "MovieAPI/"]
COPY ["Common/Common.csproj", "Common/"]

RUN dotnet restore "MovieAPI/MovieAPI.csproj"

COPY ["MovieAPI/", "MovieAPI/"]
COPY ["Common/", "Common/"]

WORKDIR "/src/MovieAPI"
RUN dotnet build "MovieAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieAPI.dll"]