using ImpactFlow.Core.Atlas;
using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Core.Tests.ImpactAnalysis
{
    public class ImpactAnalyzerTests
    {
        [Fact]
        public void Analyze_ReturnsSourceImpact_WhenGraphContainsOnlyObservedNode()
        {
            // Arrange — graf zawiera wyłącznie źródło problemu, bez połączeń.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var graph = new AtlasGraph(new[] { sourceNode }, Array.Empty<DependencyEdge>());

            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));

            // Siła problemu jest częścią obserwacji: 1m oznacza całkowitą awarię.
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var analyzer = new ImpactAnalyzer();

            // Act
            var impacts = analyzer.Analyze(graph, observation);

            // Assert — oczekujemy dokładnie jednego wyniku dotyczącego źródła.
            var sourceImpact = Assert.Single(impacts);

            Assert.Equal(sourceNode.Id, sourceImpact.NodeId);
            Assert.Equal(1m, sourceImpact.ImpactRatio);
            Assert.Equal(0m, sourceImpact.RemainingCapacityRatio);
            Assert.Equal(observedAt, sourceImpact.EffectiveAt);
            Assert.Equal(new[] { sourceNode.Id }, sourceImpact.Path);
        }

        [Fact]
        public void Analyze_ThrowsException_WhenObservationSeverityIsUnknown()
        {
            // Arrange — węzeł istnieje, ale nie mamy danych o jego stanie ani sile problemu.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var graph = new AtlasGraph(new[] { sourceNode }, Array.Empty<DependencyEdge>());
            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Unknown, observedAt, null);
            var analyzer = new ImpactAnalyzer();

            // Act + Assert — brak danych musi zatrzymać obliczenia.
            var exception = Assert.Throws<ArgumentException>(() => analyzer.Analyze(graph, observation));

            Assert.Equal("observation", exception.ParamName);
        }
        [Fact]
        public void Analyze_ReturnsTargetImpact_WhenSourceHasOutgoingEdge()
        {
            // Arrange — źródło ma problem 40%, a krawędź przenosi 90% jego wpływu.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var targetNode = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            var edge = new DependencyEdge(Guid.NewGuid(), sourceNode.Id, targetNode.Id, DependencyMode.Required, 0.9m, TimeSpan.FromMinutes(1));
            var graph = new AtlasGraph(new[] { sourceNode, targetNode }, new[] { edge });
            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Degraded, observedAt, 0.4m);
            var analyzer = new ImpactAnalyzer();

            // Act
            var impacts = analyzer.Analyze(graph, observation);

            // Assert — lista zawiera źródło oraz bezpośrednio dotknięty węzeł.
            Assert.Equal(2, impacts.Count);
            var targetImpact = Assert.Single(impacts, impact => impact.NodeId == targetNode.Id);

            Assert.Equal(0.36m, targetImpact.ImpactRatio);
            Assert.Equal(0.64m, targetImpact.RemainingCapacityRatio);
            Assert.Equal(observedAt.AddMinutes(1), targetImpact.EffectiveAt);
            Assert.Equal(new[] { sourceNode.Id, targetNode.Id }, targetImpact.Path);
        }
    }
}