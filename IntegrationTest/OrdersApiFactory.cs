using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitTestExemple.Data;

namespace IntegrationTesting;

public class OrdersApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private const int ContainerPort = 3306;
    private const int HostPort = 3307;
    private const string ContainerPassword = "my-secret-pw";

    private readonly IContainer _mysqlContainer = new ContainerBuilder()
        .WithImage("mysql:latest")
        .WithEnvironment("MYSQL_ROOT_PASSWORD", ContainerPassword)
        .WithPortBinding(HostPort, ContainerPort)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(ContainerPort))
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));

            services.Remove(dbContextDescriptor!);
            services.Remove(dbContextOptionsDescriptor!);

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    $"server=localhost;port={HostPort};database=unittetstexample;uid=root;password={ContainerPassword}", 
                    new MySqlServerVersion(new Version(8, 0, 34)))
                );
        });
    }

    public async Task InitializeAsync()
    {
        await _mysqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mysqlContainer.StopAsync();
    }
}
