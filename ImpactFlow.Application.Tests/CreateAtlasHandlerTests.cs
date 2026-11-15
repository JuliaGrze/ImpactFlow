using ImpactFlow.Application.Atlas;
using ImpactFlow.Core.Atlas;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImpactFlow.Application.Tests
{
    public class CreateAtlasHandlerTests
    {
        [Fact]
        public void Handle_CreatesMapWithFirstDraft()
        {
            var handler = new CreateAtlasHandler();

            var result = handler.Handle("Obsługa zamówień");

            Assert.Equal("Obsługa zamówień", result.Map.Name);
            Assert.Equal(result.Map.Id, result.DraftVersion.MapId);
            Assert.Equal(1, result.DraftVersion.Number);
            Assert.Equal(AtlasVersionStatus.Draft, result.DraftVersion.Status);
            Assert.Empty(result.DraftVersion.Nodes);
            Assert.Empty(result.DraftVersion.Edges);
        }
    }
}
