namespace ImpactFlow.Core.ImpactAnalysis
{
    /// <summary>
    /// Stan elementu ustalony na podstawie obserwacji lub ręcznego zgłoszenia.
    /// </summary>
    public enum ObservedStatus
    {
        Unknown, // nie mamy wystarczających danych
        Healthy, // element działa prawidłowo
        Degraded, // działa, ale gorzej niż powinien
        Unavailable // nie działa
    }
}
