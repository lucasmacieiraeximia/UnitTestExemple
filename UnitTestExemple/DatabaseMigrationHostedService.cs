using Microsoft.EntityFrameworkCore;
using UnitTestExemple.Data;

namespace UnitTestExemple;

public class DatabaseMigrationHostedService : IHostedService
{
    private readonly AppDbContext _dbContext;

    public DatabaseMigrationHostedService(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

}
