using ImpactFlow.Core.Atlas;

namespace ImpactFlow.Core.Tests.Atlas
{
    public sealed class AtlasNodeTests
    {
        [Fact]
        public void Constructor_CreatesNode_WhenArgumentsAreValid()
        {
            //Arrange
            var id = Guid.NewGuid();
            //Act
            var node = new AtlasNode(id, "Payment API", NodeType.ExternalService, 5);
            //Assert
            Assert.Equal(id, node.Id);
            Assert.Equal("Payment API", node.Name);
            Assert.Equal(NodeType.ExternalService, node.Type);
            Assert.Equal(5, node.Criticality);            
        }

        [Fact]
        public void Constructor_ThrowsException_WhenIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new AtlasNode(Guid.Empty, "Payment API", NodeType.ExternalService, 5)
            );

            Assert.Equal("id", exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ThrowsException_WhenNameIsBlank(string name)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new AtlasNode(Guid.NewGuid(), name, NodeType.ExternalService, 5)
            );

            Assert.Equal("name", exception.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Constructor_ThrowsException_WhenCriticalityIsOutsideRange(int criticality)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AtlasNode(Guid.NewGuid(), "Payment API", NodeType.ExternalService, criticality));

            Assert.Equal("criticality", exception.ParamName);
        }

        [Fact]
        public void Constructor_TrimsName()
        {
            var node = new AtlasNode(Guid.NewGuid(), "  Payment API  ", NodeType.ExternalService, 5);

            Assert.Equal("Payment API", node.Name);
        }

    }
}
