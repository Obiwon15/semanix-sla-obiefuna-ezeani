FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RenderingService.Web/RenderingService.Web.csproj", "RenderingService.Web/"]
COPY ["RenderingService.Application/RenderingService.Application.csproj", "RenderingService.Application/"]
COPY ["RenderingService.Domain/RenderingService.Domain.csproj", "RenderingService.Domain/"]
COPY ["RenderingService.Infrastructure/RenderingService.Infrastructure.csproj", "RenderingService.Infrastructure/"]
RUN dotnet restore "RenderingService.Web/RenderingService.Web.csproj"
COPY . .
WORKDIR "/src/RenderingService.Web"
RUN dotnet build "RenderingService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RenderingService.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RenderingService.Web.dll"]