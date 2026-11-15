namespace ImpactFlow.Api.Contracts.Atlas
{
    public sealed record CreateAtlasResponse(Guid MapId, string Name, Guid DraftVersionId);
}
