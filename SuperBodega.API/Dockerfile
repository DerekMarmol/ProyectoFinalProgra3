FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SuperBodega.API/SuperBodega.API.csproj", "SuperBodega.API/"]
COPY ["SuperBodega.Core/SuperBodega.Core.csproj", "SuperBodega.Core/"]
COPY ["SuperBodega.Infrastructure/SuperBodega.Infrastructure.csproj", "SuperBodega.Infrastructure/"]
RUN dotnet restore "SuperBodega.API/SuperBodega.API.csproj"
COPY . .
WORKDIR "/src/SuperBodega.API"
RUN dotnet build "SuperBodega.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SuperBodega.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SuperBodega.API.dll"]