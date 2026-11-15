using ImpactFlow.Contracts.Commands.Atlas;
using MediatR;

namespace ImpactFlow.Infrastructure.Persistence.Atlas
{
    public sealed class SaveAtlasCommandHandler : IRequestHandler<SaveAtlasCommand>
    {
        private readonly ImpactFlowDbContext _dbContext;

        public SaveAtlasCommandHandler(ImpactFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(SaveAtlasCommand command, CancellationToken cancellationToken)
        {
            _dbContext.AtlasMaps.Add(command.Map);
            _dbContext.AtlasVersions.Add(command.InitialDraft);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
