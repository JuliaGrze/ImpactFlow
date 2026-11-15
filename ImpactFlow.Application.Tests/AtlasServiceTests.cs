using ImpactFlow.Application.Atlas;
using ImpactFlow.Contracts.Commands.Atlas;
using ImpactFlow.Core.Atlas;
using MediatR;

namespace ImpactFlow.Application.Tests
{
    public class AtlasServiceTests
    {
        [Fact]
        public async Task CreateAsync_SendsSaveCommandWithMapAndFirstDraft()
        {
            var sender = new RecordingSender();
            var service = new AtlasService(sender);

            var result = await service.CreateAsync("Obsługa zamówień", CancellationToken.None);

            Assert.Equal("Obsługa zamówień", result.Map.Name);
            Assert.Equal(result.Map.Id, result.DraftVersion.MapId);
            Assert.Equal(1, result.DraftVersion.Number);
            Assert.Equal(AtlasVersionStatus.Draft, result.DraftVersion.Status);
            Assert.Empty(result.DraftVersion.Nodes);
            Assert.Empty(result.DraftVersion.Edges);

            var command = Assert.IsType<SaveAtlasCommand>(sender.SentCommand);
            Assert.Same(result.Map, command.Map);
            Assert.Same(result.DraftVersion, command.InitialDraft);
        }

        private sealed class RecordingSender : ISender
        {
            public IRequest? SentCommand { get; private set; }

            public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
            {
                SentCommand = request;
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => throw new NotSupportedException();

            public Task<object?> Send(object request, CancellationToken cancellationToken = default) => throw new NotSupportedException();

            public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default) => throw new NotSupportedException();

            public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        }
    }
}