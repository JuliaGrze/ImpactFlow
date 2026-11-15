using System.ComponentModel.DataAnnotations;

namespace ImpactFlow.Api.Contracts.Atlas
{
    public sealed class CreateAtlasRequest
    {
        [Required(ErrorMessage = "Podaj nazwę mapy.")]
        public string Name { get; init; } = string.Empty;
    }
}
