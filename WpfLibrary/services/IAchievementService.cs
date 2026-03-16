using WpfLibrary.models;

namespace WpfLibrary.services
{
    public interface IAchievementService
    {
        IReadOnlyList<Achievement> GetAll();

        IReadOnlyList<Achievement> GetUnlocked();

        IReadOnlyList<Achievement> CheckOnWin(
            DifficultyLevel difficulty,
            int timeSeconds,
            int flagsUsed,
            int winStreak);
        IReadOnlyList<Achievement> CheckOnLoss();

        void Reset();
    }
}