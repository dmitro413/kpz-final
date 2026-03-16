namespace WpfLibrary.services
{
    public interface IGameHistoryService
    {
        IReadOnlyList<models.GameHistoryEntry> GetRecent(int count = 20);
        void Record(models.GameHistoryEntry entry);
        void Clear();
    }
}