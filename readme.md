//
docker-compose run --rm ef-tools sh -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef migrations add MigracaoDbHealth --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj --context ApplicationDbContext"

docker-compose run --rm ef-tools sh -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef database update --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj --context ApplicationDbContext"

docker-compose run --rm ef-tools sh -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef dbcontext list --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj"
//

docker-compose down

docker-compose run --rm ef-tools sh -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef migrations add AddHealthChecksUIStorageDb --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj && /root/.dotnet/tools/dotnet-ef database update --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj"

docker-compose run --rm ef-tools sh -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef migrations add AddHealthChecksUIStorageDb --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj && /root/.dotnet/tools/dotnet-ef database update --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj"

docker-compose run --rm ef-tools sh -c "
export PATH=/root/.dotnet/tools:$PATH
dotnet tool install --global dotnet-ef --version 9.0.9
dotnet-ef migrations add AddHealthChecksUIStorageDb \
    --project Infrastructure/TMB.Challenge.Infrastructure.csproj \
    --startup-project API/TMB.Challenge.API.csproj \
    --context HealthChecksDb
dotnet-ef database update \
    --project Infrastructure/TMB.Challenge.Infrastructure.csproj \
    --startup-project API/TMB.Challenge.API.csproj \
    --context HealthChecksDb
"


docker-compose run --rm ef-tools sh -c "export PATH=`$PATH:/root/.dotnet/tools && dotnet-ef migrations add AddHealthChecksUIStorageDb --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj && dotnet-ef database update --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj"

docker-compose run --rm ef-tools sh -c "export PATH=/root/.dotnet/tools:$PATH && dotnet-ef migrations add AddHealthChecksUIStorageDb --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj && dotnet-ef database update --project Infrastructure/TMB.Challenge.Infrastructure.csproj --startup-project API/TMB.Challenge.API.csproj"


docker-compose up --build -d

docker-compose up -d --no-deps --build api

curl -X POST https://localhost:5001/api/orders -H "Content-Type: application/json" \ -d '{ "produto": "Notebook", "cliente": "Maria Silva", "valor": 3499.90}'
