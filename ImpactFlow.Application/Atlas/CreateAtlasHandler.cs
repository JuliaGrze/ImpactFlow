using ImpactFlow.Core.Atlas;

namespace ImpactFlow.Application.Atlas
{
    public sealed class CreateAtlasHandler
    {
        public (AtlasMap Map, AtlasVersion DraftVersion) Handle(string name)
        {
            var map = new AtlasMap(Guid.NewGuid(), name);
            var draftVersion = new AtlasVersion(Guid.NewGuid(), map.Id, 1);

            return(map, draftVersion);
        }
    }
}
