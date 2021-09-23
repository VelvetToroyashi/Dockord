﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Docker.Discord/Docker.Discord.csproj", "Docker.Discord/"]
RUN dotnet restore "Docker.Discord/Docker.Discord.csproj"
COPY . .
WORKDIR "/src/Docker.Discord"
RUN dotnet build "Docker.Discord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Docker.Discord.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker.Discord.dll"]