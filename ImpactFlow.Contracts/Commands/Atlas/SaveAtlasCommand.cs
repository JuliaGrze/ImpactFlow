using ImpactFlow.Core.Atlas;
using MediatR;

namespace ImpactFlow.Contracts.Commands.Atlas
{
    public sealed record SaveAtlasCommand(AtlasMap Map, AtlasVersion InitialDraft) : IRequest;
}
