# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HMZ_rt.csproj", "."]
RUN dotnet restore "HMZ_rt.csproj"
COPY . .
RUN dotnet build "HMZ_rt.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "HMZ_rt.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install dependencies
RUN apt-get update && \
    apt-get install -y openssl ca-certificates && \
    rm -rf /var/lib/apt/lists/*

# Install CA certificates
RUN apt-get update && \
    apt-get install -y --no-install-recommends openssl ca-certificates && \
    rm -rf /var/lib/apt/lists/*
# Copy and install GTS Root R4 certificate
COPY ["certs/GTSRootR4.der", "/usr/local/share/ca-certificates/GTSRootR4.crt"]
RUN chmod 644 /usr/local/share/ca-certificates/GTSRootR4.crt && \
    update-ca-certificates
# Application setup
RUN mkdir -p /app/certs
COPY --from=publish /app/publish .
COPY ["certs/api.hmzrt.eu_full.pfx", "/app/certs/"]
RUN chmod 644 /app/certs/api.hmzrt.eu_full.pfx
# Environment variables
ENV ASPNETCORE_URLS=https://+:443;http://+:81
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="kando"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certs/api.hmzrt.eu_full.pfx

# Ports and entrypoint
EXPOSE 81
EXPOSE 443
ENTRYPOINT ["dotnet", "HMZ_rt.dll"]