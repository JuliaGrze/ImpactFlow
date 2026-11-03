using ImpactFlow.Core.Atlas;
using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Application.ImpactAnalysis.AnalyzeImpact
{
    /// <summary>
    /// Dane potrzebne do wykonania analizy wpływu: elementy mapy, połączenia między nimi oraz obserwacja stanu węzła źródłowego
    /// </summary>
    public sealed record AnalyzeImpactInput(IReadOnlyList<AtlasNode> Nodes, IReadOnlyList<DependencyEdge> Edges, NodeObservation Observation);
}
