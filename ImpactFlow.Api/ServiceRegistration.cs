using ImpactFlow.Application.Atlas;
using ImpactFlow.Application.Atlas.Interfaces;

namespace ImpactFlow.Api
{
    internal static class ServiceRegistration
    {
        public static IServiceCollection AddImpactFlowServices(this IServiceCollection services)
        {
            AddAtlasServices(services);

            return services;
        }

        private static void AddAtlasServices(IServiceCollection services)
        {
            services.AddSingleton<IAtlasStore, InMemoryAtlasStore>();
            services.AddTransient<CreateAtlasHandler>();
        }
    }
}