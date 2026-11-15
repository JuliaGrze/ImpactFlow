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
            ArgumentNullException.ThrowIfNull(input.Version);

            var graph = input.Version.CreateAnalysisGraph();

            return _analyzer.Analyze(graph, input.Observation);
        }

    }
}
