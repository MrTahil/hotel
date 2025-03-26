# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Először másoljuk a HTML mappát
COPY Html/ ./Html/

# 2. Másoljuk a projekt fájlokat és végezzük el a restore-t
COPY *.csproj .
RUN dotnet restore

# 3. Másoljunk minden mást és buildeljük
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Környezeti változók
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:81
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV TZ=Europe/Budapest

# CA tanúsítványok és időzóna beállítás
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ca-certificates \
    openssl && \
    update-ca-certificates && \
    ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && \
    echo $TZ > /etc/timezone && \
    rm -rf /var/lib/apt/lists/*

# 4. Másoljuk a publikált alkalmazást ÉS a HTML mappát
COPY --from=build /app/publish .
COPY --from=build /src/Html ./Html

EXPOSE 81

ENTRYPOINT ["dotnet", "HMZ_rt.dll"]