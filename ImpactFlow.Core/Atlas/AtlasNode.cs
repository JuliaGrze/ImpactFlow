namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Reprezentuje pojedyńczy element na mapie zależności firmy
    /// </summary>
    public sealed class AtlasNode
    {
        public Guid Id { get; }
        public string Name { get; }
        public NodeType Type { get; }

        /// <summary>
        /// Krytyczność biznesowa węzła
        /// </summary>
        public int Criticality { get; }

        public AtlasNode(Guid id, string name, NodeType type, int criticality)
        {
            if(id == Guid.Empty)
                throw new ArgumentException($"Identyfikator węzła nie może być pusty", nameof(id));
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Nazwa węzła nie może być pusta", nameof(name));
            if (criticality is < 1 or > 5)
                throw new ArgumentOutOfRangeException(nameof(criticality), "Krytyczność musi mieścić się w zakresie od 1 do 5.");

            Id = id;
            Name = name.Trim();
            Type = type;
            Criticality = criticality;
        }
    }
}
