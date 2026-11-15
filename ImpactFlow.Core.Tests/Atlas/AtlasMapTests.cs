using ImpactFlow.Core.Atlas;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImpactFlow.Core.Tests.Atlas
{
    public class AtlasMapTests
    {
        [Fact]
        public void Constructor_CreatesMap_WhenArgumentsAreValid()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var map = new AtlasMap(id, "Obsługa zamówień");

            // Assert
            Assert.Equal(id, map.Id);
            Assert.Equal("Obsługa zamówień", map.Name);
        }

        [Fact]
        public void Constructor_TrimsMapName()
        {
            var map = new AtlasMap(Guid.NewGuid(), "  Obsługa zamówień  ");

            Assert.Equal("Obsługa zamówień", map.Name);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenIdIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() => new AtlasMap(Guid.Empty, "Obsługa zamówień"));

            Assert.Equal("id", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ThrowsException_WhenNameIsBlank(string? name)
        {
            // Celowo przekazujemy także null, aby sprawdzić ochronę konstruktora
            var exception = Assert.Throws<ArgumentException>(() => new AtlasMap(Guid.NewGuid(), name!));

            Assert.Equal("name", exception.ParamName);
        }
    }
}
