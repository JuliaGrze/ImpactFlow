using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Core.Tests.ImpactAnalysis
{
    public class NodeImpactTests
    {
        [Fact]
        public void Constructor_CreatesNodeImpact_WhenArgumentsAreValid()
        {
            // Arrange
            var sourceNodeId = Guid.NewGuid();
            var nodeId = Guid.NewGuid();
            var effectiveAt = new DateTimeOffset(2026, 8, 17, 12, 1, 0, TimeSpan.FromHours(2));
            var path = new[] { sourceNodeId, nodeId };

            // Act
            var impact = new NodeImpact(nodeId, 0.9m, effectiveAt, path);

            // Assert
            Assert.Equal(nodeId, impact.NodeId);
            Assert.Equal(0.9m, impact.ImpactRatio);
            Assert.Equal(0.1m, impact.RemainingCapacityRatio);
            Assert.Equal(effectiveAt, impact.EffectiveAt);
            Assert.Equal(path, impact.Path);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenNodeIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new NodeImpact(Guid.Empty, 0.9m, DateTimeOffset.UtcNow, new[] { Guid.NewGuid() }));

            Assert.Equal("nodeId", exception.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void Constructor_ThrowsException_WhenImpactRatioIsOutsideRange(int impactRatio)
        {
            var nodeId = Guid.NewGuid();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new NodeImpact(nodeId, impactRatio, DateTimeOffset.UtcNow, new[] { nodeId }));

            Assert.Equal("impactRatio", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenEffectiveAtIsNotSpecified()
        {
            var nodeId = Guid.NewGuid();

            var exception = Assert.Throws<ArgumentException>(() =>
                new NodeImpact(nodeId, 0.9m, default, new[] { nodeId }));

            Assert.Equal("effectiveAt", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenPathIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new NodeImpact(Guid.NewGuid(), 0.9m, DateTimeOffset.UtcNow, null!));

            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenPathIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new NodeImpact(Guid.NewGuid(), 0.9m, DateTimeOffset.UtcNow, Array.Empty<Guid>()));

            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenPathContainsEmptyId()
        {
            var nodeId = Guid.NewGuid();
            var path = new[] { Guid.Empty, nodeId };

            var exception = Assert.Throws<ArgumentException>(() =>
                new NodeImpact(nodeId, 0.9m, DateTimeOffset.UtcNow, path));

            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenPathDoesNotEndWithNodeId()
        {
            var nodeId = Guid.NewGuid();
            var differentNodeId = Guid.NewGuid();

            var exception = Assert.Throws<ArgumentException>(() =>
                new NodeImpact(nodeId, 0.9m, DateTimeOffset.UtcNow, new[] { differentNodeId }));

            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void Constructor_CopiesPath_ToProtectImpactFromExternalChanges()
        {
            var nodeId = Guid.NewGuid();
            var path = new List<Guid> { Guid.NewGuid(), nodeId };

            var impact = new NodeImpact(nodeId, 0.9m, DateTimeOffset.UtcNow, path);

            path.Clear();

            Assert.Equal(2, impact.Path.Count);
            Assert.Equal(nodeId, impact.Path[^1]);
        }
    }

}
