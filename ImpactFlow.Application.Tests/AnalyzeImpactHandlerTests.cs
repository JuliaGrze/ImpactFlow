using ImpactFlow.Application.ImpactAnalysis.AnalyzeImpact;
using ImpactFlow.Core.Atlas;
using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Application.Tests
{
    public class AnalyzeImpactHandlerTests
    {
        [Fact]
        public void Handle_UsesVersionProvidedInInput()
        {
            // Arrange - dwie wersje jednej mapy mają te same węzły
            var mapId = Guid.NewGuid();
            var firstVersion = new AtlasVersion(Guid.NewGuid(), mapId, 1);
            var secondVersion = new AtlasVersion(Guid.NewGuid(), mapId, 2);

            var paymentApi = new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, 5);
            var authorization = new AtlasNode(Guid.NewGuid(), "Autoryzacja płatności", NodeType.ProcessStep, 5);

            firstVersion.AddNode(paymentApi);
            firstVersion.AddNode(authorization);
            firstVersion.Publish();

            secondVersion.AddNode(paymentApi);
            secondVersion.AddNode(authorization);
            secondVersion.AddEdge(new DependencyEdge(Guid.NewGuid(), paymentApi.Id, authorization.Id, DependencyMode.Required, 0.9m, TimeSpan.Zero));
            secondVersion.Publish();

            var observedAt = new DateTimeOffset(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);
            var observation = new NodeObservation(paymentApi.Id, ObservedStatus.Unavailable, observedAt, 1m);
            var handler = new AnalyzeImpactHandler(new ImpactAnalyzer());

            // Act - zmieniamy tylko wersję mapy, obserwacja pozostaje ta sama
            var firstResult = handler.Handle(new AnalyzeImpactInput(firstVersion, observation));
            var secondResult = handler.Handle(new AnalyzeImpactInput(secondVersion, observation));

            // Assert
            Assert.Equal(paymentApi.Id, Assert.Single(firstResult).NodeId);
            Assert.Equal(2, secondResult.Count);
            Assert.Single(secondResult, impact => impact.NodeId == authorization.Id);
        }
    }
}
