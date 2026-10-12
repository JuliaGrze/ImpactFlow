namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Rodzaj elementu modelowanego węzła w Atlasie.
    /// </summary>
    public enum NodeType
    {
        BusinessProcess, // np.: Obsługa zamówienia
        ProcessStep, // np.: Autoryzacja płatności
        System, // np.: System magazynowy
        ExternalService, // np.: Payment API 
        Resource // np.: Operator magazynu 
    }
}
