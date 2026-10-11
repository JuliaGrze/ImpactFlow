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

        [Fact]
        public void Analyze_PropagatesImpactThroughEntireChain()
        {
            // Arrange — dwie kolejne zależności osłabiają wpływ i dodają opóźnienia.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var authorization = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            var orderHandling = new AtlasNode(Guid.NewGuid(), "Obsługa zamówienia", NodeType.BusinessProcess, 5);

            var paymentEdge = new DependencyEdge(Guid.NewGuid(), sourceNode.Id, authorization.Id, DependencyMode.Required, 0.9m, TimeSpan.FromMinutes(1));
            var orderEdge = new DependencyEdge(Guid.NewGuid(), authorization.Id, orderHandling.Id, DependencyMode.Required, 0.8m, TimeSpan.FromMinutes(5));
            var graph = new AtlasGraph(new[] { sourceNode, authorization, orderHandling }, new[] { paymentEdge, orderEdge });

            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var analyzer = new ImpactAnalyzer();

            // Act
            var impacts = analyzer.Analyze(graph, observation);

            // Assert — ostatni węzeł otrzymuje wpływ przez oba połączenia.
            Assert.Equal(3, impacts.Count);
            var orderImpact = Assert.Single(impacts, impact => impact.NodeId == orderHandling.Id);

            Assert.Equal(0.72m, orderImpact.ImpactRatio);
            Assert.Equal(0.28m, orderImpact.RemainingCapacityRatio);
            Assert.Equal(observedAt.AddMinutes(6), orderImpact.EffectiveAt);
            Assert.Equal(new[] { sourceNode.Id, authorization.Id, orderHandling.Id }, orderImpact.Path);
        }

        [Fact]
        public void Analyze_DoesNotRevisitNode_WhenGraphContainsCycle()
        {
            // Arrange — drugie połączenie prowadzi z powrotem do źródła.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var authorization = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);

            var forwardEdge = new DependencyEdge(Guid.NewGuid(), sourceNode.Id, authorization.Id, DependencyMode.Required, 0.9m, TimeSpan.FromMinutes(1));
            var returnEdge = new DependencyEdge(Guid.NewGuid(), authorization.Id, sourceNode.Id, DependencyMode.Required, 1m, TimeSpan.FromMinutes(1));
            var graph = new AtlasGraph(new[] { sourceNode, authorization }, new[] { forwardEdge, returnEdge });

            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var analyzer = new ImpactAnalyzer();

            // Act
            var impacts = analyzer.Analyze(graph, observation);

            // Arrange 
            Assert.Equal(2, impacts.Count);

            var sourceImpact = Assert.Single(impacts, impact => impact.NodeId == sourceNode.Id);
            var targetImpact = Assert.Single(impacts, impact => impact.NodeId == authorization.Id);

            Assert.Equal(1m, sourceImpact.ImpactRatio);
            Assert.Equal(new[] { sourceNode.Id }, sourceImpact.Path);
            Assert.Equal(0.9m, targetImpact.ImpactRatio);
            Assert.Equal(new[] { sourceNode.Id, authorization.Id }, targetImpact.Path);
        }

        [Fact]
        public void Analyze_ReturnsStrongestImpact_WhenMultiplePathsReachSameNode()
        {
            // Arrange — bezpośrednia droga daje 40%, a droga przez autoryzację daje 72%.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var authorization = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            var orderHandling = new AtlasNode(Guid.NewGuid(), "Obsługa zamówienia", NodeType.BusinessProcess, 5);

            var directEdge = new DependencyEdge(Guid.NewGuid(), sourceNode.Id, orderHandling.Id, DependencyMode.Required, 0.4m, TimeSpan.FromMinutes(1));
            var paymentEdge = new DependencyEdge(Guid.NewGuid(), sourceNode.Id, authorization.Id, DependencyMode.Required, 0.9m, TimeSpan.FromMinutes(1));
            var orderEdge = new DependencyEdge(Guid.NewGuid(), authorization.Id, orderHandling.Id, DependencyMode.Required, 0.8m, TimeSpan.FromMinutes(5));
            var graph = new AtlasGraph(new[] { sourceNode, authorization, orderHandling }, new[] { directEdge, paymentEdge, orderEdge });

            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var analyzer = new ImpactAnalyzer();

            // Act
            var impacts = analyzer.Analyze(graph, observation);

            // Assert — każdy węzeł ma jeden wynik; dla zamówień wygrywa droga przez autoryzację.
            Assert.Equal(3, impacts.Count);
            var orderImpact = Assert.Single(impacts, impact => impact.NodeId == orderHandling.Id);

            Assert.Equal(0.72m, orderImpact.ImpactRatio);
            Assert.Equal(observedAt.AddMinutes(6), orderImpact.EffectiveAt);
            Assert.Equal(new[] { sourceNode.Id, authorization.Id, orderHandling.Id }, orderImpact.Path);
        }

        [Fact]
        public void Analyze_ReturnsEarlierImpact_WhenPathsHaveEqualImpact()
        {
            // Arrange — obie drogi dają 50%, ale bezpośrednia dociera później.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var authorization = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);
            var orderHandling = new AtlasNode(Guid.NewGuid(), "Obsługa zamówienia", NodeType.BusinessProcess, 5);

            var directEdge = new DependencyEdge(Guid.NewGuid(), sourceNode.Id, orderHandling.Id, DependencyMode.Required, 0.5m, TimeSpan.FromMinutes(10));
            var paymentEdge = new DependencyEdge(Guid.NewGuid(), sourceNode.Id, authorization.Id, DependencyMode.Required, 1m, TimeSpan.FromMinutes(1));
            var orderEdge = new DependencyEdge(Guid.NewGuid(), authorization.Id, orderHandling.Id, DependencyMode.Required, 0.5m, TimeSpan.FromMinutes(2));
            var graph = new AtlasGraph(new[] { sourceNode, authorization, orderHandling }, new[] { directEdge, paymentEdge, orderEdge });

            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var analyzer = new ImpactAnalyzer();

            // Act
            var impacts = analyzer.Analyze(graph, observation);

            // Assert — przy równym wpływie wygrywa droga docierająca po 3 minutach.
            Assert.Equal(3, impacts.Count);
            var orderImpact = Assert.Single(impacts, impact => impact.NodeId == orderHandling.Id);

            Assert.Equal(0.5m, orderImpact.ImpactRatio);
            Assert.Equal(observedAt.AddMinutes(3), orderImpact.EffectiveAt);
            Assert.Equal(new[] { sourceNode.Id, authorization.Id, orderHandling.Id }, orderImpact.Path);
        }

        [Fact]
        public void Analyze_ThrowsException_WhenObservedNodeDoesNotExistInGraph()
        {
            // Arrange — obserwacja wskazuje inny identyfikator niż węzeł należący do grafu.
            var existingNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var graph = new AtlasGraph(new[] { existingNode }, Array.Empty<DependencyEdge>());
            var missingNodeId = Guid.NewGuid();
            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(missingNodeId, ObservedStatus.Unavailable, observedAt, 1m);
            var analyzer = new ImpactAnalyzer();

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => analyzer.Analyze(graph, observation));
        }

        [Fact]
        public void Analyze_ExcludesNode_WhenNoPathConnectsItToSource()
        {
            // Arrange — oba węzły należą do grafu, ale nie są połączone.
            var sourceNode = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var unrelatedNode = new AtlasNode(Guid.NewGuid(), "System kadrowy", NodeType.System, 2);
            var graph = new AtlasGraph(new[] { sourceNode, unrelatedNode }, Array.Empty<DependencyEdge>());
            var observedAt = new DateTimeOffset(2026, 8, 21, 12, 0, 0, TimeSpan.FromHours(2));
            var observation = new NodeObservation(sourceNode.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var analyzer = new ImpactAnalyzer();

            // Act
            var impacts = analyzer.Analyze(graph, observation);

            // Assert — wynik zawiera tylko źródło, ponieważ drugi węzeł nie jest z nim połączony.
            var sourceImpact = Assert.Single(impacts);
            Assert.Equal(sourceNode.Id, sourceImpact.NodeId);

        }

    }
}