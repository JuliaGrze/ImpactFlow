namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Określa sposób, w jaki element zależny korzysta ze swojej zależności
    /// </summary>
    public enum DependencyMode
    {
        Required,
        Optional,
        Redundant //istnieją równoległe elementy mogące przejąć pracę np.: Autoryzacja płatności → kilku operatorów płatności
    }
}
