# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution dan project files dulu (untuk cache layer)
COPY MathService.slnx .
COPY src/MathService/MathService.csproj src/MathService/
COPY tests/MathService.Tests/MathService.Tests.csproj tests/MathService.Tests/
RUN dotnet restore src/MathService/MathService.csproj
# Copy semua kode lalu build
COPY . .
RUN dotnet publish src/MathService/MathService.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime (image jauh lebih kecil)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MathService.dll"]