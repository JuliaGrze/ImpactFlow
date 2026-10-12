namespace ImpactFlow.Core.ImpactAnalysis
{
    /// <summary>
    /// Przechowuje zaobserwowany stan węzła, siłę problemu i czas obserwacji.
    /// </summary>
    public sealed class NodeObservation
    {
        public Guid NodeId { get; }
        public ObservedStatus Status { get; }
        public DateTimeOffset ObservedAt { get; }

        /// <summary>
        /// Siła problemu od 0 do 1. Null oznacza brak danych dla stanu Unknown.
        /// </summary>
        public decimal? Severity { get; }

        public NodeObservation(Guid nodeId, ObservedStatus status, DateTimeOffset observedAt, decimal? severity)
        {
            if (nodeId == Guid.Empty) 
                throw new ArgumentException("Identyfikator obserwowanego węzła nie może być pusty.", nameof(nodeId));
            if (!Enum.IsDefined(status)) 
                throw new ArgumentOutOfRangeException(nameof(status), "Nieznany stan obserwowany.");
            if (observedAt == default) 
                throw new ArgumentException("Czas obserwacji musi być określony.", nameof(observedAt));
            if (severity is < 0 or > 1) 
                throw new ArgumentOutOfRangeException(nameof(severity), "Siła problemu musi mieścić się w zakresie od 0 do 1.");

            // Status i siła problemu muszą opisywać tę samą sytuację.
            var severityMatchesStatus = status switch
            {
                ObservedStatus.Unknown => severity is null,
                ObservedStatus.Healthy => severity == 0m,
                ObservedStatus.Unavailable => severity == 1m,
                ObservedStatus.Degraded => severity is > 0 and < 1,
                _ => false
            };

            if (!severityMatchesStatus) 
                throw new ArgumentException("Siła problemu jest niezgodna ze statusem obserwacji.", nameof(severity));

            NodeId = nodeId;
            Status = status;
            ObservedAt = observedAt;
            Severity = severity;
        }
    }
}