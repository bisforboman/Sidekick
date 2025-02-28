using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonDatabase(this IServiceCollection services, string databasePath)
    {
        services.AddDbContextPool<SidekickDbContext>(o => o.UseSqlite("Data Source=" + databasePath)
            .UseLoggerFactory(new LoggerFactory()));

        return services;
    }
}
