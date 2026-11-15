using ImpactFlow.Core.Atlas;
using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Application.ImpactAnalysis.AnalyzeImpact
{
    //// <summary>
    /// Wersja mapy i obserwacja stanu źródła potrzebne do analizy wpływu.
    /// </summary>
    public sealed record AnalyzeImpactInput(AtlasVersion Version, NodeObservation Observation);
}
