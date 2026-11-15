using ImpactFlow.Application.Atlas;
using ImpactFlow.Application.Atlas.Interfaces;
using ImpactFlow.Core.Atlas;

namespace ImpactFlow.Application.Tests
{
    public class CreateAtlasHandlerTests
    {
        [Fact]
        public async Task HandleAsync_CreatesAndSavesMapWithFirstDraft()
        {
            var store = new RecordingAtlasStore();
            var handler = new CreateAtlasHandler(store);

            var result = await handler.HandleAsync("Obsługa zamówień", CancellationToken.None);

            Assert.Equal("Obsługa zamówień", result.Map.Name);
            Assert.Equal(result.Map.Id, result.DraftVersion.MapId);
            Assert.Equal(1, result.DraftVersion.Number);
            Assert.Equal(AtlasVersionStatus.Draft, result.DraftVersion.Status);
            Assert.Empty(result.DraftVersion.Nodes);
            Assert.Empty(result.DraftVersion.Edges);
            Assert.Same(result.Map, store.SavedMap);
            Assert.Same(result.DraftVersion, store.SavedDraft);
        }

        private sealed class RecordingAtlasStore : IAtlasStore
        {
            public AtlasMap? SavedMap { get; private set; }
            public AtlasVersion? SavedDraft { get; private set; }

            public Task AddAsync(AtlasMap map, AtlasVersion initialDraft, CancellationToken cancellationToken)
            {
                SavedMap = map;
                SavedDraft = initialDraft;
                return Task.CompletedTask;
            }
        }
    }
}