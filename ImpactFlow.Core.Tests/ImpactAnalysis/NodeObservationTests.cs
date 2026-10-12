using ImpactFlow.Core.ImpactAnalysis;

namespace ImpactFlow.Core.Tests.ImpactAnalysis
{
    public class NodeObservationTests
    {
        [Theory]
        [InlineData(ObservedStatus.Healthy, 0)]
        [InlineData(ObservedStatus.Degraded, 40)]
        [InlineData(ObservedStatus.Unavailable, 100)]
        [InlineData(ObservedStatus.Unknown, null)]
        public void Constructor_CreatesObservation_WhenArgumentsAreValid(ObservedStatus status, int? severityPercent)
        {
            // Arrange — procent zamieniamy na wartość 0–1; null pozostaje null.
            var nodeId = Guid.NewGuid();
            var observedAt = new DateTimeOffset(2026, 8, 17, 21, 46, 0, TimeSpan.FromHours(2));
            decimal? severity = severityPercent / 100m;

            // Act
            var observation = new NodeObservation(nodeId, status, observedAt, severity);

            // Assert
            Assert.Equal(nodeId, observation.NodeId);
            Assert.Equal(status, observation.Status);
            Assert.Equal(observedAt, observation.ObservedAt);
            Assert.Equal(severity, observation.Severity);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenNodeIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() => 
                new NodeObservation(Guid.Empty, ObservedStatus.Unavailable, DateTimeOffset.UtcNow, 1m));

            Assert.Equal("nodeId", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenStatusIsUndefined()
        {
            var undefinedStatus = (ObservedStatus)999;

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new NodeObservation(Guid.NewGuid(), undefinedStatus, DateTimeOffset.UtcNow, 1m));

            Assert.Equal("status", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenObservedAtIsNotSpecified()
        {
            var exception = Assert.Throws<ArgumentException>(() => 
                new NodeObservation(Guid.NewGuid(), ObservedStatus.Unavailable, default, 1m));

            Assert.Equal("observedAt", exception.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Constructor_ThrowsException_WhenSeverityIsOutsideRange(int severityPercent)
        {
            var severity = severityPercent / 100m;

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new NodeObservation(Guid.NewGuid(), ObservedStatus.Degraded, DateTimeOffset.UtcNow, severity));

            Assert.Equal("severity", exception.ParamName);
        }

        [Theory]
        [InlineData(ObservedStatus.Healthy, 40)]
        [InlineData(ObservedStatus.Healthy, null)]
        [InlineData(ObservedStatus.Unavailable, 40)]
        [InlineData(ObservedStatus.Unavailable, null)]
        [InlineData(ObservedStatus.Degraded, 0)]
        [InlineData(ObservedStatus.Degraded, 100)]
        [InlineData(ObservedStatus.Degraded, null)]
        [InlineData(ObservedStatus.Unknown, 0)]
        [InlineData(ObservedStatus.Unknown, 40)]
        [InlineData(ObservedStatus.Unknown, 100)]
        public void Constructor_ThrowsException_WhenSeverityDoesNotMatchStatus(ObservedStatus status, int? severityPercent)
        {
            decimal? severity = severityPercent / 100m;

            var exception = Assert.Throws<ArgumentException>(() => new NodeObservation(Guid.NewGuid(), status, DateTimeOffset.UtcNow, severity));

            Assert.Equal("severity", exception.ParamName);
        }
    }
}