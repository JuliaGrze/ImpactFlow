namespace ImpactFlow.Core.Atlas
{
    /// <summary>
    /// Określa, czy wersja mapy jest edytowalna, czy została opublikowana.
    /// </summary>
    public enum AtlasVersionStatus
    {
        Draft, // wersja robocza; można zmieniać jej węzły i połączenia
        Published // wersja opublikowana; jej zawartości nie zmieniamy. Kolejne zmiany wymagają nowej wersji roboczej
    }
}