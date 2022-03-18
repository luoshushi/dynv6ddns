#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:2.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /src
COPY ["dynv6ddns/dynv6ddns.csproj", "dynv6ddns/"]
RUN dotnet restore "dynv6ddns/dynv6ddns.csproj"
COPY . .
WORKDIR "/src/dynv6ddns"
RUN dotnet build "dynv6ddns.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dynv6ddns.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dynv6ddns.dll"]