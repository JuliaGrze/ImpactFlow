using System;
using System.Collections.Generic;
using System.Text;

namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Reprezentuje skierowaną zależność: element zależny -> jego zależność
    /// np.: Autoryzacja płatności -> Payment API
    /// </summary>
    public class DependencyEdge
    {
        public Guid Id { get; set; }
        public Guid DependentNodeId { get; set; } // np.: Autoryzacja płatności
        public Guid DependencyNodeId { get; } // np.: Payment API
        public DependencyMode Mode { get; }

        /// <summary>
        /// Określa siłę przenoszonego wpływu
        /// 1.0  → pełny wpływ
        /// 0.8  → 80% wpływu
        /// </summary>
        public decimal ImportanceWeight { get; }

        /// <summary>
        /// Określa, po jakim czasie problem stanie się odczuwalny
        /// </summary>
        public TimeSpan PropagationDelay { get; }

        public DependencyEdge(
            Guid id,
            Guid dependentNodeId,
            Guid dependencyNodeId,
            DependencyMode mode,
            decimal importanceWeight,
            TimeSpan propagationDelay)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Identyfikator zależności nie może być pusty.", nameof(id));

            if (dependentNodeId == Guid.Empty)
                throw new ArgumentException("Identyfikator elementu zależnego nie może być pusty.", nameof(dependentNodeId));

            if (dependencyNodeId == Guid.Empty)
                throw new ArgumentException("Identyfikator zależności nie może być pusty.", nameof(dependencyNodeId));

            if (dependentNodeId == dependencyNodeId)
                throw new ArgumentException("Węzeł nie może bezpośrednio zależeć od samego siebie.", nameof(dependencyNodeId));

            if (!Enum.IsDefined(mode))
                throw new ArgumentOutOfRangeException(nameof(mode), "Nieznany tryb zależności.");

            if (importanceWeight is <= 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(importanceWeight), "Waga musi być większa od 0 i nie większa niż 1.");

            if (propagationDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(propagationDelay), "Opóźnienie propagacji nie może być ujemne.");

            Id = id;
            DependentNodeId = dependentNodeId;
            DependencyNodeId = dependencyNodeId;
            Mode = mode;
            ImportanceWeight = importanceWeight;
            PropagationDelay = propagationDelay;
        }

    }
}
