FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# копируем решение и проекты для корректного restore
COPY ["MatrixN3HealthManager.sln", "."]
COPY ["N3HealthManager/MatrixN3HealthManager.csproj", "N3HealthManager/"]

RUN dotnet restore "MatrixN3HealthManager.sln"

# копируем оставшийся исходный код
COPY . .

RUN dotnet publish "N3HealthManager/MatrixN3HealthManager.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MatrixN3HealthManager.dll"]










