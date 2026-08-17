using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Core.Tests.ImpactAnalysis
{
    public class NodeObservationTests
    {
        [Fact]
        public void Constructor_CreatesObservation_WhenArgumentsAreValid()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var observedAt = new DateTimeOffset(2026, 8, 17, 21, 46, 0, TimeSpan.FromHours(2));

            // Act
            var observation = new NodeObservation(nodeId, ObservedStatus.Unavailable, observedAt);

            // Assert
            Assert.Equal(nodeId, observation.NodeId);
            Assert.Equal(ObservedStatus.Unavailable, observation.Status);
            Assert.Equal(observedAt, observation.ObservedAt);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenNodeIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new NodeObservation(Guid.Empty, ObservedStatus.Unavailable, DateTimeOffset.UtcNow));

            Assert.Equal("nodeId", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenStatusIsUnknown()
        {
            var unknowStatus = (ObservedStatus)999;

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new NodeObservation(Guid.NewGuid(), unknowStatus, DateTimeOffset.UtcNow));

            Assert.Equal("status", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenObservedAtIsNotSpecified()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new NodeObservation(Guid.NewGuid(), ObservedStatus.Unavailable, default));

            Assert.Equal("observedAt", exception.ParamName);
        }
    }
}
