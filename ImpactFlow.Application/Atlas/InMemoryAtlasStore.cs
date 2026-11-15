using ImpactFlow.Application.Atlas.Interfaces;
using ImpactFlow.Core.Atlas;
using System.Collections.Concurrent;

namespace ImpactFlow.Application.Atlas
{
    public class InMemoryAtlasStore : IAtlasStore
    {
        private readonly ConcurrentDictionary<Guid, (AtlasMap Map, AtlasVersion Draft)> _maps = new();
        public Task AddAsync(AtlasMap map, AtlasVersion initialDraft, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(!_maps.TryAdd(map.Id, (map, initialDraft)))
                throw new InvalidOperationException($"Mapa o identyfikatorze {map.Id} już istnieje.");

            return Task.CompletedTask;
        }
    }
}
