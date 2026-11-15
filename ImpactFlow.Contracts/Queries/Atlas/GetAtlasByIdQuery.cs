using ImpactFlow.Core.Atlas;
using MediatR;

namespace ImpactFlow.Contracts.Queries.Atlas
{
    public sealed record GetAtlasByIdQuery(Guid MapId) : IRequest<AtlasMap?>;
}