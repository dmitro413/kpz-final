using WpfLibrary.models;

namespace WpfLibrary.services
{
    public interface IAchievementService
    {
        IReadOnlyList<Achievement> GetAll();

        IReadOnlyList<Achievement> GetUnlocked();

        IReadOnlyList<Achievement> CheckOnWin(GameResult result);
        IReadOnlyList<Achievement> CheckOnLoss();

        void Reset();
    }
}