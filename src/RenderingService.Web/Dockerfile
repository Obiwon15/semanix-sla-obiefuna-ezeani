FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RenderingService/RenderingService.csproj", "src/RenderingService/"]
RUN dotnet restore "src/RenderingService/RenderingService.csproj"
COPY . .
WORKDIR "/src/src/RenderingService"
RUN dotnet build "RenderingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RenderingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RenderingService.dll"]