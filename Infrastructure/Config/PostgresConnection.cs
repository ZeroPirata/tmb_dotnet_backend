using Microsoft.Extensions.Configuration;


namespace TMB.Challenge.Infrastructure.Config;

public class PostgresConnection(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public string GetConnectionString()
    {
        string? port = _config["POSTGRES_PORT"];
        string? host = _config["POSTGRES_HOST"];
        string? user = _config["POSTGRES_USER"];
        string? password = _config["POSTGRES_PASSWORD"];
        string? database = _config["POSTGRES_DB"];
        return $"Host={host};Port={port};Username={user};Password={password};Database={database}";
    }
}