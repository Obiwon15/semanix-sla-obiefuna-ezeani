FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FormsService/FormsService.csproj", "src/FormsService/"]
RUN dotnet restore "src/FormsService/FormsService.csproj"
COPY . .
WORKDIR "/src/src/FormsService"
RUN dotnet build "FormsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FormsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FormsService.dll"]