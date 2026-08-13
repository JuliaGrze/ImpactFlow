namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Reprezentuje kompletną mapę węzłów oraz połączeń, po których problemy mogą wpływać na kolejne elementy firmy
    /// </summary>
    public class AtlasGraph
    {
        // Słownik pozwala szybko znaleźć węzeł na podstawie jego identyfikatora
        private readonly Dictionary<Guid, AtlasNode> _nodesById;
        // Dla każdego węzła źródłowego przechowuje krawędzie, którymi wpływ może przejść do następnych węzłów
        private readonly Dictionary<Guid, IReadOnlyList<DependencyEdge>> _outgoingEdgesBySourceId;

        public IReadOnlyList<AtlasNode> Nodes { get; }
        public IReadOnlyList<DependencyEdge> Edges { get; }

        public AtlasGraph(IEnumerable<AtlasNode> nodes, IEnumerable<DependencyEdge> edges)
        {
            ArgumentNullException.ThrowIfNull(nodes);
            ArgumentNullException.ThrowIfNull(edges);

            var nodeList = nodes.ToList();
            var edgeList = edges.ToList();

            // Dwa węzły nie mogą mieć tego samego identyfikatora
            var duplicateNodeId = nodeList.GroupBy(node => node.Id).FirstOrDefault(id => id.Count() > 1);
            if (duplicateNodeId != null)
                throw new ArgumentException("Węzły nie mogą mieć powtarzających się identyfikatorów.", nameof(nodes));

            // Dwie krawędzie nie mogą mieć tego samego identyfikatora
            var duplicateEdgeId = edgeList.GroupBy(edge => edge.Id).FirstOrDefault(id => id.Count() > 1);
            if(duplicateEdgeId != null)
                throw new ArgumentException($"Zależność o identyfikatorze {duplicateEdgeId} występuje więcej niż raz.", nameof(edges));

            // {Pozwalamy mieć tylko jedno bezpośrednie połączenie pomiędzy tą samą parą węzłów
            var duplicateConnection = edgeList.GroupBy(edge => new { edge.SourceNodeId, edge.TargetNodeId }).FirstOrDefault(group => group.Count() > 1);
            if (duplicateConnection is not null)
                throw new ArgumentException("Pomiędzy tymi samymi węzłami istnieje więcej niż jedna zależność.", nameof(edges));

            // Budujemy indeks węzłów używany do szybkiego wyszukiwania po Guid
            _nodesById = nodeList.ToDictionary(node => node.Id);

            // Każda krawędź musi wskazywać węzły należące do tego grafu
            foreach (var edge in edgeList)
            {
                if (!_nodesById.ContainsKey(edge.SourceNodeId))
                    throw new ArgumentException($"Źródłowy węzeł {edge.SourceNodeId} nie istnieje w grafie.", nameof(edges));

                if (!_nodesById.ContainsKey(edge.TargetNodeId))
                    throw new ArgumentException($"Docelowy węzeł {edge.TargetNodeId} nie istnieje w grafie.", nameof(edges));
            }

            // Grupujemy wychodzące krawędzie według węzła źródłowego.
            // Później analizator wpływu użyje tego indeksu do przechodzenia po grafie
            _outgoingEdgesBySourceId = edgeList.GroupBy(edge => edge.SourceNodeId).ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<DependencyEdge>)group.ToArray());

            // Na zewnątrz udostępniamy kolekcje tylko do odczytu,
            // aby po utworzeniu graf nie został przypadkowo uszkodzony
            Nodes = nodeList.AsReadOnly();
            Edges = edgeList.AsReadOnly();
        }

        /// <summary>
        /// Zwraca węzeł o podanym identyfikatorze
        /// </summary>
        public AtlasNode GetNode(Guid id)
        {
            if (!_nodesById.TryGetValue(id, out var node))
                throw new KeyNotFoundException($"Węzeł o identyfikatorze {id} nie istnieje.");

            return node;
        }

        /// <summary>
        /// Zwraca połączenia, którymi wpływ może przejść z podanego węzła do kolejnych elementów
        /// </summary>
        public IReadOnlyList<DependencyEdge> GetOutgoingEdges(Guid sourceNodeId)
        {
            return _outgoingEdgesBySourceId.TryGetValue(sourceNodeId, out var edges) ? edges : Array.Empty<DependencyEdge>();
        }
    }
}
