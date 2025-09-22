using Microsoft.Extensions.Configuration;


namespace TMB.Challenge.Infrastructure.Config;

/// <summary>
/// Classe para acessar as configurações do PostgreSQL.
/// </summary>
public class PostgresConnection(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    /// <summary>
    /// Gera a string de conexão com o banco de dados PostgreSQL usando as configurações fornecidas.
    /// </summary>
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