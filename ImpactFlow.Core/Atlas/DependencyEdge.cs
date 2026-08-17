namespace ImpactFlow.Core.Atlas
{

    /// <summary>
    /// Reprezentuje połączenie, po którym wpływ problemu przechodzi
    /// z węzła źródłowego do węzła docelowego.
    /// Przykład: Payment API → Autoryzacja płatności.
    /// </summary>
    public sealed class DependencyEdge
    {
        public Guid Id { get; }

        /// <summary>
        /// Węzeł, z którego rozpoczyna się przepływ wpływu.
        /// Przykład: Payment API.
        /// </summary>
        public Guid SourceNodeId { get; }

        /// <summary>
        /// Węzeł, który może zostać dotknięty problemem źródła.
        /// Przykład: Autoryzacja płatności.
        /// </summary>
        public Guid TargetNodeId { get; }

        public DependencyMode Mode { get; }

        /// <summary>
        /// Określa, jaka część wpływu przechodzi do węzła docelowego.
        /// </summary>
        public decimal ImportanceWeight { get; }

        /// <summary>
        /// Określa czas, po którym wpływ dociera do węzła docelowego.
        /// </summary>
        public TimeSpan PropagationDelay { get; }

        public DependencyEdge(
            Guid id,
            Guid sourceNodeId,
            Guid targetNodeId,
            DependencyMode mode,
            decimal importanceWeight,
            TimeSpan propagationDelay)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Identyfikator zależności nie może być pusty.", nameof(id));

            if (sourceNodeId == Guid.Empty)
                throw new ArgumentException("Identyfikator węzła źródłowego nie może być pusty.", nameof(sourceNodeId));

            if (targetNodeId == Guid.Empty)
                throw new ArgumentException("Identyfikator węzła docelowego nie może być pusty.", nameof(targetNodeId));

            if (sourceNodeId == targetNodeId)
                throw new ArgumentException("Węzeł nie może bezpośrednio wpływać na samego siebie.", nameof(targetNodeId));

            if (!Enum.IsDefined(mode))
                throw new ArgumentOutOfRangeException(nameof(mode), "Nieznany tryb zależności.");

            if (importanceWeight is <= 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(importanceWeight), "Waga musi być większa od 0 i nie większa niż 1.");

            if (propagationDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(propagationDelay), "Opóźnienie propagacji nie może być ujemne.");

            Id = id;
            SourceNodeId = sourceNodeId;
            TargetNodeId = targetNodeId;
            Mode = mode;
            ImportanceWeight = importanceWeight;
            PropagationDelay = propagationDelay;
        }
    }
}