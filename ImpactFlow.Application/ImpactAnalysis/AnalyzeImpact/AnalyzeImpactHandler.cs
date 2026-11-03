using ImpactFlow.Core.Atlas;
using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Application.ImpactAnalysis.AnalyzeImpact
{
    /// <summary>
    /// Przygotowuje graf na podstawie danych wejściowych i uruchamia analizę wpływu
    /// </summary>
    public sealed class AnalyzeImpactHandler
    {
        private readonly ImpactAnalyzer _analyzer;

        public AnalyzeImpactHandler(ImpactAnalyzer analyzer)
        {
            ArgumentNullException.ThrowIfNull(analyzer);
            _analyzer = analyzer;
        }

        public IReadOnlyList<NodeImpact> Handle(AnalyzeImpactInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var graph = new AtlasGraph(input.Nodes, input.Edges);
            return _analyzer.Analyze(graph, input.Observation);
        }

    }
}
