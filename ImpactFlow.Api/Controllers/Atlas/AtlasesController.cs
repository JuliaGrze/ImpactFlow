using ImpactFlow.Api.Contracts.Atlas;
using ImpactFlow.Application.Atlas;
using Microsoft.AspNetCore.Mvc;

namespace ImpactFlow.Api.Controllers.Atlas
{
    [ApiController]
    [Route("api/atlases")]
    public class AtlasesController : ControllerBase
    {
        private readonly AtlasService _createAtlasHandler;

        public AtlasesController(AtlasService createAtlasHandler)
        {
            _createAtlasHandler = createAtlasHandler;
        }

        [HttpPost]
        public async Task<ActionResult<CreateAtlasResponse>> Create(CreateAtlasRequest request, CancellationToken cancellationToken)
        {
            var (map, draft) = await _createAtlasHandler.CreateAsync(request.Name, cancellationToken);
            var response = new CreateAtlasResponse(map.Id, map.Name, draft.Id);

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
