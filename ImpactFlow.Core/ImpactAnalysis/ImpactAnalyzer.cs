using ImpactFlow.Core.Atlas;

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

            // Kolejka wyników, z których trzeba jeszcze przenieść wpływ na następne węzły.
            var pendingImpacts = new Queue<NodeImpact>();
            pendingImpacts.Enqueue(sourceImpact); //dodanie elemntu na koniec kolejki

            // Wyjmujemy pierwszy wynik z kolejki. Pętla kończy się, gdy kolejka jest pusta.
            while (pendingImpacts.TryDequeue(out var currentImpact))
            {
                foreach (var edge in graph.GetOutgoingEdges(currentImpact.NodeId))
                {
                    // Pomijamy powrót do węzła już obecnego w tej ścieżce, np. A → B → A.
                    if (currentImpact.Path.Contains(edge.TargetNodeId)) 
                        continue;

                    // Kolejny węzeł przejmuje część aktualnego wpływu, np. 90% × 0,8 = 72%.
                    var impactRatio = currentImpact.ImpactRatio * edge.ImportanceWeight;

                    // Opóźnienia sumują się wzdłuż ścieżki, np. 12:01 + 5 minut = 12:06.
                    var effectiveAt = currentImpact.EffectiveAt + edge.PropagationDelay;

                    // Zachowujemy dotychczasową drogę i dopisujemy kolejny węzeł.
                    var path = currentImpact.Path.Append(edge.TargetNodeId);
                    var targetImpact = new NodeImpact(edge.TargetNodeId, impactRatio, effectiveAt, path);

                    // Zachowujemy obliczony wynik do zwrócenia użytkownikowi.
                    impacts.Add(targetImpact);

                    // Zaplanujemy również analizę jego połączeń, aby problem przeszedł dalej.
                    pendingImpacts.Enqueue(targetImpact);
                }
            }

            // Dla każdego węzła wybieramy największy wpływ.
            // Przy równym wpływie wybieramy ścieżkę, której skutek pojawia się wcześniej.
            return impacts
                .GroupBy(impact => impact.NodeId)
                .Select(group => group.OrderByDescending(impact => impact.ImpactRatio).ThenBy(impact => impact.EffectiveAt).First())
                .ToList()
                .AsReadOnly();
        }
    }
}