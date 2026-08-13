using ImpactFlow.Core.Atlas;

namespace ImpactFlow.Core.Tests.Atlas
{
    public class AtlasGraphTests
    {
        [Fact]
        public void Constructor_CreatesGraph_WhenNodesAndEdgesAreValid()
        {
            //Arrange
            var paymentApi = CreateNode("Payment API", NodeType.ExternalService);
            var paymentAuthorization = CreateNode("Autoryzacja płatności", NodeType.ProcessStep);

            var edge = CreateEdge(paymentApi.Id, paymentAuthorization.Id);

            //Act
            var graph = new AtlasGraph(new[] { paymentApi, paymentAuthorization }, new[] { edge });

            //Assert
            Assert.Equal(2, graph.Nodes.Count);
            Assert.Single(graph.Edges);
            Assert.Equal(paymentApi, graph.GetNode(paymentApi.Id));
        }

        [Fact]
        public void GetOutgoingEdges_ReturnsEdgesStartingFromSpecifiedNode()
        {
            var paymentApi = CreateNode("Payment API", NodeType.ExternalService);
            var paymentAuthorization = CreateNode("Autoryzacja płatności", NodeType.ProcessStep);
            var orderHandling = CreateNode("Obsługa zamówienia", NodeType.BuisnessProcess);

            var paymentEdge = CreateEdge(paymentApi.Id, paymentAuthorization.Id);
            var processEdge = CreateEdge(paymentAuthorization.Id, orderHandling.Id);

            var graph = new AtlasGraph(
                new[] { paymentApi, paymentAuthorization, orderHandling },
                new[] { paymentEdge, processEdge });

            var outgoingEdges = graph.GetOutgoingEdges(paymentApi.Id);

            var returnedEdge = Assert.Single(outgoingEdges);
            Assert.Equal(paymentEdge.Id, returnedEdge.Id);
            Assert.Equal(paymentAuthorization.Id, returnedEdge.TargetNodeId);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenSourceNodeDoesNotExist()
        {
            var targetNode = CreateNode("Autoryzacja płatności", NodeType.ProcessStep);
            var missingSourceNodeId = Guid.NewGuid();
            var edge = CreateEdge(missingSourceNodeId, targetNode.Id);

            var exception = Assert.Throws<ArgumentException>(() =>
                new AtlasGraph(new[] { targetNode }, new[] { edge }));

            Assert.Equal("edges", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenTargetNodeDoesNotExist()
        {
            var sourceNode = CreateNode("Payment API", NodeType.ExternalService);
            var missingTargetNodeId = Guid.NewGuid();
            var edge = CreateEdge(sourceNode.Id, missingTargetNodeId);

            var exception = Assert.Throws<ArgumentException>(() =>
                new AtlasGraph(new[] { sourceNode }, new[] { edge }));

            Assert.Equal("edges", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenNodeIdIsDuplicated()
        {
            var id = Guid.NewGuid();

            var firstNode = new AtlasNode(id, "Payment API", NodeType.ExternalService, 5);
            var secondNode = new AtlasNode(id, "Inna usługa", NodeType.ExternalService, 3);

            var exception = Assert.Throws<ArgumentException>(() =>
                new AtlasGraph(
                    new[] { firstNode, secondNode },
                    Array.Empty<DependencyEdge>()));

            Assert.Equal("nodes", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenConnectionIsDuplicated()
        {
            var sourceNode = CreateNode("Payment API", NodeType.ExternalService);
            var targetNode = CreateNode("Autoryzacja płatności", NodeType.ProcessStep);

            var firstEdge = CreateEdge(sourceNode.Id, targetNode.Id);
            var secondEdge = CreateEdge(sourceNode.Id, targetNode.Id);

            var exception = Assert.Throws<ArgumentException>(() =>
                new AtlasGraph(
                    new[] { sourceNode, targetNode },
                    new[] { firstEdge, secondEdge }));

            Assert.Equal("edges", exception.ParamName);
        }

        [Fact]
        public void GetNode_ThrowsException_WhenNodeDoesNotExist()
        {
            var graph = new AtlasGraph(
                Array.Empty<AtlasNode>(),
                Array.Empty<DependencyEdge>());

            Assert.Throws<KeyNotFoundException>(() => graph.GetNode(Guid.NewGuid()));
        }

        [Fact]
        public void GetOutgoingEdges_ReturnsEmptyCollection_WhenNodeHasNoOutgoingEdges()
        {
            var node = CreateNode("Payment API", NodeType.ExternalService);

            var graph = new AtlasGraph(
                new[] { node },
                Array.Empty<DependencyEdge>());

            var outgoingEdges = graph.GetOutgoingEdges(node.Id);

            Assert.Empty(outgoingEdges);
        }

        private static AtlasNode CreateNode(string name, NodeType type)
        {
            return new AtlasNode(Guid.NewGuid(), name, type, 5);
        }

        private static DependencyEdge CreateEdge(Guid sourceNodeId, Guid targetNodeId)
        {
            return new DependencyEdge(Guid.NewGuid(), sourceNodeId, targetNodeId, DependencyMode.Required, 1m, TimeSpan.Zero);
        }
    }
}
