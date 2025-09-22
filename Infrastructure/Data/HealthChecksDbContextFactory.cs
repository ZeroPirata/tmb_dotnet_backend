using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using HealthChecks.UI.Data;

namespace TMB.Challenge.Infrastructure.Data
{
    public class HealthChecksDbContextFactory : IDesignTimeDbContextFactory<HealthChecksDb>
    {
        public HealthChecksDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HealthChecksDb>();
            optionsBuilder.UseNpgsql("Host=postgres;Database=tmb_challenge;Username=postgres;Password=example");

            return new HealthChecksDb(optionsBuilder.Options);
        }
    }
}
