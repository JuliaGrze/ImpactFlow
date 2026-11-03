namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Określa tożsamość i nazwę mapy. Jej zawartość należy do poszczególnych wersji.
    /// </summary>
    public sealed class AtlasMap
    {
        public Guid Id { get; }
        public string Name { get; }

        public AtlasMap(Guid id, string name)
        {
            if (id == Guid.Empty) 
                throw new ArgumentException("Identyfikator mapy nie może być pusty.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentException("Nazwa mapy nie może być pusta.", nameof(name));

            Id = id;
            Name = name.Trim();
        }
    }
}