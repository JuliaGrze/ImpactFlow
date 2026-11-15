using ImpactFlow.Core.Atlas;
using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Core.Tests.Atlas
{
    public class AtlasVersionTests
    {
        [Fact]
        public void Constructor_CreatesEmptyDraft()
        {
            var mapId = Guid.NewGuid();
            var version = new AtlasVersion(Guid.NewGuid(), mapId, 1);

            Assert.Equal(mapId, version.MapId);
            Assert.Equal(1, version.Number);
            Assert.Equal(AtlasVersionStatus.Draft, version.Status);
            Assert.Empty(version.Nodes);
        }

        [Fact]
        public void AddNode_AddsNodeToDraft()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            var node = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);

            version.AddNode(node);

            Assert.Same(node, Assert.Single(version.Nodes));
        }

        [Fact]
        public void AddNode_RejectsDuplicateId()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            var node = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            version.AddNode(node);

            // Inna nazwa nie pozwala dodać drugiego węzła z tym samym Id.
            var duplicate = new AtlasNode(node.Id, "Inna usługa", NodeType.ExternalService, 3);

            Assert.Throws<ArgumentException>(() => version.AddNode(duplicate));
            Assert.Same(node, Assert.Single(version.Nodes));
        }

        [Fact]
        public void AddNode_RejectsNull()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);

            Assert.Throws<ArgumentNullException>(() => version.AddNode(null!));
            Assert.Empty(version.Nodes);
        }

        [Fact]
        public void AddEdge_AddsConnection_WhenBothNodesBelongToVersion()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            var source = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var target = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            version.AddNode(source);
            version.AddNode(target);

            var edge = new DependencyEdge(Guid.NewGuid(), source.Id, target.Id, DependencyMode.Required, 0.9m, TimeSpan.FromMinutes(1));
            version.AddEdge(edge);

            Assert.Same(edge, Assert.Single(version.Edges));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddEdge_RejectsConnection_WhenOneNodeIsOutsideVersion(bool addOnlySource)
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            var source = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var target = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);

            // W wersji zapisujemy tylko jeden koniec połączenia
            version.AddNode(addOnlySource ? source : target);
            var edge = new DependencyEdge(Guid.NewGuid(), source.Id, target.Id, DependencyMode.Required, 0.9m, TimeSpan.FromMinutes(1));

            var exception = Assert.Throws<ArgumentException>(() => version.AddEdge(edge));

            Assert.Equal("edge", exception.ParamName);
            Assert.Empty(version.Edges);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddEdge_RejectsDuplicateConnection(bool reuseId)
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            var source = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var target = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            version.AddNode(source);
            version.AddNode(target);

            var firstEdge = new DependencyEdge(Guid.NewGuid(), source.Id, target.Id, DependencyMode.Required, 0.9m, TimeSpan.Zero);
            version.AddEdge(firstEdge);

            // Naruszamy tylko jedną regułe naraz: powtarzamy Id albo kierunek połącznenia
            var duplicate = reuseId
                ? new DependencyEdge(firstEdge.Id, target.Id, source.Id, DependencyMode.Required, 0.9m, TimeSpan.Zero)
                : new DependencyEdge(Guid.NewGuid(), source.Id, target.Id, DependencyMode.Required, 0.9m, TimeSpan.Zero);

            Assert.Throws<ArgumentException>(() => version.AddEdge(duplicate));
            Assert.Same(firstEdge, Assert.Single(version.Edges));
        }

        [Fact]
        public void Publish_ChangesStatusToPublished_WhenVersionHasNode()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            version.AddNode(new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5));

            version.Publish();

            Assert.Equal(AtlasVersionStatus.Published, version.Status);
        }

        [Fact]
        public void Publish_RejectsEmptyVersion()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);

            Assert.Throws<InvalidOperationException>(() => version.Publish());
            Assert.Equal(AtlasVersionStatus.Draft, version.Status);
        }

        [Fact]
        public void PublishedVersion_RejectsNewNodesAndEdges()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            var source = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var target = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            version.AddNode(source);
            version.AddNode(target);
            version.Publish();

            // Oba końce są w wersji, więc jedynym powodem odrzucenia jest publikacja.
            var edge = new DependencyEdge(Guid.NewGuid(), source.Id, target.Id, DependencyMode.Required, 0.9m, TimeSpan.Zero);
            var newNode = new AtlasNode(Guid.NewGuid(), "Zapasowe API", NodeType.ExternalService, 4);

            Assert.Throws<InvalidOperationException>(() => version.AddNode(newNode));
            Assert.Throws<InvalidOperationException>(() => version.AddEdge(edge));
            Assert.Equal(2, version.Nodes.Count);
            Assert.Empty(version.Edges);
        }

        [Fact]
        public void CreateAnalysisGraph_RejectsDraft()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            version.AddNode(new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5));

            Assert.Throws<InvalidOperationException>(() => version.CreateAnalysisGraph());
        }

        [Fact]
        public void CreateAnalysisGraph_UsesNodesAndEdgesFromPublishedVersion()
        {
            var version = new AtlasVersion(Guid.NewGuid(), Guid.NewGuid(), 1);
            var source = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var target = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            version.AddNode(source);
            version.AddNode(target);
            version.AddEdge(new DependencyEdge(Guid.NewGuid(), source.Id, target.Id, DependencyMode.Required, 0.9m, TimeSpan.Zero));
            version.Publish();

            var observedAt = new DateTimeOffset(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);
            var observation = new NodeObservation(source.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var graph = version.CreateAnalysisGraph();
            var impacts = new ImpactAnalyzer().Analyze(graph, observation);

            var targetImpact = Assert.Single(impacts, impact => impact.NodeId == target.Id);
            Assert.Equal(0.9m, targetImpact.ImpactRatio);
            Assert.Equal(new[] { source.Id, target.Id }, targetImpact.Path);
        }

    }
}
