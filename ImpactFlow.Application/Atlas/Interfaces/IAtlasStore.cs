using ImpactFlow.Core.Atlas;

namespace ImpactFlow.Application.Atlas.Interfaces
{
    public interface IAtlasStore
    {
        /// <summary>
        /// Zapisuje nową mapę i jej pierwszą wersję roboczą jako jedną operację.
        /// </summary>
        Task AddAsync(AtlasMap map, AtlasVersion initialDraft, CancellationToken cancellationToken);
    }
}
