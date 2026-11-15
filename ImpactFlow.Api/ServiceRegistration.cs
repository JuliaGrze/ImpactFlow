using ImpactFlow.Application.Atlas;
using ImpactFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ImpactFlow.Api
{
    internal static class ServiceRegistration
    {
        public static IServiceCollection AddImpactFlowServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            AddMessagingServices(services);
            AddAtlasServices(services);
            AddPersistenceServices(services, configuration);

            return services;
        }

        private static void AddMessagingServices(IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ImpactFlowDbContext).Assembly));
        }

        private static void AddAtlasServices(IServiceCollection services)
        {
            services.AddTransient<AtlasService>();
        }

        private static void AddPersistenceServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ImpactFlowDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("ImpactFlow")
                    ?? throw new InvalidOperationException("Brakuje konfiguracji połączenia z bazą ImpactFlow.");

                options.UseNpgsql(connectionString);
            });
        }
    }
}