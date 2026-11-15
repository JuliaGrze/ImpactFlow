namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Reprezentuje wersję konkretnej mapy. Nowa wersja rozpoczyna się jako robocza.
    /// </summary>
    public sealed class AtlasVersion
    {
        private readonly List<AtlasNode> _nodes = new();
        private readonly List<DependencyEdge> _edges = new();

        public Guid Id { get; }
        public Guid MapId { get; }
        public int Number { get; }
        public AtlasVersionStatus Status { get; private set; }
        public IReadOnlyList<AtlasNode> Nodes { get; }
        public IReadOnlyList<DependencyEdge> Edges { get; }

        public AtlasVersion(Guid id, Guid mapId, int number)
        {
            if (id == Guid.Empty) throw new ArgumentException("Identyfikator wersji nie może być pusty.", nameof(id));
            if (mapId == Guid.Empty) throw new ArgumentException("Identyfikator mapy nie może być pusty.", nameof(mapId));
            if (number < 1) throw new ArgumentOutOfRangeException(nameof(number), "Numer wersji musi być większy od zera.");

            Id = id;
            MapId = mapId;
            Number = number;
            Status = AtlasVersionStatus.Draft;
            Nodes = _nodes.AsReadOnly();
            Edges = _edges.AsReadOnly();
        }

        /// <summary>
        /// Dodaje węzeł do wersji roboczej, nie dopuszczając powtarzających się identyfikatorów.
        /// </summary>
        public void AddNode(AtlasNode node)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (Status != AtlasVersionStatus.Draft) throw new InvalidOperationException("Węzły można dodawać tylko do wersji roboczej.");
            if (_nodes.Any(existingNode => existingNode.Id == node.Id)) throw new ArgumentException($"Węzeł o identyfikatorze {node.Id} już istnieje w tej wersji.", nameof(node));

            _nodes.Add(node);
        }

        /// <summary>
        /// Dodaje połączenie do wersji roboczej. Oba węzły muszą należeć do tej wersji, a między nimi może istnieć tylko jedno połączenie w danym kierunku.
        /// </summary>
        public void AddEdge(DependencyEdge edge)
        {
            ArgumentNullException.ThrowIfNull(edge);

            if (Status != AtlasVersionStatus.Draft) throw new InvalidOperationException("Połączenia można dodawać tylko do wersji roboczej.");
            if (!_nodes.Any(node => node.Id == edge.SourceNodeId)) throw new ArgumentException("Węzeł źródłowy nie należy do tej wersji.", nameof(edge));
            if (!_nodes.Any(node => node.Id == edge.TargetNodeId)) throw new ArgumentException("Węzeł docelowy nie należy do tej wersji.", nameof(edge));
            if (_edges.Any(existing => existing.Id == edge.Id)) throw new ArgumentException("Połączenie o tym identyfikatorze już istnieje.", nameof(edge));

            // Nowe Id nie pozwala dodać drugierj strzałki między tą samą parą w tym samym kierunku
            if (_edges.Any(existing => existing.SourceNodeId == edge.SourceNodeId && existing.TargetNodeId == edge.TargetNodeId))
                throw new ArgumentException("Takie połączenie już istnieje w tej wersji.", nameof(edge));

            _edges.Add(edge);
        }

        /// <summary>
        /// Publikuję wersję roboczą mapy, zamykając ją na dalsze zmiany.
        /// </summary>
        public void Publish()
        {
            if (Status != AtlasVersionStatus.Draft) throw new InvalidOperationException("Opublikować można tylko wersję roboczą.");
            if (_nodes.Count == 0) throw new InvalidOperationException("Nie można opublikować wersji bez węzłów.");

            Status = AtlasVersionStatus.Published;
        }

        /// <summary>
        /// Tworzy graf do bieżącej analizy na podstawie opublikowanej wersji mapy.
        /// </summary>
        public AtlasGraph CreateAnalysisGraph()
        {
            if(Status != AtlasVersionStatus.Published) throw new InvalidOperationException("Do bieżącej analizy można użyć tylko opublikowanej wersji.");

            return new AtlasGraph(_nodes, _edges);
        }
    }
}