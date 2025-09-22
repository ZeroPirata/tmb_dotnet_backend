# Stage de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solução e todos os projetos
COPY ["TMB.Challenge.sln", "."]
COPY ["API/TMB.Challenge.API.csproj", "API/"]
COPY ["Worker/TMB.Challenge.Worker.csproj", "Worker/"]
COPY ["Application/TMB.Challenge.Application.csproj", "Application/"]
COPY ["Domain/TMB.Challenge.Domain.csproj", "Domain/"]
COPY ["Infrastructure/TMB.Challenge.Infrastructure.csproj", "Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "TMB.Challenge.sln"

# Copiar todo o código-fonte
COPY . .

# Build e publish do Worker
WORKDIR "/src/Worker"
RUN dotnet publish "TMB.Challenge.Worker.csproj" -c Release -o /app/publish

# Stage de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TMB.Challenge.Worker.dll"]
