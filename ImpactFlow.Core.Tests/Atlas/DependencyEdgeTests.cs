using ImpactFlow.Core.Atlas;

namespace ImpactFlow.Core.Tests.Atlas
{
    public class DependencyEdgeTests
    {
        [Fact]
        public void Constructor_CreatesDependency_WhenArgumentsAreValid()
        {
            //Arrange
            var id = Guid.NewGuid();
            var sourceNodeId = Guid.NewGuid();
            var targetNodeId = Guid.NewGuid();
            var delay = TimeSpan.FromMinutes(15);

            //Act
            var edge = new DependencyEdge(id, sourceNodeId, targetNodeId, DependencyMode.Required ,0.8m, delay);

            //Assert
            Assert.Equal(id, edge.Id);
            Assert.Equal(sourceNodeId, edge.SourceNodeId);
            Assert.Equal(targetNodeId, edge.TargetNodeId);
            Assert.Equal(DependencyMode.Required, edge.Mode);
            Assert.Equal(0.8m, edge.ImportanceWeight);
            Assert.Equal(delay, edge.PropagationDelay);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(2)]
        public void Constructor_ThrowsException_WhenWeightIsOutsideRange(int weight)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DependencyEdge(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DependencyMode.Required, weight, TimeSpan.FromMinutes(15))
            );

            Assert.Equal("importanceWeight", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenDelayIsNegative()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DependencyEdge(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DependencyMode.Required, 1m, TimeSpan.FromMinutes(-1))
            );

            Assert.Equal("propagationDelay", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenModeIsUnknown()
        {
            var unknowMode = (DependencyMode)999;

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DependencyEdge(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), unknowMode, 1m, TimeSpan.FromMinutes(15))
            );

            Assert.Equal("mode", exception.ParamName);
        }

    }
}
