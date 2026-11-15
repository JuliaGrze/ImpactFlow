using ImpactFlow.Application.Atlas.Interfaces;
using ImpactFlow.Core.Atlas;

namespace ImpactFlow.Application.Atlas
{
    public sealed class CreateAtlasHandler
    {
        private readonly IAtlasStore _atlasStore;

        public CreateAtlasHandler(IAtlasStore atlasStore)
        {
            _atlasStore = atlasStore;
        }

        public async Task<(AtlasMap Map, AtlasVersion DraftVersion)> HandleAsync(string name, CancellationToken cancellationToken)
        {
            var map = new AtlasMap(Guid.NewGuid(), name);
            var draftVersion = new AtlasVersion(Guid.NewGuid(), map.Id, 1);

            await _atlasStore.AddAsync(map, draftVersion, cancellationToken);

            return(map, draftVersion);
        }
    }
}
