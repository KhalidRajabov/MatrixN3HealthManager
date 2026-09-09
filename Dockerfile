FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["MatrixN3HealthManager.sln", "."]
COPY ["N3HealthManager/MatrixN3HealthManager.csproj", "N3HealthManager/"]

RUN dotnet restore "MatrixN3HealthManager.sln"

COPY . .

RUN dotnet publish "N3HealthManager/MatrixN3HealthManager.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Install New Relic .NET Agent
RUN apt-get update \
    && apt-get install -y curl ca-certificates gnupg \
    && mkdir -p /etc/apt/keyrings \
    && echo 'deb [signed-by=/etc/apt/keyrings/newrelic-apt.gpg] http://apt.newrelic.com/debian/ newrelic non-free' \
        > /etc/apt/sources.list.d/newrelic.list \
    && curl -fsSL https://download.newrelic.com/NEWRELIC_APT_2DAD550E.public \
        | gpg --dearmor > /etc/apt/keyrings/newrelic-apt.gpg \
    && apt-get update \
    && apt-get install -y newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*

# Enable New Relic profiler
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER="{36032161-FFC0-4B61-B559-F6C5D41BAE5A}" \
    CORECLR_NEWRELIC_HOME="/usr/local/newrelic-dotnet-agent" \
    CORECLR_PROFILER_PATH="/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so"

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MatrixN3HealthManager.dll"]