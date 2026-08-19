namespace ImpactFlow.Core.ImpactAnalysis
{
    /// <summary>
    /// Reprezentuje obliczony wpływ problemu na pojedynczy węzeł Atlasu.
    /// </summary>
    public sealed class NodeImpact
    {
        public Guid NodeId { get; }
        public decimal ImpactRatio { get; }

        /// <summary>
        /// Uproszczona pozostała zdolność działania węzła.
        /// </summary>
        public decimal RemainingCapacityRatio => 1m - ImpactRatio;
        public DateTimeOffset EffectiveAt { get; }

        /// <summary>
        /// Identyfikatory węzłów tworzących drogę od źródła problemu do bieżącego węzła.
        /// </summary>
        public IReadOnlyList<Guid> Path { get; }

        public NodeImpact(Guid nodeId, decimal impactRatio, DateTimeOffset effectiveAt, IEnumerable<Guid> path)
        {
            if (nodeId == Guid.Empty)
                throw new ArgumentException("Identyfikator węzła nie może być pusty.", nameof(nodeId));
            if (impactRatio < 0 ||  impactRatio > 1)
                throw new ArgumentOutOfRangeException(nameof(impactRatio), "Wpływ musi mieścić się w zakresie od 0 do 1.");
            if(effectiveAt == default)
                throw new ArgumentException("Czas rozpoczęcia wpływu musi być określony.", nameof(effectiveAt));

            ArgumentNullException.ThrowIfNull(path);

            var pathList = path.ToList();

            if(pathList.Count == 0)
                throw new ArgumentException("Ścieżka wpływu nie może być pusta.", nameof(path));
            if (pathList.Any(id => id == Guid.Empty)) 
                throw new ArgumentException("Ścieżka wpływu nie może zawierać pustych identyfikatorów.", nameof(path));
            if (pathList[^1] != nodeId)  // pathList[^1] oznacza ostatni element listy
                throw new ArgumentException("Ścieżka wpływu musi kończyć się analizowanym węzłem.", nameof(path));

            NodeId = nodeId;
            ImpactRatio = impactRatio;
            EffectiveAt = effectiveAt;
            Path = pathList.AsReadOnly();
        }
    }
}
