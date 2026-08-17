namespace ImpactFlow.Core.ImpactAnalysis
{

    /// <summary>
    /// Reprezentuje stan konkretnego węzła zaobserwowany w określonym czasie.
    /// Jest informacją wejściową dla późniejszej analizy wpływu.
    /// </summary>
    public sealed class NodeObservation
    {
        public Guid NodeId { get; }
        public ObservedStatus Status { get; }
        public DateTimeOffset ObservedAt { get; }

        public NodeObservation(Guid nodeId, ObservedStatus status, DateTimeOffset observedAt)
        {
            if(nodeId == Guid.Empty)
                throw new ArgumentException("Identyfikator obserwowanego węzła nie może być pusty.", nameof(nodeId));
            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException(nameof(status), "Nieznany stan obserwowany.");
            if(observedAt == default)
                throw new ArgumentException("Czas obserwacji musi być określony.", nameof(observedAt));

            NodeId = nodeId;
            Status = status;
            ObservedAt = observedAt;
        }
    }
}
