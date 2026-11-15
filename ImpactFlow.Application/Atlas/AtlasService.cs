using ImpactFlow.Contracts.Commands.Atlas;
using ImpactFlow.Core.Atlas;
using MediatR;

namespace ImpactFlow.Application.Atlas
{
    public sealed class AtlasService
    {
        private readonly ISender _sender;

        public AtlasService(ISender sender)
        {
            _sender = sender;
        }

        public async Task<(AtlasMap Map, AtlasVersion DraftVersion)> CreateAsync(string name, CancellationToken cancellationToken)
        {
            var map = new AtlasMap(Guid.NewGuid(), name);
            var draftVersion = new AtlasVersion(Guid.NewGuid(), map.Id, 1);

            await _sender.Send(new SaveAtlasCommand(map, draftVersion), cancellationToken);

            return(map, draftVersion);
        }
    }
}
