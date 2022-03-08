FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Docker.Discord.Remora/Docker.Discord.Remora.csproj", "Docker.Discord.Remora/"]
RUN dotnet restore "Docker.Discord.Remora/Docker.Discord.Remora.csproj"
COPY . .
WORKDIR "/src/Docker.Discord.Remora"
RUN dotnet build "Docker.Discord.Remora.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Docker.Discord.Remora.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker.Discord.Remora.dll"]
