using ImpactFlow.Core.Atlas;
using System.Security.Cryptography;

namespace ImpactFlow.Core.ImpactAnalysis
{
    /// <summary>
    /// Oblicza wpływ problemu na węzły Atlasu.
    /// </summary>
    public sealed class ImpactAnalyzer
    {
        public IReadOnlyList<NodeImpact> Analyze(AtlasGraph graph, NodeObservation observation)
        {
            ArgumentNullException.ThrowIfNull(graph);
            ArgumentNullException.ThrowIfNull(observation);

            // Obserwacja musi dotyczyć węzła należącego do analizowanego grafu.
            graph.GetNode(observation.NodeId);

            // Brak danych o sile problemu uniemożliwia obliczenie wpływu.
            var severity = observation.Severity ?? throw new ArgumentException("Nie można obliczyć wpływu dla obserwacji o nieznanej sile problemu.", nameof(observation));

            var sourceImpact = new NodeImpact(observation.NodeId, severity, observation.ObservedAt, new[] { observation.NodeId });
            // Tworzymy listę wyników.Pierwszy wynik dotyczy źródła.
            var impacts = new List<NodeImpact> { sourceImpact };

            // Na tym etapie analizujemy wyłącznie bezpośrednich odbiorców wpływu źródła.
            // Pobieramy wszystkie połączenia wychodzące ze źródła. Dla każdego obliczamy wynik węzła docelowego.
            foreach (var edge in graph.GetOutgoingEdges(observation.NodeId)) 
            {
                // Siła problemu * waga połącznia
                var impactRatio = sourceImpact.ImpactRatio * edge.ImportanceWeight;
                // Opóźnienie
                var effectiveAt = sourceImpact.EffectiveAt + edge.PropagationDelay;
                // Droga
                var path = new[] { observation.NodeId, edge.TargetNodeId };

                impacts.Add(new NodeImpact(edge.TargetNodeId, impactRatio, effectiveAt, path));
            }
            return impacts.AsReadOnly();
        }
    }
}